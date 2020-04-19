using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MAAV.Application.Exceptions;
using MAAV.Application.Extensions;
using MAAV.DataContracts;
using MAAV.Domain.Repositories;
using System.Linq;
using System.Collections.Generic;

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

        public async Task<DataContracts.Organisation> GetByOrganisationNameAsync(string organisationId)
        {
            var organisation = await repository.GetByAsync(o => o.Name == organisationId);
            return organisation?.ToContract();
        }

        public async Task<Organisation> RegisterAsync(OrganisationRegistration organisationContract)
        {
            if (await repository.ExistsByAsync(o => o.Id == organisationContract.Id))
            {
                throw new NameAlreadyUsedException(organisationContract.Name);
            }
            var organisationEntity = organisationContract.ToEntity();
            organisationEntity = await repository.AddAsync(organisationEntity);


            var user = GeneratePasswordAndAttachAdminRole(organisationContract.AdminUser, organisationContract.Id);
            user.CreatedAt = DateTime.Now;
            user = await userRepository.AddAsync(user);

            var administrationTeam = new Domain.Entities.Team
            {
                Name = "org-admin",
                Id = "org-admin",
                OrganisationId = organisationContract.Id,
                CreatedAt = DateTime.Now
            };

            administrationTeam.Users.Add(new Domain.Entities.TeamUser(user));

            administrationTeam = await teamRepository.AddAsync(administrationTeam);
            user.TeamsPermissions.Add(new Domain.Entities.TeamPermission
            {
                TeamId = administrationTeam.Id,
                IsOwner = true
            });
            user = await userRepository.UpdateAsync(user);

            return organisationEntity.ToContract();
        }

        private Domain.Entities.User GeneratePasswordAndAttachAdminRole(User userAdmin, string organisationId)
        {

            var passkeys = userAdmin.Password.Encrypt();
            var userEntity = userAdmin.ToEntity();

            userEntity.PasswordHash = Convert.ToBase64String(passkeys.PasswordHash);
            userEntity.PasswordSalt = Convert.ToBase64String(passkeys.PasswordSalt);
            userEntity.OrganisationId = organisationId;
            userEntity.OrganisationRoles = new []{ "admin" };
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
