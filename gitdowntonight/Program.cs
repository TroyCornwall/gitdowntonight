using System;
using gitdowntonight.models;
using gitdowntonight.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace gitdowntonight
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //setup our DI
            var serviceProvider = Startup.ConfigureServices(new ServiceCollection(), args);
            
            //Get our options which include the organisation and access token
            //These are set either by appsettings.json, env vars, or command line flags
            var options = serviceProvider.GetService<IOptionsMonitor<MyOptions>>().CurrentValue;
           
           //Get all our services
            var statCalculator = serviceProvider.GetService<ICalcStatsForOrg>();
            var sorter = serviceProvider.GetService <ISortContributors>();
            var resultHandler = serviceProvider.GetService<IHandleResults>();
            

           
            var contributors = statCalculator.CalculateStatsForOrg(options.Organization);
            contributors = sorter.Sort(contributors);
            resultHandler.Handle(contributors);

        }
    }
}