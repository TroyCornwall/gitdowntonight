using System;
using System.IO;
using System.Net;
using gitdowntonight.models;
using gitdowntonight.Services;
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


            if (options.GetValue<String>("AspnetcoreEnvironment").Equals("Development"))
            {
                //Disable cert validation - For debugging 
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            }


            //Setup DI
            services.AddTransient<IGithubApi, GithubApiService>();
            services.AddTransient<ICalcStatsForOrg, CalculateStatsUsingApiService>();
            services.AddTransient<ISortContributors, ContributorSortingService>();
            services.AddTransient<IHandleResults, ResultPrintingService>();
            services.AddTransient<IMonitorOrganizationStats, RunOnceStatsForOrganizationService>();

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
                .Enrich.FromLogContext()
                .WriteTo.Console(LogEventLevel.Information)
                .CreateLogger();
        }
    }
}