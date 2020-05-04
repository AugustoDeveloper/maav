using System;
using System.Threading.Tasks;
using MAAV.DataContracts;
using MAAV.Domain.Repositories;
using MAAV.Application.Extensions;
using MAAV.Application.Exceptions;
using System.Collections.Generic;
using System.Linq;
using MAAV.Application.Validation;
using System.Text;

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

        public async Task<User> AddAsync(string orgId, User user)
        {
            if (!(await new UserValidator().ValidateAsync(user)).IsValid)
            {
                throw new ArgumentException($"The user information is invalid");
            }

            if (!await organisationRepository.ExistsByAsync(o => o.Id == orgId))
            {
                throw new ArgumentException($"The organisation {orgId} not exists");
            }

            if (await this.repository.ExistsByAsync(u => u.Username == user.Username && u.OrganisationId == orgId))
            {
                throw new NameAlreadyUsedException(user.Username);
            }

            var userEntity = user.ToEntity();
            userEntity.OrganisationId = orgId;
            var passkeys = user.Password.Encrypt();

            userEntity.PasswordHash = Convert.ToBase64String(passkeys.PasswordHash);
            userEntity.PasswordSalt = Convert.ToBase64String(passkeys.PasswordSalt);
            userEntity.OrganisationRoles = user.Roles;
            userEntity = await repository.AddAsync(userEntity);

            return userEntity.ToContract();
        }

        public async Task DeleteAsync(string orgId, string username)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Id == orgId))
            {
                throw new ArgumentException($"The organisation {orgId} not exists");
            }

            var userEntity = await this.repository.GetByAsync(u => u.OrganisationId == orgId && u.Username == username);
            if(userEntity == null)
            {
                return;
            }

            foreach (var teamPermission in userEntity.TeamsPermissions)
            {
                var team = await this.teamRepository.GetByAsync(t => t.OrganisationId == orgId && t.TeamCode == teamPermission.TeamCode);
                team.Users.RemoveAll(u => userEntity.Id.Equals(u.Id));
                await this.teamRepository.UpdateAsync(team);
            }

            await this.repository.DeleteAsync(t => t.Username == username && t.OrganisationId == orgId);
        }

        public async Task<User> GetByUsernameAsync(string orgId, string username)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Id == orgId))
            {
                throw new ArgumentException($"The organisation {orgId} not exists");
            }            
            var user = await repository.GetByAsync(t => t.Username == username && t.OrganisationId == orgId);
            return user?.ToContract();
        }

        public async Task<User> UpdateAsync(string orgId, User user, bool sameAuthUser = false)
        {
            if (!(await new UserValidator().ValidateAsync(user)).IsValid)
            {
                throw new ArgumentException($"The user information is invalid");
            }

            if (!await organisationRepository.ExistsByAsync(o => o.Id == orgId))
            {
                throw new ArgumentException($"The organisation {orgId} not exists");
            }

            var userEntity = await this.repository.GetByAsync(u => u.OrganisationId == orgId && u.Username == user.Username);
            if (userEntity == null)
            {
                return null;
            }

            userEntity.FirstName = user.FirstName;
            userEntity.LastName = user.LastName;
            if(!sameAuthUser)
            {
                userEntity.OrganisationRoles = user.Roles;
            }

            this.repository.UpdateAsync(userEntity);

            return userEntity.ToContract();
        }


        public async Task RemoveUserToTeamAsync(string orgId, string teamId, string username)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Id == orgId))
            {
                throw new ArgumentException($"The organisation {orgId} not exists");
            }

            var teamLocated = await this.teamRepository.GetByAsync(t => t.OrganisationId == orgId && t.TeamCode == teamId);
            if (teamLocated == null)
            {
                throw new ArgumentException($"The team {teamId} on organisation {orgId} not exists");
            }

            var userEntity = await this.repository.GetByAsync(u => u.OrganisationId == orgId && u.Username == username);
            if (userEntity == null)
            {
                throw new ArgumentException($"The user {username} on organisation {orgId} not exists");
            }

            userEntity.TeamsPermissions.RemoveAll(tr => tr.TeamCode == teamId);
            teamLocated.Users.RemoveAll(u => userEntity.Username.Equals(u.Username));

            await this.teamRepository.UpdateAsync(teamLocated);
            await this.repository.UpdateAsync(userEntity);
        }

        public async Task<List<User>> LoadAllUsers(string orgId)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Id == orgId))
            {
                throw new ArgumentException($"The organisation {orgId} not exists");
            }

            var users = await repository.LoadByAsync(u => u.OrganisationId == orgId);

            return users.Select(u => u.ToContract()).ToList();
        }

        public async Task<Authentication> AuthenticateAsync(string orgId, string username, string password)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Id == orgId))
            {
                throw new UnauthorizedAccessException($"The organisation {orgId} not exists");
            }

            var userEntity = await this.repository.GetByAsync(u => u.OrganisationId == orgId && u.Username == username);
            if (userEntity == null)
            {
                throw new UnauthorizedAccessException($"The user {username} on organisation {orgId} not exists");
            }

            if (!password.IsValid(userEntity.PasswordHash, userEntity.PasswordSalt))
            {
                throw new UnauthorizedAccessException("Invalid username\\password!");
            }

            var authResult = new Authentication
            {
                User = userEntity.ToContract(),
                OrganisationId = userEntity.OrganisationId,
            };
            
            return authResult;
        }

        public async Task SetRolesAsync(string orgId, string teamId, string username, TeamPermission permission)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Id == orgId))
            {
                throw new ArgumentException($"The organisation {orgId} not exists");
            }

            var teamEntity = await this.teamRepository.GetByAsync(t =>t.OrganisationId == orgId && t.TeamCode == teamId);
            if (teamEntity == null)
            {
                throw new ArgumentException($"The team {teamId} on organisation {orgId} not exists");
            }

            var userEntity = await this.repository.GetByAsync(u => u.OrganisationId == orgId && u.Username == username);
            if (userEntity == null)
            {
                throw new ArgumentException($"The user {username} on organisation {orgId} not exists");
            }

            userEntity.TeamsPermissions.RemoveAll(p => p.TeamCode == teamId);
            userEntity.TeamsPermissions.Add(permission.ToEntity());
            teamEntity.Users.RemoveAll(u => userEntity.Username.Equals(u.Username));
            teamEntity.Users.Add(new Domain.Entities.TeamUser(userEntity));
 
            await this.teamRepository.UpdateAsync(teamEntity);
            await this.repository.UpdateAsync(userEntity);
        }

        public async Task<bool> IsOwner(string orgId, string teamId, string username)
        {
            var userEntity = await this.repository.GetByAsync(u => u.Username == username && u.OrganisationId == orgId);
            if (userEntity == null)
            {
                return false;
            }
            var permission = userEntity.TeamsPermissions.FirstOrDefault(p => p.TeamCode == teamId);
            if (permission == null)
            {
                return false;
            }

            return permission.IsOwner;
        }

        public async Task ResetPassword(string orgId, string username, string password)
        {
            if (!await organisationRepository.ExistsByAsync(o => o.Id == orgId))
            {
                throw new ArgumentException($"The organisation {orgId} not exists");
            }

            var userEntity = await this.repository.GetByAsync(u => u.OrganisationId == orgId && u.Username == username);
            if (userEntity == null)
            {
                throw new ArgumentException($"The user {username} on organisation {orgId} not exists");
            }

            var passkeys = password.Encrypt();

            userEntity.PasswordHash = Convert.ToBase64String(passkeys.PasswordHash);
            userEntity.PasswordSalt = Convert.ToBase64String(passkeys.PasswordSalt);
            await repository.UpdateAsync(userEntity);
        }
    }
}
