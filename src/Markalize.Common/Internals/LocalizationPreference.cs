
namespace Markalize.Internals
{
    using System;

    /// <summary>
    /// A preference for localization.
    /// </summary>
    public class LocalizationPreference
    {
        private LocalizationPreference()
        {
        }

        /// <summary>
        /// Gets the dimension flag.
        /// </summary>
        public string Dimension { get; private set; }

        /// <summary>
        /// Get the dimension value.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Creates a <see cref="LocalizationPreference"/> for the language dimension.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static LocalizationPreference ForLanguage(string value)
        {
            // NOTE: should we tolerate unknown language codes?
            var pref = new LocalizationPreference();
            pref.Dimension = Constants.LanguageDimension;
            pref.Value = value;
            return pref;
        }

        /// <summary>
        /// Creates a <see cref="LocalizationPreference"/> for the region dimension.
        /// </summary>
        public static LocalizationPreference ForRegion(string value)
        {
            // NOTE: should we tolerate unknown region codes?
            var pref = new LocalizationPreference();
            pref.Dimension = Constants.RegionDimension;
            pref.Value = value;
            return pref;
        }

        /// <summary>
        /// Creates a <see cref="LocalizationPreference"/> for a tag.
        /// </summary>
        public static LocalizationPreference Tag(string value)
        {
            var pref = new LocalizationPreference();
            pref.Value = value;
            return pref;
        }

        /// <summary>
        /// Creates a <see cref="LocalizationPreference"/> for the specific dimension.
        /// </summary>
        public static LocalizationPreference ForDimension(string dimension, string value)
        {
            var pref = new LocalizationPreference();
            pref.Dimension = dimension;
            pref.Value = value;
            return pref;
        }
    }
}