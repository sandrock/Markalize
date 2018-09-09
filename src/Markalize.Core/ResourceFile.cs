
namespace Markalize.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    internal class ResourceFile
    {
        private string[] tags;
        private SortedDictionary<string, Entity> items = new SortedDictionary<string, Entity>();

        public ResourceFile()
        {
        }

        public ResourceFile(IEnumerable<string> tags)
        {
            this.tags = tags != null ? tags.ToArray() : new string[0];
        }

        public string[] Tags
        {
            get
            {
                var copy = new string[this.tags.Length];
                Array.Copy(this.tags, copy, copy.Length);
                return copy;
            }
        }

        internal bool TryLocalize(string key, out string value)
        {
            Entity entity;
            if (this.items.TryGetValue(key, out entity))
            {
                value = entity.Value;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public string[] Keys
        {
            get
            {
                var copy = new string[this.items.Count];
                int i = 0;
                foreach (var key in this.items.Keys)
                {
                    copy[i++] = key;
                }

                return copy;
            }
        }

        public CultureInfo Culture { get; internal set; }

        public Dictionary<string, string> Dimensions { get; internal set; }

        internal void Set(string key, int number, string genre, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The value cannot be empty", "key");

            Entity entity;
            if (!this.items.TryGetValue(key, out entity))
            {
                this.items.Add(key, entity = new Entity());
            }

            entity.Set(number, genre, value);
        }
    }
}