using System.Collections.Generic;
using gitdowntonight.models;
using Microsoft.Extensions.Options;
using Serilog;

namespace gitdowntonight.Services
{
    public class ResultPrintingService : IHandleResults
    {
        private MyOptions _options;
        public ResultPrintingService(IOptionsMonitor<MyOptions> options)
        {
            _options = options.CurrentValue;
        }

        /// <summary>
        /// Prints out the results to serilog
        /// What Serilog does with this can be configured in <see cref="Startup"/>
        /// </summary>
        /// <param name="contributions"> The list of Contributions</param>
        public void Handle(List<Contribution> contributions)
        {
            //I ran this over the Github Org, and it had 700 contributors. 
            //So I added a setting, so you can control how many you return
            var limit = contributions.Count;
            if (_options.ResultLimit > 0)
            {
                limit = _options.ResultLimit;
            }
            for (var i = 0; i < limit;)
            {
                var name = contributions[i].Name;
                var numberOfContributions = contributions[i].NumberOfContributions;
                Log.Information("{0}: {1}: {2} contributions", ++i, name, numberOfContributions);
            }
        }
    }
}