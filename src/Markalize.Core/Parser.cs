
namespace Markalize.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal class Parser
    {
        private static readonly Regex titleEnclosedPrefixRegex = new Regex(@"\[([^\]]+)\]", RegexOptions.Compiled);
        private static readonly Regex keyRegex = new Regex(@"^\s*([A-Z\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Pc}\p{Lm}]+)(@([0-9]+)?([A-Za-z]+)?)?(\s*=\s*)?", RegexOptions.Compiled);

        public Parser()
        {
        }

        internal void Parse(StreamReader reader, ResourceFile resource)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (resource == null)
                throw new ArgumentNullException("resource");

            string line = reader.ReadLine(), nextLine = null;
            int skip = 0;
            var prefixes = new List<string>(5);
            bool isInFencedCode = false, inValue = false;
            while (!reader.EndOfStream)
            {
                nextLine = reader.ReadLine();
                if (skip-- > 0)
                {
                    // don't do anything
                }
                else if (line.StartsWith("```") || line.StartsWith("~~~"))
                {
                    if (isInFencedCode)
                    {
                        isInFencedCode = false;
                    }
                    else
                    {
                        isInFencedCode = true;
                    }
                }
                else if (string.IsNullOrEmpty(line) || isInFencedCode)
                {
                    // don't do anything
                }
                else if (IsTitle1Row(nextLine))
                {
                    // this is a title!
                    skip++;
                    var prefix = FindEnclosedPrefixInTitle(line) ?? ExtractKey(line, out string _, out int __, out string ___);
                    prefixes.Clear();
                    prefixes.Add(prefix);
                }
                else if (IsTitle2Row(nextLine))
                {
                    // this is a title!
                    skip++;
                    var prefix = FindEnclosedPrefixInTitle(line) ?? ExtractKey(line, out string _, out int __, out string ___);

                    while (prefixes.Count > 1)
                    {
                        prefixes.RemoveAt(1); // keep title1 prefix;
                    }

                    prefixes.Add(prefix);
                }
                else if (IsResetTitleRow(line))
                {
                    prefixes.Clear();
                }
                else
                {
                    var key = ExtractKey(line, out string rawValue, out int number, out string genre);

                    if (key != null && prefixes.Count > 0)
                    {
                        key = string.Concat(prefixes) + key;
                    }

                    if (key != null)
                    {
                        var valueTrimmed = rawValue.Trim();
                        if (valueTrimmed.Length > 0 && valueTrimmed[0] == '"')
                        {
                            inValue = true;
                        }

                        // TODO: handle various forms of value

                        resource.Set(key, number, genre, rawValue);
                    }
                    else
                    {
                    }
                }

                line = nextLine;
            }

            if (!string.IsNullOrEmpty(line))
            {
                throw new MarkalizeException("File does not seem to end with an empty line");
            }
        }

        private static string ExtractKey(string line, out string value, out int number, out string genre)
        {
            var match = keyRegex.Match(line);
            if (match.Success)
            {
                var key = match.Groups[1].Value;

                if (match.Groups[2].Success && match.Groups[3].Success)
                {
                    number = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                }
                else
                {
                    number = 0;
                }

                if (match.Groups[2].Success && match.Groups[4].Success)
                {
                    genre = match.Groups[4].Value;
                }
                else
                {
                    genre = null;
                }

                value = line.Substring(match.Length);

                return key;
            }
            else
            {
                number = 0;
                genre = null;
                value = line;
                return null;
            }
        }

        private static string FindEnclosedPrefixInTitle(string line)
        {
            var match = titleEnclosedPrefixRegex.Match(line);
            if (match.Success && match.Groups[1].Success)
            {
                // This is a title with a [prefix_]
                return match.Groups[1].Value.Trim();
            }
            else
            {
                // Prefix_
                return null;
            }
        }

        private static bool IsTitle1Row(string nextLine)
        {
            return nextLine != null && nextLine.Length > 0 && nextLine.Trim().All(c => c == '=');
        }

        private static bool IsTitle2Row(string nextLine)
        {
            return nextLine != null && nextLine.Length > 0 && nextLine.Trim().All(c => c == '-');
        }

        private static bool IsResetTitleRow(string nextLine)
        {
            return nextLine != null && nextLine.Length >= 3 && nextLine.Trim().All(c => c == '-');
        }
    }
}
