using System.Collections.Generic;
using gitdowntonight.models;

namespace gitdowntonight.Services
{
    public interface IHandleResults
    {
        void Handle(List<Contribution> contributions);
    }
}