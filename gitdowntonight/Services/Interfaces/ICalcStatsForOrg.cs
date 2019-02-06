using System.Collections.Generic;
using gitdowntonight.Models;

namespace gitdowntonight.Services.Interfaces
{
    public interface ICalcStatsForOrg
    {
        List<Contribution> CalculateStatsForOrg(string org);
    }
}