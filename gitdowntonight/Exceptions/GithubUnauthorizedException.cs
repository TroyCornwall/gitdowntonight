using System;
using System.Runtime.Serialization;

namespace gitdowntonight.Exceptions
{
    public class GithubUnauthorizedException : GithubApiException
    {
        protected GithubUnauthorizedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public GithubUnauthorizedException(string message) : base(message)
        {
        }

        public GithubUnauthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}