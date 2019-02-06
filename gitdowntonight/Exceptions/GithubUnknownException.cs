using System;
using System.Runtime.Serialization;
using gitdowntonight.models;

namespace gitdowntonight.Exceptions
{
    public class GithubUnknownException : GithubApiException
    {
        protected GithubUnknownException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public GithubUnknownException(string message) : base(message)
        {
        }

        public GithubUnknownException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}