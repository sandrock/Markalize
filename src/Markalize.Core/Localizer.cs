
namespace Markalize.Core
{
    using System;
    using System.Collections.Generic;

    internal class Localizer : ILocalizer
    {
        private readonly ResourceSet resourceSet;
        private readonly List<ResourceFile> files = new List<ResourceFile>();

        public Localizer(ResourceSet resourceSet)
        {
            this.resourceSet = resourceSet;
        }

        public string Localize(string key)
        {
            string value;
            foreach (var file in this.files)
            {
                if (file.TryLocalize(key, out value))
                {
                    return value;
                }
            }

            return null;
        }

        internal void AddSources(ResourceFile[] sources)
        {
            foreach (var source in sources)
            {
                if (!this.files.Contains(source))
                {
                    this.files.Add(source);
                }
            }
        }
    }
}