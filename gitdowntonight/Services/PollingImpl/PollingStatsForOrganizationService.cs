using System;
using System.Threading;
using gitdowntonight.Exceptions;
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
        private readonly ILogger _log = Log.ForContext<PollingStatsForOrganizationService>();

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
        ///  This handles our Polling & Storage solution.
        ///  To switch between either checkout Startup.cs
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
                    //Token is invalid/missing/revoked
                    //We cannot recover from this
                    running = false;
                    _log.Error($"{e.Message}\nCould not recover from exception");
                    //Using Enum values for consistency, and less magic numbers. 
                    Environment.ExitCode = (int) ExitCodes.UnauthorizedToken;
                }
                catch (GithubOrgNotFoundException e)
                {
                    //Could not organization
                    //We cannot recover from this
                    running = false;
                    _log.Error($"{e.Message}\nCould not recover from exception");
                    Environment.ExitCode = (int) ExitCodes.OrganizationNotFound;
                }
                catch(GithubRepoNotFoundException e)
                {
                    //Could not find a repo
                    //We can recover from this when we get a list of repos next time we run
                    _log.Debug(e.Message);
                    continue;
                }
                catch(GithubUnknownException e)
                {
                    //Unknown github issue, this could be due to polling too quickly
                    //We should be able to recover from this, but it will mess with the results of the this 
                    _log.Debug(e.Message);
                    continue;
                }               
                catch (Exception e)
                {
                    _log.Error($"We hit an exception, shutting down\n{e.Message}");
                    Environment.Exit((int)ExitCodes.UnknownError);
                }
                
                //Sleep until we are meant to run again
                var millisecondsToSleep = _options.SleepPeriod * 1000;
                Thread.Sleep(millisecondsToSleep);
            }
            
        }
    }
}