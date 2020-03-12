using MAAV.Domain.Entities;
using MAAV.Domain.Repositories;
using System.Threading.Tasks;

namespace MAAV.Infrastructure.Repository.LiteDB
{
    public class TeamRepository : LiteDbRepository<Team>, ITeamRepository
    {
        public TeamRepository(string connectionString) : base(connectionString)
        {
        }

        public override Task<bool> DeleteAsync(Team entity)
            => ExecuteAsync((r) => r.Delete<Team>(entity.Name));
    }
}