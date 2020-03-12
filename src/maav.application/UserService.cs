using System;
using System.Threading.Tasks;
using MAAV.DataContracts;
using MAAV.Domain.Repositories;
using MAAV.Application.Extensions;
using MAAV.Application.Exceptions;
using System.Collections.Generic;
using System.Linq;
using MAAV.Application.Validation;

namespace MAAV.Application
{
    public partial class UserService : IUserService
    {
        private readonly ITeamRepository teamRepository;
        private readonly IOrganisationRepository organisationRepository;
        private readonly IUserRepository repository;

        public UserService(IUserRepository userRepository, IOrganisationRepository organisationRepository, ITeamRepository teamRepository = null)
        {
            this.teamRepository = teamRepository;
            this.organisationRepository = organisationRepository;
            this.repository = userRepository;
        }

        public async Task<User> AddAsync(string organisationName, User user)
        {
            if (!(await new UserValidator().ValidateAsync(user)).IsValid)
            {
                throw new ArgumentException($"The user information is invalid");
            }

            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }

            if (await this.repository.ExistsByAsync(u => u.Username == user.Username && u.OrganisationName == organisationName))
            {
                throw new NameAlreadyUsedException(user.Username);
            }

            var userEntity = user.ToEntity();
            userEntity.OrganisationName = organisationName;
            var passkeys = user.Password.Encrypt();

            userEntity.PasswordHash = passkeys.PasswordHash;
            userEntity.PasswordSalt = passkeys.PasswordSalt;
            userEntity.OrganisationRoles = new []{ "Administrator" };
            userEntity = await repository.AddAsync(userEntity);

            return userEntity.ToContract();
        }

        public async Task DeleteAsync(string organisationName, string username)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }

            await this.repository.DeleteAsync(t => t.Username == username && t.OrganisationName == organisationName);
        }

        public async Task<User> GetByUsernameAsync(string organisationName, string username)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }            
            var user = await repository.GetByAsync(t => t.Username == username && t.OrganisationName == organisationName);
            return user?.ToContract();
        }

        public async Task<User> UpdateAsync(string organisationName, User user)
        {
            if (!(await new UserValidator().ValidateAsync(user)).IsValid)
            {
                throw new ArgumentException($"The user information is invalid");
            }

            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }

            var userEntity = await this.repository.GetByAsync(u => u.OrganisationName == organisationName && u.Username == user.Username);
            if (userEntity == null)
            {
                return null;
            }

            userEntity.FirstName = user.FirstName;
            userEntity.LastName = user.LastName;

            return userEntity.ToContract();
        }

        public async Task AddUserToTeamAsync(string organisationName, string teamName, User user)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }

            var teamLocated = await this.teamRepository.GetByAsync(t => t.OrganisationName == organisationName && t.Name == teamName);
            if (teamLocated == null)
            {
                throw new ArgumentException($"The team {teamName} on organisation {organisationName} not exists");
            }

            var userEntity = await this.repository.GetByAsync(u => u.OrganisationName == organisationName && u.Username == user.Username);
            if (userEntity == null)
            {
                throw new ArgumentException($"The user {user.Username} on organisation {organisationName} not exists");
            }

            userEntity.TeamRoles.RemoveAll(tr => tr.TeamId == teamLocated.Id);

            userEntity.TeamRoles.AddRange(user.TeamRoles.ToEntity(utr => 
            {
                utr.TeamId = teamLocated.Id;
                utr.TeamName = teamName;
                return utr;
            }));

            await this.repository.UpdateAsync(userEntity);
        }

        public async Task RemoveUserToTeamAsync(string organisationName, string teamName, string username)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }

            var teamLocated = await this.teamRepository.GetByAsync(t => t.OrganisationName == organisationName && t.Name == teamName);
            if (teamLocated == null)
            {
                throw new ArgumentException($"The team {teamName} on organisation {organisationName} not exists");
            }

            var userEntity = await this.repository.GetByAsync(u => u.OrganisationName == organisationName && u.Username == username);
            if (userEntity == null)
            {
                throw new ArgumentException($"The user {username} on organisation {organisationName} not exists");
            }

            userEntity.TeamRoles.RemoveAll(tr => tr.TeamName == teamName);

            await this.repository.UpdateAsync(userEntity);
        }

        public async Task<List<User>> LoadAllUsers(string organisationName)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }

            var users = await repository.LoadByAsync(u => u.OrganisationName == organisationName);

            return users.Select(u => u.ToContract()).ToList();
        }

        public async Task<Authentication> AuthenticateAsync(string organisationName, string username, string password)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new UnauthorizedAccessException($"The organisation {organisationName} not exists");
            }

            var userEntity = await this.repository.GetByAsync(u => u.OrganisationName == organisationName && u.Username == username);
            if (userEntity == null)
            {
                throw new UnauthorizedAccessException($"The user {username} on organisation {organisationName} not exists");
            }

            if (!password.IsValid(userEntity.PasswordHash, userEntity.PasswordSalt))
            {
                throw new UnauthorizedAccessException("Invalid username\\password!");
            }

            var authResult = new Authentication
            {
                User = userEntity.ToContract(),
                OrganisationName = userEntity.OrganisationName,
                Roles = userEntity.OrganisationRoles
            };
            
            return authResult;
        }

        public async Task SetRolesAsync(string organisationName, string username, string[] roles)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }

            var userEntity = await this.repository.GetByAsync(u => u.OrganisationName == organisationName && u.Username == username);
            if (userEntity == null)
            {
                throw new ArgumentException($"The user {username} on organisation {organisationName} not exists");
            }

            userEntity.OrganisationRoles = roles;
            
            await this.repository.UpdateAsync(userEntity);
        }

        public async Task<string[]> LoadRolesAsync(string organisationName, string username)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Name == organisationName))
            {
                throw new ArgumentException($"The organisation {organisationName} not exists");
            }

            var userEntity = await this.repository.GetByAsync(u => u.OrganisationName == organisationName && u.Username == username);
            if (userEntity == null)
            {
                throw new ArgumentException($"The user {username} on organisation {organisationName} not exists");
            }

            return userEntity.OrganisationRoles;
        }
    }
}