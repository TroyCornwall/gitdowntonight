using System.Collections.Generic;
using gitdowntonight.models;

public interface ICalcStatsForOrg
{
    List<Contribution> CalculateStatsForOrg(string org);
}