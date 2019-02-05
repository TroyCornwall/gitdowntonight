using System.Collections.Generic;
using System.Linq;
using gitdowntonight.models;

namespace gitdowntonight.Services
{
    public class ContributorSortingService: ISortContributors
    {
        public List<Contributor> Sort(List<Contributor> contributors)
        {
            return contributors.OrderByDescending(x => x.NumberOfContributions).ToList();
        }
    }

    public interface ISortContributors
    {
        List<Contributor> Sort(List<Contributor> contributors);
    }

}