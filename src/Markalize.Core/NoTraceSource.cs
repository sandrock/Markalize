
namespace Markalize.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Copy of <see cref="TraceSource"/> that uses <see cref="Trace"/> internaly.
    /// </summary>
    internal sealed class NoTraceSource
    {
        private readonly string name;
        private readonly bool prependDateTime;

        public NoTraceSource()
        {
        }

        public NoTraceSource(string name)
            : this(name, false)
        {
        }

        public NoTraceSource(string name, bool prependDateTime)
        {
            this.prependDateTime = prependDateTime;
            if (!string.IsNullOrEmpty(name))
            {
                this.name = name + ": ";
            }
        }

        public void Write(StringBuilder sb)
        {
            if (sb == null)
                throw new ArgumentNullException("sb");

            if (sb.Length == 0)
                return;

            var message = sb.ToString().Trim();

            if (this.prependDateTime)
            {
                Trace.WriteLine(DateTime.UtcNow.ToString("o") + " " + this.name + message);
            }
            else
            {
                Trace.WriteLine(this.name + message);
            }
        }

        public void WriteLine(string message)
        {
            if (this.prependDateTime)
            {
                Trace.WriteLine(DateTime.UtcNow.ToString("o") + " " + this.name + message);
            }
            else
            {
                Trace.WriteLine(this.name + message);
            }
        }

        public void WriteLine(string format, params object[] args)
        {
            if (this.prependDateTime)
            {
                Trace.WriteLine(DateTime.UtcNow.ToString("o") + " " + this.name + string.Format(format, args));
            }
            else
            {
                Trace.WriteLine(this.name + string.Format(format, args));
            }
        }
    }
}
