
namespace Markalize.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
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

            string line = reader.ReadLine(), nextLine = null, key = null, genre = null, lineEnding;
            int skip = 0, number = 0, quoteCount = 0;
            var prefixes = new List<string>(5);
            bool isInFencedCode = false, inQuotedValue = false, nextLineInContinuation = false;
            var lineBuilder = new StringBuilder();
            StringBuilder sb = null;
            while (!reader.EndOfStream)
            {
                nextLine = ReadLine(reader, out lineEnding);
                if (skip-- > 0)
                {
                    // don't do anything
                }
                else if (inQuotedValue)
                {
                    var valueTrimmed = line.Trim();
                    if (valueTrimmed.Length > 0 && valueTrimmed[valueTrimmed.Length - 1] == '"' && valueTrimmed.Reverse().Take(quoteCount).All(c => c == '"'))
                    {
                        // line ends with '"'
                        // end of value
                        inQuotedValue = false;
                        sb.Append(valueTrimmed, 0, valueTrimmed.Length - quoteCount);
                        resource.Set(key, number, genre, sb.ToString());
                        sb = null;
                    }
                    else
                    {
                        // value continues
                        sb.Append(valueTrimmed);
                        sb.Append(lineEnding);
                    }
                }
                else if (nextLineInContinuation)
                {
                    var valueTrimmed = line.Trim();
                    if (valueTrimmed.Length > 0 && valueTrimmed[valueTrimmed.Length - 1] == '\\')
                    {
                        // line ends with '\'
                        nextLineInContinuation = true;
                        sb.Append(valueTrimmed, 0, valueTrimmed.Length - 1);
                    }
                    else
                    {
                        // end of value
                        nextLineInContinuation = false;
                        sb.Append(valueTrimmed);
                        resource.Set(key, number, genre, sb.ToString());
                        sb = null;
                    }
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
                    key = ExtractKey(line, out string rawValue, out number, out genre);

                    if (key != null && prefixes.Count > 0)
                    {
                        key = string.Concat(prefixes) + key;
                    }

                    if (key != null)
                    {
                        var valueTrimmed = rawValue.Trim();
                        if (valueTrimmed.Length > 0 && valueTrimmed[0] == '"')
                        {
                            // value starts with double quotes
                            quoteCount = valueTrimmed.TakeWhile(c => c == '"').Count();
                            valueTrimmed = valueTrimmed.Substring(quoteCount);

                            if (valueTrimmed.Length > 0 && valueTrimmed[valueTrimmed.Length - 1] == '"' && valueTrimmed.Reverse().Take(quoteCount).All(c => c == '"'))
                            {
                                // value ends with double quotes
                                valueTrimmed = valueTrimmed.Substring(0, valueTrimmed.Length - quoteCount);
                                resource.Set(key, number, genre, valueTrimmed);
                            }
                            else
                            {
                                inQuotedValue = true;
                                sb = new StringBuilder();
                                sb.Append(valueTrimmed);
                                sb.Append(lineEnding);
                            }
                        }
                        else if (valueTrimmed[valueTrimmed.Length - 1] == '\\')
                        {
                            // value ends with backslash
                            nextLineInContinuation = true;
                            sb = new StringBuilder();
                            sb.Append(valueTrimmed.Substring(0, valueTrimmed.Length - 1));
                        }
                        else
                        {
                            // value is a one-liner
                            resource.Set(key, number, genre, valueTrimmed);
                        }
                    }
                    else
                    {
                    }
                }

                line = nextLine;
            }

            // this can't work with reader.ReadLine()
            ////if (!string.IsNullOrEmpty(line))
            ////{
            ////    throw new MarkalizeException("File does not seem to end with an empty line");
            ////}
        }

        private static string ReadLine(StreamReader reader, out string lineEnding)
        {
            lineEnding = null;
            var lineBuilder = new StringBuilder();
            char c1 = (char)reader.Peek();
            char c2;
            while (c1 != -1)
            {
                c1 = (char)reader.Read();

                if (c1 == '\r')
                {
                    c2 = (char)reader.Peek();
                    if (c2 == '\n')
                    {
                        // windows line ending
                        lineEnding = "\r\n";
                        break;
                    }
                    else
                    {
                        // won't-name-it line ending
                        lineEnding = "\r";
                        break;
                    }
                }
                else if (c1 == '\n')
                {
                    // unix line ending
                    lineEnding = "\n";
                    break;
                }
                else
                {
                    lineBuilder.Append(c1);
                }
            }

            return lineBuilder.ToString();
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
