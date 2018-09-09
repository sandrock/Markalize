
namespace Markalize
{
    using System;

    /// <summary>
    /// Helps obtain a localizer from a resource.
    /// </summary>
    public interface ILocalizer
    {
        /// <summary>
        /// Localizes the given key.
        /// </summary>
        /// <param name="key">the resource key</param>
        /// <returns>a localized text</returns>
        string Localize(string key);
    }
}
