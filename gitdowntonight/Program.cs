using System;
using gitdowntonight.Services;
using gitdowntonight.Services.Interfaces;
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
            
            //Get our main service
            var monitorOrganizationStats = serviceProvider.GetService<IMonitorOrganizationStats>();
            monitorOrganizationStats.Run();
        }
    }
}