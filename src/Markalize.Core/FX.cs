
namespace Markalize.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal static class FX
    {
        internal static void AppendIf(this StringBuilder sb, string value, bool condition)
        {
            if (condition)
            {
                sb.Append(value);
            }
        }
    }
}
