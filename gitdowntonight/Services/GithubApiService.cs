using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
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


        public List<GithubContributerStats> GetStatsForRepo(string org, string repo)
        {
            Log.Debug($"Getting stats for {repo}");
            // GET /repos/:owner/:repo/stats/contributors
            try
            {
                var url = $"{_options.GithubBaseUrl}/repos/{org.ToLower()}/{repo.ToLower()}/stats/contributors";
                var client = new RestClient(url);
                client.Proxy = new WebProxy("http://localhost:8888", false);
                var request = new RestRequest(Method.GET);
                request.AddHeader("Accept", "application/vnd.github.v3+json");
                request.AddHeader("Authorization", $"Bearer {_options.GhAccessToken}");
                var result = client.Get<List<GithubContributerStats>>(request).Data;
                if (result.Count == 0)
                {
                    GetStatsForRepo(org, repo);
                }
                return result;
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to call Github Get Stats for repo");
                throw;
            }
        }

        public List<GithubRepo> GetReposForOrganization(string org)
        {
            Log.Debug("Querying repos");
            try
            {
                var url = $"{_options.GithubBaseUrl}/orgs/{org.ToLower()}/repos";
                var client = new RestClient(url);
//                client.Proxy = new WebProxy("http://localhost:8888", false);
                var request = new RestRequest(Method.GET);
                request.AddHeader("Accept", "application/vnd.github.v3+json");
                request.AddHeader("Authorization", $"Bearer {_options.GhAccessToken}");
                return client.Get<List<GithubRepo>>(request).Data;
            } catch (Exception e)
            {
                Log.Error(e, "Failed to call Github Get Stats for repo");
                throw;
            }
        }
    }

    public interface IGithubApi
    {
        List<GithubContributerStats> GetStatsForRepo(string org, string repo);
        List<GithubRepo> GetReposForOrganization(string org);
    }
}