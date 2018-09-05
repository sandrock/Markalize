
namespace Markalize.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public class ResourceSet
    {
        private static readonly string[] fileExtensions = new string[] { "md", "txt", };
        private static readonly string[] fileExtensions1 = new string[] { "mz", };
        private readonly List<ResourceFile> resources = new List<ResourceFile>();
        private string[] keys;

        public ResourceSet()
        {
        }

        public string[] Keys
        {
            get
            {
                if (this.keys == null)
                {
                    var keys = new List<string>();
                    for (int i = 0; i < this.resources.Count; i++)
                    {
                        foreach (var key in this.resources[i].Keys)
                        {
                            if (!keys.Contains(key))
                            {
                                keys.Add(key);
                            }
                        }
                    }

                    this.keys = keys.ToArray();
                }

                var copy = new string[this.keys.Length];
                Array.Copy(this.keys, copy, this.keys.Length);
                return copy;
            }
        }

        public ILocalizer GetLocalizer(params string[] tags)
        {
            var localizer = new Localizer(this);
            for (int i = 0; i < tags.Length; i++)
            {
                var tagsCopy = new string[tags.Length - i];
                Array.Copy(tags, tagsCopy, tagsCopy.Length);

                var sources = this.resources.Where(x => ContainsAll(x.Tags, tagsCopy, StringComparison.OrdinalIgnoreCase)).ToArray();
                localizer.AddSources(sources);
            }

            return localizer;
        }

        private static bool ContainsAll(string[] haystack, string[] needles, StringComparison stringComparison)
        {
            var ok = new bool[needles.Length];
            for (int i = 0; i < haystack.Length; i++)
            {
                for (int j = 0; j < ok.Length; j++)
                {
                    if (!ok[j])
                    {
                        for (int k = 0; k < needles.Length; k++)
                        {
                            if (string.Equals(haystack[i], needles[k], stringComparison))
                            {
                                ok[j] = true;

                                if (Array.TrueForAll(ok, x => x))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        public void LoadFromAssembly(Assembly assembly, string location)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            if (string.IsNullOrEmpty(location))
                throw new ArgumentException("The value cannot be empty", "location");

            // assembly embedded files use a dot as folder separator
            var location1 = assembly.GetName().Name + "." + location.Replace("/", ".").Replace("\\", ".");

            var parser = new Parser();

            // find files that match the pattern
            var files = assembly.GetManifestResourceNames()
                .Where(x => x.StartsWith(location1, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            // determine the roles of each file
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];

                // decompose remaining of file name by splitting on dots 
                var parts = file.Substring(location1.Length).Split('.');
                var tags = new List<string>();
                for (int j = 0; j < parts.Length; j++)
                {
                    var part = parts[parts.Length - j - 1];

                    if (j == 0)
                    {
                        if (fileExtensions.Any(x => x.Equals(part, StringComparison.OrdinalIgnoreCase)))
                        {
                            // this .md/.txt file
                        }
                        else
                        {
                            // bad file extension
                            goto nextFile;
                        }
                    }
                    else if (j == 1)
                    {
                        if (fileExtensions1.Any(x => x.Equals(part, StringComparison.OrdinalIgnoreCase)))
                        {
                            // this .ml.*/.ml.* file
                        }
                        else
                        {
                            // bad file extension
                            goto nextFile;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(part.Trim()))
                        {
                            tags.Add(part.Trim());
                        }
                    }
                }

                // open and parse file
                var resource = new ResourceFile(tags);
                Stream stream;
                try
                {
                    stream = assembly.GetManifestResourceStream(file);
                }
                catch (Exception ex)
                {
                    throw new MarkalizeException("Failed to open file \"" + file + "\": " + ex.Message, ex);
                }

                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    try
                    {
                        parser.Parse(reader, resource);
                    }
                    catch (MarkalizeException ex)
                    {
                        throw new MarkalizeException("Failed to parse file \"" + file + "\": " + ex.Message, ex);
                    }
                    catch (Exception ex)
                    {
                        throw new MarkalizeException("Failed to parse file \"" + file + "\": " + ex.Message, ex);
                    }
                }

                this.resources.Add(resource);
                this.keys = null;

                nextFile:;
            }

        }
    }
}
