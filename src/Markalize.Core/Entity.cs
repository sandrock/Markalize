
namespace Markalize.Core
{
    using System;
    using System.Collections.Generic;

    public class Entity
    {
        private string defaultValue;
        private Dictionary<int, Dictionary<string, string>> numbers;

        public string Value { get => this.defaultValue; }

        internal void Set(int number, string genre, string value)
        {
            if (this.defaultValue == null || (number == 0 && genre == null))
            {
                this.defaultValue = value;
            }

            if (number > 0 || genre != null)
            {
                if (this.numbers == null)
                {
                    this.numbers = new Dictionary<int, Dictionary<string, string>>();
                }
            }

            if (number > 0 || genre != null)
            {
                Dictionary<string, string> numberSet;
                if (!this.numbers.TryGetValue(number, out numberSet))
                {
                    this.numbers.Add(number, numberSet = new Dictionary<string, string>());
                }

                numberSet[genre ?? string.Empty] = value;
            }
        }
    }
}