using System.Collections.Generic;
using gitdowntonight.models;
using Serilog;

namespace gitdowntonight.Services
{
    public class ResultPrintingService : IHandleResults
    {
        public void Handle(List<Contributor> contributors)
        {
            for (var i = 0; i < contributors.Count;)
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