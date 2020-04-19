using MAAV.DataContracts.GitHub;
using System.Threading.Tasks;

namespace MAAV.Application
{
    public interface  IGithubEventResultService
    {
        Task<GithubEventResult> GetLastEventAsync(string commitId, string teamId, string appId, string orgId);
    }
}
