using System;
using System.Collections.Generic;
using System.Net;
using gitdowntonight.models;
using Microsoft.Extensions.Options;
using RestSharp;
using Serilog;

namespace gitdowntonight.Services
{
    public class GithubApiService : IGithubApi
    {
        private MyOptions _options;

        public GithubApiService(IOptionsMonitor<MyOptions> options)
        {
            _options = options.CurrentValue;
        }

        /// <summary>
        ///  Queries the Github API for to get the stats for a repo
        /// </summary>
        /// <param name="org"> Organization / Owner of the repo</param>
        /// <param name="repo"> The Repo to get the stats for </param>
        /// <returns>Returns a list of contributors, and how many contributions they made to the repo</returns>
        /// <exception cref="GithubRepoNotFoundException">Could not find the repo</exception>
        /// <exception cref="GithubUnauthorizedException">Token was missing/revoked/invalid</exception>
        /// <exception cref="GithubApiException">Handles other errors from the API</exception>
        public List<GithubContributerStats> GetStatsForRepo(string org, string repo)
        {
            Log.Debug($"Getting stats for {repo}");
            // GET /repos/:owner/:repo/stats/contributors

            var url = $"{_options.GithubBaseUrl}/repos/{org.ToLower()}/{repo.ToLower()}/stats/contributors";
            var client = CreateRestClient(url);

            var request = CreateRestRequest();
            var result = client.Get<List<GithubContributerStats>>(request);

            //For some reason, sometimes when you query a repo, it returns a 202 with no content !?!?
            //This maybe my internet?
            //The gihub.com/github/rails repo did it 22 times in a row before returning repo data 
            if (result.StatusCode == HttpStatusCode.Accepted)
            {
                return GetStatsForRepo(org, repo);
            }

            // 404 - Could not find repo - this shouldn't happen, as we got the list of repos from the last call. 
            // However someone could possibly delete a repo in between the two calls
            // This seems recoverable - next time this runs, we should be fine
            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                Log.Error($"Could not find repo {repo} in organization {org}");
                throw new GithubRepoNotFoundException($"Could not find repo {repo} in organization {org}");
            }

            // 401 - Unauthorized
            // :sadpanda: For this to happen when querying the repo but not the org, they probably revoked the token
            // while we are running. This isn't recoverable, we need a new token.
            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                Log.Error("Github says our token dead");
                throw new GithubUnauthorizedException("Github says our token dead");
            }

            //Catch all other errors.
            //These could be intermittent networking issues, or us being rate-limited or w.e. 
            //These should be recoverable 
            if (!result.IsSuccessful)
            {
                Log.Error($"Status Code {result.StatusCode}, for {org}/{repo}");
                throw new GithubApiException(
                    $"{result.StatusCode}: We hit an error querying the repo {repo} for organization {org}");
            }

            //Else we should have a correct response
            return result.Data;
        }

        /// <summary>
        /// Gets a list of repos for a organization
        /// </summary>
        /// <param name="org"> The organization to query about</param>
        /// <returns></returns>
        /// <exception cref="GithubOrgNotFoundException">Could not find the organization</exception>
        /// <exception cref="GithubUnauthorizedException">Token was missing/revoked/invalid</exception>
        /// <exception cref="GithubApiException">Handles other errors from the API</exception>
        public List<GithubRepo> GetReposForOrganization(string org)
        {
            Log.Debug("Querying repos");

            var url = $"{_options.GithubBaseUrl}/orgs/{org.ToLower()}/repos";
            var client = CreateRestClient(url);
            var request = CreateRestRequest();
            var result = client.Get<List<GithubRepo>>(request);

            // 404 - Could not find org
            // However someone could possibly delete a repo in between the two calls
            // This seems recoverable - next time this runs, we should be fine
            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                Log.Error($"Could not find organization {org}");
                throw new GithubOrgNotFoundException($"Could not find organization {org}");
            }

            // 401 - Unauthorized
            // Oh no - This is probably a user input error, or the token has been revoked
            // This isn't recoverable, we need a new token.
            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                Log.Error("Github says our token dead");
                throw new GithubUnauthorizedException("Github says our token dead");
            }

            //Catch all other errors.
            //These could be intermittent networking issues, or us being rate-limited or w.e. 
            //These should be recoverable 
            if (!result.IsSuccessful)
            {
                Log.Error($"Status Code {result.StatusCode}, for {org}");
                throw new GithubApiException(
                    $"{result.StatusCode}: We hit an error querying for organization {org}");
            }

            return result.Data;
        }

        private RestClient CreateRestClient(string url)
        {
            //Create client
            var client = new RestClient(url);

            //Use debugging proxy when running in development mode
            if (_options.AspnetcoreEnvironment.Equals("Development"))
            {
                client.Proxy = new WebProxy("http://localhost:8888", false);
            }

            return client;
        }

        private RestRequest CreateRestRequest()
        {
            var request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/vnd.github.v3+json");
            request.AddHeader("Authorization", $"Bearer {_options.GhAccessToken}");
            return request;
        }
    }

    public interface IGithubApi
    {
        List<GithubContributerStats> GetStatsForRepo(string org, string repo);
        List<GithubRepo> GetReposForOrganization(string org);
    }
}