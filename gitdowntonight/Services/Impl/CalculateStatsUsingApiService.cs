using System;
using System.Collections.Generic;
using gitdowntonight.models;
using Serilog;

namespace gitdowntonight.Services
{
    public class CalculateStatsUsingApiService : ICalcStatsForOrg
    {
        private readonly IGithubApi _githubApi;
        private readonly ILogger _log = Log.ForContext<CalculateStatsUsingApiService>();
        //Having this at class level and using DI means this gets initialized when the application starts
        private readonly List<Contribution> _contributors = new List<Contribution>();

        public CalculateStatsUsingApiService(IGithubApi githubApi)
        {
            _githubApi = githubApi;
        }

        /// <summary>
        /// Calculate contributor stats for a Github organization
        /// </summary>
        /// <param name="org"> The Organization you want stats for </param>
        /// <returns> List of contributors, and how many contributions they made </returns>
        public List<Contribution> CalculateStatsForOrg(string org)
        {
            //Clear stats from last run
            _contributors.Clear();
            //Firstly we need to get all the repos for the org
            var repos = _githubApi.GetReposForOrganization(org);

            _log.Debug($"Got {repos.Count} repos");
            //We then need to get the stats for each of these
            foreach (var repo in repos)
            {
                var stats = _githubApi.GetStatsForRepo(org, repo.Name);
                //Now we have the stats for this repo we need to add them to a total for each user
                foreach (var contributorStat in stats)
                {
                    //Either update the count of someone who contributed, or add them to the list.
                    UpdateOrAddContribution(contributorStat);
                }
            }

            return _contributors;
        }

        

        private void UpdateOrAddContribution(GithubContributerStats contribution)
        {
            var githubUsername = contribution.Author.Login;

            //Check if we already have this user in our list of results
            var contributor = _contributors.Find(x => x.Name.Equals(githubUsername));
            if (contributor != null)
            {
                //Add to their contributions
                contributor.NumberOfContributions += contribution.Total;
            }
            else
            {
                //The user wasn't in the list, so add them
                _log.Debug($"Adding {githubUsername} to Contributors");
                _contributors.Add(new Contribution
                {
                    Name = githubUsername,
                    NumberOfContributions = contribution.Total
                });
            }
        }
    }
}