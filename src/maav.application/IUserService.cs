using System.Collections.Generic;
using System.Threading.Tasks;
using MAAV.DataContracts;

namespace MAAV.Application
{
    public interface IUserService
    {
        Task<User> GetByUsernameAsync(string organisationId, string username);
        Task<User> AddAsync(string organisationId, User user);
        Task<User> UpdateAsync(string organisationId, User user, bool sameAuthUser = false);
        Task DeleteAsync(string organisationId, string username);
        Task RemoveUserToTeamAsync(string organisationId, string teamName, string username);
        Task<List<User>> LoadAllUsers(string organisationId);
        Task<Authentication> AuthenticateAsync(string organisationId, string username, string password);
        Task SetRolesAsync(string organisationId, string teamId, string username, TeamPermission permission);
        Task<bool> IsOwner(string orgId, string teamId, string username);
        Task ResetPassword(string organisationId, string username, string password);
    }
}