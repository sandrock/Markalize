
namespace Markalize.Internals
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implemented by classes that allow getting a localizer.
    /// </summary>
    public interface ILocalizerGetter
    {
        /// <summary>
        /// Obtain a localizer using some preferences.
        /// </summary>
        /// <param name="preferences">the preferences</param>
        /// <returns>a configured localizer</returns>
        ILocalizer GetLocalizer(IList<LocalizationPreference> preferences);
    }
}
