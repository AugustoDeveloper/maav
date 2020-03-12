using MAAV.Domain.Entities;
using MAAV.Domain.Repositories;
using System.Threading.Tasks;

namespace MAAV.Infrastructure.Repository.LiteDB
{
    public class ApplicationRepository : LiteDbRepository<Application>, IApplicationRepository
    {
        public ApplicationRepository(string connectionString) : base(connectionString)
        {
        }

        public override Task<bool> DeleteAsync(Application entity)
            => ExecuteAsync((r) => r.Delete<Application>(entity.Name));
    }
}