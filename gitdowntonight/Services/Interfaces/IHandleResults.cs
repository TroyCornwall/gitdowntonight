using System.Collections.Generic;
using gitdowntonight.Models;

namespace gitdowntonight.Services.Interfaces
{
    public interface IHandleResults
    {
        void Handle(List<Contribution> contributions);
    }
}