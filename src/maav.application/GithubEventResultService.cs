using MAAV.Application.Extensions;
using MAAV.DataContracts.GitHub;
using MAAV.Domain.Repositories;
using System.Threading.Tasks;

namespace MAAV.Application
{
    public class GithubEventResultService : IGithubEventResultService
    {
        private readonly IGithubEventResultRepository repository;
        private readonly IApplicationRepository appRepository;

        public GithubEventResultService(IGithubEventResultRepository repository, IApplicationRepository appRepository)
        {
            this.repository = repository;
            this.appRepository = appRepository;
        }

        public async Task<GithubEventResult> GetLastEventAsync(string commitId, string teamId, string appId, string organisationId)
        {
            var appLocated = await this.appRepository.GetByAsync(app => app.TeamCode == teamId && app.OrganisationId == organisationId && appId == app.Id);
            if (appLocated == null)
            {
                return null;
            }

            var @event = await this.repository.GetByAsync(x => x.PushCommit == commitId && x.TeamCode == teamId && x.OrganisationId == organisationId && x.AppId == appId && x.Status.Equals("ready"));
            return @event.ToContract();
        }
    }
}
