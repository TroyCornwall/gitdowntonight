using System;
using System.IO;
using System.Runtime.Serialization;

namespace gitdowntonight.Exceptions
{
    public class GitDownTonightIOException : IOException
    {
        public GitDownTonightIOException()
        {
        }

        protected GitDownTonightIOException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public GitDownTonightIOException(string message) : base(message)
        {
        }

        public GitDownTonightIOException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public GitDownTonightIOException(string message, int hresult) : base(message, hresult)
        {
        }
    }
}