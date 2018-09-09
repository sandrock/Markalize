
namespace Markalize.Core
{
    using Markalize.Internals;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    public sealed class ResourceSet : ILocalizerGetter
    {
        private static readonly NoTraceSource trace = new NoTraceSource(nameof(ResourceSet), true);
        private static readonly string[] fileExtensions = new string[] { "md", "txt", };
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

        public LocalizerBuilder MakeLocalizer()
        {
            return new LocalizerBuilder(this);
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

            {
                var sources = this.resources.Where(x => x.Tags != null || x.Tags.Contains("Default")).ToArray();
                localizer.AddSources(sources);
            }

            {
                var sources = this.resources.Where(x => x.Tags == null || x.Tags.Length == 0).ToArray();
                localizer.AddSources(sources);
            }

            return localizer;
        }

        public ILocalizer GetLocalizer(IList<LocalizationPreference> preferences)
        {
            var localizer = new Localizer(this);
            for (int i = 0; i < preferences.Count; i++)
            {
                var tagsCopy = new LocalizationPreference[preferences.Count - i];
                preferences.CopyTo(tagsCopy, 0, 0, tagsCopy.Length);

                var sources = this.resources.Where(x => tagsCopy.AllMatch(x)).ToArray();
                localizer.AddSources(sources);
            }

            {
                var sources = this.resources.Where(x => x.Tags != null || x.Tags.Contains("Default")).ToArray();
                localizer.AddSources(sources);
            }

            {
                var sources = this.resources.Where(x => x.Tags == null || x.Tags.Length == 0).ToArray();
                localizer.AddSources(sources);
            }

            return localizer;
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
            var traceMessage = new StringBuilder();

            // find files that match the pattern
            var files = assembly.GetManifestResourceNames()
                .Where(x => x.StartsWith(location1, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            // determine the roles of each file
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                bool isDefault = false;

                // decompose remaining of file name by splitting on dots 
                var parts = file.Substring(location1.Length).Split('.');
                var tags = new List<string>();
                var dimensions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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
                            traceMessage.Append("File ");
                            traceMessage.Append(file);
                            traceMessage.AppendLine(" has an unknown extension.");
                            goto nextFile;
                        }
                    }
                    else if ("Default".Equals(part, StringComparison.OrdinalIgnoreCase))
                    {
                        isDefault = true;
                    }
                    else
                    {
                        var dashIndex = part.IndexOf('-');
                        if (dashIndex > 0 && dashIndex < (part.Length - 1))
                        {
                            // part is Dimension+Value
                            dimensions.Add(part.Substring(0, dashIndex), part.Substring(dashIndex + 1));
                        }
                        else
                        {
                            // part is not Dimension+Value
                            tags.Add(part);
                            traceMessage.Append("File ");
                            traceMessage.Append(file);
                            traceMessage.AppendLine(" has a file name part that does not look like a `Dimension-Value`.");
                        }
                    }
                }

                // determine standard culture
                var resource = new ResourceFile(tags);
                resource.Dimensions = dimensions;
                if (dimensions.ContainsKey("L") && dimensions.ContainsKey("R"))
                {
                    try
                    {
                        var culture = new CultureInfo(dimensions["L"] + "-" + dimensions["R"]);
                        resource.Culture = culture;
                    }
                    catch (CultureNotFoundException)
                    {
                        resource.Culture = CultureInfo.InvariantCulture;
                    }
                }
                else if (dimensions.ContainsKey("L"))
                {
                    try
                    {
                        var culture = new CultureInfo(dimensions["L"]);
                        resource.Culture = culture;
                    }
                    catch (CultureNotFoundException)
                    {
                        resource.Culture = CultureInfo.InvariantCulture;
                    }
                }

                // open and parse file
                Stream stream;
                try
                {
                    stream = assembly.GetManifestResourceStream(file);
                }
                catch (Exception ex)
                {
                    trace.Write(traceMessage);
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
                        trace.Write(traceMessage);
                        throw new MarkalizeException("Failed to parse file \"" + file + "\": " + ex.Message, ex);
                    }
                    catch (Exception ex)
                    {
                        trace.Write(traceMessage);
                        throw new MarkalizeException("Failed to parse file \"" + file + "\": " + ex.Message, ex);
                    }
                }

                if (isDefault)
                {
                    this.resources.Insert(0, resource);
                }
                else
                {
                    this.resources.Add(resource);
                }

                this.keys = null;

                nextFile:;
            }

            trace.Write(traceMessage);
        }

        private static bool ContainsAll(IList<string> haystack, IList<string> needles, StringComparison stringComparison)
        {
            var ok = new bool[needles.Count];
            for (int i = 0; i < haystack.Count; i++)
            {
                for (int j = 0; j < ok.Length; j++)
                {
                    if (!ok[j])
                    {
                        for (int k = 0; k < needles.Count; k++)
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
    }
}
