using System.Collections.Generic;
using System.Linq;
using gitdowntonight.Models;
using gitdowntonight.Services.Interfaces;

namespace gitdowntonight.Services.Impl
{
    public class ContributorSortingService: ISortContributors
    {
        /// <summary>
        /// Sorts a list of contributions in a descending list of how many contributions they made
        /// </summary>
        /// <param name="contributions">The list to sort</param>
        /// <returns>The sorted list</returns>
        public List<Contribution> Sort(List<Contribution> contributions)
        {
            return contributions.OrderByDescending(x => x.NumberOfContributions).ToList();
        }
    }
}