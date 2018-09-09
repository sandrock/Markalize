
namespace Markalize.Internals
{
    using System;

    public class LocalizationPreference
    {
        private LocalizationPreference()
        {
        }

        public string Dimension { get; private set; }

        public string Value { get; private set; }

        internal static LocalizationPreference ForLanguage(string value)
        {
            // NOTE: should we tolerate unknown language codes?
            var pref = new LocalizationPreference();
            pref.Dimension = Constants.LanguageDimension;
            pref.Value = value;
            return pref;
        }

        internal static LocalizationPreference ForRegion(string value)
        {
            // NOTE: should we tolerate unknown region codes?
            var pref = new LocalizationPreference();
            pref.Dimension = Constants.RegionDimension;
            pref.Value = value;
            return pref;
        }

        internal static LocalizationPreference Tag(string value)
        {
            var pref = new LocalizationPreference();
            pref.Value = value;
            return pref;
        }

        internal static LocalizationPreference ForDimension(string dimension, string value)
        {
            var pref = new LocalizationPreference();
            pref.Dimension = dimension;
            pref.Value = value;
            return pref;
        }
    }
}