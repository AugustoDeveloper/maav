using System;
using System.Threading.Tasks;
using MAAV.DataContracts;
using MAAV.Domain.Repositories;
using MAAV.Application.Extensions;
using MAAV.Application.Exceptions;
using System.Linq;

namespace MAAV.Application
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository repository;
        private readonly IOrganisationRepository organisationRepository;
        private readonly IUserRepository userRepository;

        public TeamService(ITeamRepository repository, IOrganisationRepository organisationRepository, IUserRepository userRepository)
        {
            this.repository = repository;
            this.organisationRepository = organisationRepository;
            this.userRepository = userRepository;
        }

        public async Task<Team> GetByTeamNameAsync(string organisationId, string teamId)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Id == organisationId))
            {
                throw new ArgumentException($"The organisation {organisationId} not exists");
            }

            var team = await repository.GetByAsync(t => t.TeamCode == teamId && t.OrganisationId == organisationId);
            return team?.ToContract();
        }

        public async Task<Team> AddAsync(string organisationId, Team team)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Id == organisationId))
            {
                throw new ArgumentException($"The organisation {organisationId} not exists");
            }

            if (await this.repository.ExistsByAsync(t => t.OrganisationId == organisationId && t.TeamCode == team.Id))
            {
                throw new NameAlreadyUsedException(team.Id);
            }

            var teamEntity = team.ToEntity();
            teamEntity.OrganisationId = organisationId;

            teamEntity = await this.repository.AddAsync(teamEntity);
            return teamEntity.ToContract();
        }

        public async Task<Team> UpdateAsync(string organisationId, Team team)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Id == organisationId))
            {
                throw new ArgumentException($"The organisation {organisationId} not exists");
            }

            var teamLocated = await this.repository.GetByAsync(t => t.OrganisationId == organisationId && t.TeamCode == team.Id);
            if (teamLocated == null)
            {
                return null;
            }
            
            var teamEntity = team.ToEntity();
            teamEntity.TeamCode = teamLocated.TeamCode;
            teamEntity.OrganisationId = organisationId;

            teamEntity = await this.repository.UpdateAsync(teamEntity);
            return teamEntity.ToContract();
        }

        public async Task DeleteAsync(string organisationId, string teamId)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Id == organisationId))
            {
                throw new ArgumentException($"The organisation {organisationId} not exists");
            }

            var teamLocated = await this.repository.GetByAsync(t => t.OrganisationId == organisationId && t.TeamCode == teamId);
            if (teamLocated == null)
            {
                return;
            }

            foreach (var user in teamLocated.Users)
            {
                var userEntity = await userRepository.GetByAsync(u => u.Id == user.Id);
                userEntity.TeamsPermissions.RemoveAll(p => p.TeamCode == teamId);
                await this.userRepository.UpdateAsync(userEntity);
            }

            await this.repository.DeleteAsync(t => t.TeamCode == teamId && t.OrganisationId == organisationId);
        }

        public async Task<Team[]> LoadByOrganisationIdAsync(string organisationId)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Id == organisationId))
            {
                throw new ArgumentException($"The organisation {organisationId} not exists");
            }

            var teams = await repository.LoadByAsync(t => t.OrganisationId == organisationId);

            return teams?.ToContract().ToArray();
        }
    }
}