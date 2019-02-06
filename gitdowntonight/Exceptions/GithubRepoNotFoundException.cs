using System;
using System.Runtime.Serialization;

namespace gitdowntonight.models
{
    public class GithubRepoNotFoundException : GithubApiException
    {

        protected GithubRepoNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public GithubRepoNotFoundException(string message) : base(message)
        {
        }

        public GithubRepoNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}