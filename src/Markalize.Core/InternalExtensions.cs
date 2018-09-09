
namespace Markalize.Core
{
    using Markalize.Internals;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal static class InternalExtensions
    {
        internal static void CopyTo<T>(this IList<T> source, T[] destination, int sourceOffset, int destinationOffset, int length)
        {
            int s = sourceOffset;
            int d = destinationOffset;
            for (int ops = 0; ops < length; ops++)
            {
                destination[d++] = source[s++];
            }
        }

        internal static bool AllMatch(this IList<LocalizationPreference> prefs, ResourceFile file)
        {
            for (int i = 0; i < prefs.Count; i++)
            {
                bool match = false;
                var pref = prefs[i];
                if (pref.Dimension != null)
                {
                    string value;
                    if (file.Dimensions != null && file.Dimensions.TryGetValue(pref.Dimension, out value))
                    {
                        match = pref.Value.Equals(value, StringComparison.OrdinalIgnoreCase);
                    }
                }
                else
                {
                    if (file.Tags != null && file.Tags.Any(t => t.Equals(pref.Value, StringComparison.OrdinalIgnoreCase)))
                    {
                        match = true;
                    }
                }

                if (!match)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
