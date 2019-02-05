using System.IO;
using System.Net;
using Flurl.Http;
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
            services = ConfigureOptions(services, args);
            ConfigureLogging();

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            
            services.AddTransient<IGithubApi, GithubApiService>();
            services.AddTransient<ICalcStatsForOrg, CalculateStatsUsingApiService>();
            services.AddTransient<ISortContributors, ContributorSortingService>();
            services.AddTransient<IHandleResults, ResultPrintingService>();
            
            return services.BuildServiceProvider();
        }

        private static IServiceCollection ConfigureOptions(IServiceCollection services, string[] args)
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
            services.Configure<MyOptions>(config);
            return services;
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