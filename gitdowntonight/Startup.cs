using System;
using System.IO;
using System.Net;
using gitdowntonight.Services;
using gitdowntonight.Services.DBImpl;
using gitdowntonight.Services.Impl;
using gitdowntonight.Services.Interfaces;
using gitdowntonight.Services.MVPImpl;
using gitdowntonight.Services.PollingImpl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace gitdowntonight
{
    public class Startup
    {
        public static ServiceProvider ConfigureServices(IServiceCollection services, string[] args)
        {
            var options = ConfigureOptions(services, args);
            ConfigureLogging();

            //TODO: REMOVE THIS
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };


            //Setup DI
            services.AddTransient<IGithubApi, GithubApiService>();
            services.AddTransient<ICalcStatsForOrg, CalculateStatsUsingApiService>();
            services.AddTransient<ISortContributors, ContributorSortingService>();
            
//            services.AddTransient<IHandleResults, ResultPrintingService>();
            services.AddTransient<IHandleResults, TextDatabaseResultHandlingService>();
            
            
            //If you want to run the MVP version, uncomment the following line, and comment the one after
            //services.AddTransient<IMonitorOrganizationStats, RunOnceStatsForOrganizationService>();
            services.AddTransient<IMonitorOrganizationStats, PollingStatsForOrganizationService>();

            return services.BuildServiceProvider();
        }

        private static IConfigurationRoot ConfigureOptions(IServiceCollection services, string[] args)
        {
            //This means we first load from appsettings.json 
            //Then override with env vars if they exist
            //Then override with command line args if they exist
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);

            var config = configBuilder.Build();
            //Bind to object, add to DI
            services.Configure<MyOptions>(config);
            return config;
        }

        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                //This can be noisy - Turns off debugging logs for CalculateStatsUsingApiService
                .MinimumLevel.Override("gitdowntonight.Services.CalculateStatsUsingApiService", LogEventLevel.Information)
                .Enrich.FromLogContext()
                
                .WriteTo.Console(LogEventLevel.Debug)
                .CreateLogger();
        }
    }
}