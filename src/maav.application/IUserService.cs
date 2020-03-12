using System.Collections.Generic;
using System.Threading.Tasks;
using MAAV.DataContracts;

namespace MAAV.Application
{
    public interface IUserService
    {
        Task<User> GetByUsernameAsync(string organisationName, string username);
        Task<User> AddAsync(string organisationName, User user);
        Task<User> UpdateAsync(string organisationName, User user);
        Task DeleteAsync(string organisationName, string username);
        Task AddUserToTeamAsync(string organisationName, string teamName, User user);
        Task RemoveUserToTeamAsync(string organisationName, string teamName, string username);
        Task<List<User>> LoadAllUsers(string organisationName);
        Task<Authentication> AuthenticateAsync(string organisationName, string username, string password);
        Task SetRolesAsync(string organisationName, string username, string[] roles);
        Task<string[]> LoadRolesAsync(string organisationName, string username);
    }
}