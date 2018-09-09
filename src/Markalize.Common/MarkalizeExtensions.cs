
namespace Markalize
{
    using Markalize.Internals;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class MarkalizeExtensions
    {
        public static string ToPreferencesString(this IEnumerable<LocalizationPreference> preferences)
        {
            var sb = new StringBuilder();
            var sep = string.Empty;
            foreach (var pref in preferences)
            {
                sb.Append(sep);

                if (pref.Dimension != null)
                {
                    sb.Append(pref.Dimension);
                    sb.Append("-");
                    sb.Append(pref.Value);
                }
                else
                {
                    sb.Append(pref.Value);
                }

                sep = ".";
            }

            return sb.ToString();
        }
    }
}
