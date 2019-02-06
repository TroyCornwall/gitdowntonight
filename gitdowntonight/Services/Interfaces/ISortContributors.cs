using System.Collections.Generic;
using gitdowntonight.models;

namespace gitdowntonight.Services
{
    public interface ISortContributors
    {
        List<Contribution> Sort(List<Contribution> contributions);
    }
}