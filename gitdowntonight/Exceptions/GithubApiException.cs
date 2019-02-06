using System;
using System.Runtime.Serialization;

namespace gitdowntonight.Exceptions
{
    public class GithubApiException : Exception
    {
        protected GithubApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public GithubApiException(string message) : base(message)
        {
        }

        public GithubApiException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}