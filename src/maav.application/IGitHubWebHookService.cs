using MAAV.DataContracts.GitHub;
using System;
using System.Threading.Tasks;

namespace MAAV.Application
{
    public interface IGitHubWebHookService : IDisposable
    {
        Task EnqueueAsync(IGithubEvent @event);
    }
}
