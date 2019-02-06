using System;
using gitdowntonight.models;
using gitdowntonight.Services;
using Microsoft.Extensions.Options;
using Serilog;

namespace gitdowntonight
{
    public class  PollingStatsForOrganizationService : IMonitorOrganizationStats
    {
        private readonly ICalcStatsForOrg _calcStatsService;
        private readonly ISortContributors _sortContributorsService;
        private readonly IHandleResults _handleResultService;
        private readonly MyOptions _options;

        public PollingStatsForOrganizationService(ICalcStatsForOrg calcStatsService,
            ISortContributors sortContributorsService, IHandleResults handleResultService,
            IOptionsMonitor<MyOptions> optionsMonitor)
        {
            _calcStatsService = calcStatsService;
            _sortContributorsService = sortContributorsService;
            _handleResultService = handleResultService;
            _options = optionsMonitor.CurrentValue;
        }

        /// <summary>
        ///  This handles our MVP solution.
        /// </summary>
        public void Run()
        {
            bool running = true;
            while (running)
            {
                try
                {
                    //Get Stats
                    var contributors = _calcStatsService.CalculateStatsForOrg(_options.Organization);
                    //Sort stats
                    contributors = _sortContributorsService.Sort(contributors);
                    //Print out -- Note this will move to a file by the end of this
                    _handleResultService.Handle(contributors);
                }
                catch (GithubUnauthorizedException e)
                {
                    //We cannot recover
                    running = false;
                    Log.Error($"Could not recover from exception {e.Message}");
                    Environment.ExitCode = -1;
                }
                catch (Exception e)
                {
                    Log.Error($"We hit an exception, shutting down\n{e.Message}");
                    Environment.Exit(-1);
                }
            }
            
        }
    }
}