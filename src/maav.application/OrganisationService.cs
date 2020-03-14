using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MAAV.Application.Exceptions;
using MAAV.Application.Extensions;
using MAAV.DataContracts;
using MAAV.Domain.Repositories;
using System.Linq;

namespace MAAV.Application
{
    public class OrganisationService : IOrganisationService
    {
        private readonly IOrganisationRepository repository;
        private readonly ITeamRepository teamRepository;
        private readonly IUserRepository userRepository;

        public OrganisationService(IOrganisationRepository repository, ITeamRepository teamRepository = null, IUserRepository userRepository = null)
        {
            this.repository = repository;
            this.teamRepository = teamRepository;
            this.userRepository = userRepository;
        }

        public async Task DeleteByOrganisationNameAsync(string organisationName)
        {
            await repository.DeleteAsync(o => o.Name == organisationName);
        }

        public async Task<DataContracts.Organisation> GetByOrganisationNameAsync(string organisationName)
        {
            var organisation = await repository.GetByAsync(o => o.Name == organisationName);
            return organisation?.ToContract();
        }

        public async Task<Organisation> RegisterAsync(OrganisationRegistration organisationContract)
        {
            if (await repository.ExistsByAsync(o => o.Name == organisationContract.Name))
            {
                throw new NameAlreadyUsedException(organisationContract.Name);
            }
            var organisationEntity = organisationContract.ToEntity();
            organisationEntity = await repository.AddAsync(organisationEntity);

            var administrationTeam = new Domain.Entities.Team
            {
                Name = "org-admin",
                OrganisationName = organisationContract.Name,
            };

            var users = organisationContract.AdminUsers.Select(u => GeneratePasswordAndAttachAdminRole(u, organisationContract.Name)).ToArray();

            administrationTeam = await teamRepository.AddAsync(administrationTeam);
            foreach(var user in users)
            {
                user.TeamRoles.Add(new Domain.Entities.UserTeamRole
                {
                    TeamId = administrationTeam.Id,
                    TeamName = administrationTeam.Name,
                    Roles = new [] { "Administrator" }
                });
                await userRepository.AddAsync(user);
            }

            return organisationEntity.ToContract();
        }

        private Domain.Entities.User GeneratePasswordAndAttachAdminRole(User userAdmin, string organisationName)
        {

            var passkeys = userAdmin.Password.Encrypt();
            var userEntity = userAdmin.ToEntity();

            userEntity.PasswordHash = passkeys.PasswordHash;
            userEntity.PasswordSalt = passkeys.PasswordSalt;
            userEntity.OrganisationName = organisationName;
            userEntity.OrganisationRoles = new []{ "Administrator" };
            return userEntity;
        }

        public async Task<Organisation> UpdateAsync(Organisation organisationContract)
        {
            if (!await repository.ExistsByAsync(o => o.Name == organisationContract.Name))
            {
                return null;
            }

            return (await repository.UpdateAsync(organisationContract.ToEntity())).ToContract();
        }
    }
}
