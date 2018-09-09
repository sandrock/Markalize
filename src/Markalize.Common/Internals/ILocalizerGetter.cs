
namespace Markalize.Internals
{
    using System;
    using System.Collections.Generic;

    public interface ILocalizerGetter
    {
        ILocalizer GetLocalizer(IList<LocalizationPreference> preferences);
    }
}
