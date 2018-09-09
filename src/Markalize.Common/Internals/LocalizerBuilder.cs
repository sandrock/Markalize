
namespace Markalize.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Helps create a <see cref="ILocalizer"/> using fluent methods.
    /// </summary>
    public class LocalizerBuilder
    {
        private readonly ILocalizerGetter getter;
        private readonly List<LocalizationPreference> preferences;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizerBuilder"/> class.
        /// Helps create a <see cref="ILocalizer"/> using fluent methods.
        /// </summary>
        /// <param name="getter">the getter</param>
        public LocalizerBuilder(ILocalizerGetter getter)
        {
            if (getter == null)
                throw new ArgumentNullException("getter");

            this.getter = getter;
            this.preferences = new List<LocalizationPreference>();
        }

        public LocalizerBuilder ForCulture(params string[] cultureNames)
        {
            for (int i = 0; i < cultureNames.Length; i++)
            {
                var cultureName = cultureNames[i];

                int dashIndex;
                if (cultureName == null || cultureName.Length == 0)
                {
                }
                else if (cultureName.Length == 5 && (dashIndex = cultureName.IndexOf('-')) == 2)
                {
                    var pref1 = LocalizationPreference.ForLanguage(cultureName.Substring(0, 2));
                    this.preferences.Add(pref1);
                    var pref2 = LocalizationPreference.ForRegion(cultureName.Substring(3, 2));
                    this.preferences.Add(pref2);
                }
                else if (cultureName.Length == 2)
                {
                    var pref1 = LocalizationPreference.ForLanguage(cultureName);
                    this.preferences.Add(pref1);
                }
                else
                {
                    // NOTE: should we tolerate invalid language codes?
                    var pref1 = LocalizationPreference.ForLanguage(cultureName);
                    this.preferences.Add(pref1);
                }
            }

            return this;
        }

        public LocalizerBuilder ForCulture(params CultureInfo[] cultures)
        {
            var names = new string[cultures.Length];
            for (int i = 0; i < cultures.Length; i++)
            {
                names[i] = cultures[i].Name;
            }

            return this.ForCulture(names);
        }

        public LocalizerBuilder WithTag(params string[] tags)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                var tag = tags[i];
                if (string.IsNullOrEmpty(tag))
                {
                }
                else
                {
                    var pref1 = LocalizationPreference.Tag(tag);
                    this.preferences.Add(pref1);
                }
            }

            return this;
        }

        public LocalizerBuilder WithDimension(string dimension, params string[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                var value = values[i];
                if (string.IsNullOrEmpty(value))
                {
                }
                else
                {
                    var pref1 = LocalizationPreference.ForDimension(dimension, value);
                    this.preferences.Add(pref1);
                }
            }

            return this;
        }

        public ILocalizer GetLocalizer()
        {
            return this.getter.GetLocalizer(this.preferences);
        }

        public LocalizationPreference[] GetPreferences()
        {
            return this.preferences.ToArray();
        }

        public override string ToString()
        {
            return this.preferences.ToPreferencesString();
        }
    }
}
