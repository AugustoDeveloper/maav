using MAAV.Domain.Entities;
using MAAV.Domain.Repositories;
using System.Threading.Tasks;

namespace MAAV.Infrastructure.Repository.LiteDB
{
    public class UserRepository : LiteDbRepository<User>, IUserRepository
    {
        public UserRepository(string connectionString) : base(connectionString)
        {
        }

        public override Task<bool> DeleteAsync(User entity)
            => ExecuteAsync((r) => r.Delete<User>(entity.Username));
    }
}