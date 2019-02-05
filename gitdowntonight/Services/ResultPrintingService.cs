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

        public void Handle(List<Contributor> contributors)
        {
            //I ran this over the Github Org, and it had 700 contributors. 
            //So I added a setting, so you can control how many you return
            var limit = contributors.Count;
            if (_options.ResultLimit > 0)
            {
                limit = _options.ResultLimit;
            }
            for (var i = 0; i < limit;)
            {
                var name = contributors[i].Name;
                var numberOfContributions = contributors[i].NumberOfContributions;
                Log.Information("{0}: {1}: {2} contributions", ++i, name, numberOfContributions);
            }
        }
    }

    public interface IHandleResults
    {
        void Handle(List<Contributor> contributors);
    }
}