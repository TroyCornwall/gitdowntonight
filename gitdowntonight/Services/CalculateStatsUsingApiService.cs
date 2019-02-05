using System;
using System.Collections.Generic;
using gitdowntonight.models;

namespace gitdowntonight.Services
{
    public class CalculateStatsUsingApiService : ICalcStatsForOrg
    {
        private readonly IGithubApi _githubApi;
        //Having this at class level and using DI means this gets initialized when the application starts
        private readonly List<Contributor> _contributors = new List<Contributor>();

        public CalculateStatsUsingApiService(IGithubApi githubApi)
        {
            _githubApi = githubApi;
        }


        public List<Contributor> CalculateStatsForOrg(string org)
        {
            //Firstly we need to get all the repos for the org
            var repos = _githubApi.GetReposForOrganization(org).Result;

            //We then need to get the stats for each of these
            foreach (var repo in repos)
            {
                var stats = _githubApi.GetStatsForRepo(org, repo.Name).Result;
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
            try
            {
                var contributor = _contributors.Find(x => x.Name.Equals(githubUsername));
                contributor.NumberOfContributions += contribution.Total;
            }
            catch (ArgumentNullException)
            {
                //The user wasn't in the list, so add them
                _contributors.Add(new Contributor
                {
                    Name = githubUsername,
                    NumberOfContributions = contribution.Total
                });
            }
        }
    }

    public interface ICalcStatsForOrg
    {
        List<Contributor> CalculateStatsForOrg(string org);
    }
}