using System;
using System.Runtime.Serialization;
using gitdowntonight.models;

namespace gitdowntonight.Exceptions
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