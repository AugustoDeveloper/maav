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

        public TeamService(ITeamRepository repository, IOrganisationRepository organisationRepository)
        {
            this.repository = repository;
            this.organisationRepository = organisationRepository;
        }

        public async Task<Team> GetByTeamNameAsync(string organisationName, string teamName)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }

            var team = await repository.GetByAsync(t => t.Name == teamName && t.OrganisationName == organisationName);
            return team?.ToContract();
        }

        public async Task<Team> AddAsync(string organisationName, Team team)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }

            if (await this.repository.ExistsByAsync(t => t.OrganisationName == organisationName && t.Name == team.Name))
            {
                throw new NameAlreadyUsedException(team.Name);
            }

            var teamEntity = team.ToEntity();
            teamEntity.OrganisationName = organisationName;

            teamEntity = await this.repository.AddAsync(teamEntity);
            return teamEntity.ToContract();
        }

        public async Task<Team> UpdateAsync(string organisationName, Team team)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }

            var teamLocated = await this.repository.GetByAsync(t => t.OrganisationName == organisationName && t.Name == team.Name);
            if (teamLocated == null)
            {
                return null;
            }
            
            var teamEntity = team.ToEntity();
            teamEntity.Id = teamLocated.Id;
            teamEntity.OrganisationName = organisationName;

            teamEntity = await this.repository.UpdateAsync(teamEntity);
            return teamEntity.ToContract();
        }

        public async Task DeleteAsync(string organisationName, string teamName)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }
            
            await this.repository.DeleteAsync(t => t.Name == teamName && t.OrganisationName == organisationName);
        }

        public async Task<Team[]> LoadByOrganisationNameAsync(string organisationName)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }

            var teams = await repository.LoadByAsync(t => t.OrganisationName == organisationName);

            return teams?.ToContract().ToArray();
        }
    }
}