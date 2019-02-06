using System;
using gitdowntonight.models;
using gitdowntonight.Services;
using Microsoft.Extensions.Options;
using Serilog;

namespace gitdowntonight.Services.MVPImpl
{
    public class RunOnceStatsForOrganizationService : IMonitorOrganizationStats
    {
        private readonly ICalcStatsForOrg _calcStatsService;
        private readonly ISortContributors _sortContributorsService;
        private readonly IHandleResults _handleResultService;
        private readonly MyOptions _options;
        private readonly ILogger _log = Log.ForContext<RunOnceStatsForOrganizationService>();

        public RunOnceStatsForOrganizationService(ICalcStatsForOrg calcStatsService,
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
            try
            {
                //Get Stats
                var contributors = _calcStatsService.CalculateStatsForOrg(_options.Organization);
                //Sort stats
                contributors = _sortContributorsService.Sort(contributors);
                //Print out -- Note this will move to a file by the end of this
                _handleResultService.Handle(contributors);
            }
            catch (Exception e)
            {
                _log.Error($"We hit an exception, shutting down\n{e.Message}");
                Environment.Exit(-1);
            }
        }
    }
}