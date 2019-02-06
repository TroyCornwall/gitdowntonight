using System.Collections.Generic;
using gitdowntonight.Models;

namespace gitdowntonight.Services.Interfaces
{
    public interface ISortContributors
    {
        List<Contribution> Sort(List<Contribution> contributions);
    }
}