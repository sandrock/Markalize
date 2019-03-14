
namespace Markalize.Core
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Exceptions for this library.
    /// </summary>
    [Serializable]
    public class MarkalizeException : Exception
    {
        public MarkalizeException()
        {
        }

        public MarkalizeException(string message)
            : base(message)
        {
        }

        public MarkalizeException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected MarkalizeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
