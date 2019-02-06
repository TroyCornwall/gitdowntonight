using System;
using System.Runtime.Serialization;

namespace gitdowntonight.Exceptions
{
    public class GithubOrgNotFoundException : GithubApiException
    {

        protected GithubOrgNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public GithubOrgNotFoundException(string message) : base(message)
        {
        }

        public GithubOrgNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}