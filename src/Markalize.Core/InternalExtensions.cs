
namespace Markalize.Core
{
    using Markalize.Internals;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal static class InternalExtensions
    {
        internal static bool ContainsAll(this IList<string> haystack, IList<string> needles, StringComparison stringComparison)
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

        /// <summary>
        /// Copy a part of a list into an array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="sourceOffset"></param>
        /// <param name="destinationOffset"></param>
        /// <param name="length"></param>
        internal static void CopyTo<T>(this IList<T> source, T[] destination, int sourceOffset, int destinationOffset, int length)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");
            if (source.Count < (sourceOffset + length))
                throw new ArgumentException("source is too small (" + source.Count + ") to read sourceOffset+length (" + (sourceOffset + length) + ")", "source");
            if (destination.Length < (destinationOffset + length))
                throw new ArgumentException("destination is too small (" + destination.Length + ") to read destinationOffset+length (" + (destinationOffset + length) + ")", "destination");

            int s = sourceOffset;
            int d = destinationOffset;
            for (int ops = 0; ops < length; ops++)
            {
                destination[d++] = source[s++];
            }
        }

        /// <summary>
        /// Checks whether the file matches all the given preferences.
        /// </summary>
        /// <param name="prefs"></param>
        /// <param name="file"></param>
        /// <returns></returns>
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
