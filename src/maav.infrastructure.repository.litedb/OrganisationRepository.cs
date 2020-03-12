using MAAV.Domain.Entities;
using MAAV.Domain.Repositories;
using System.Threading.Tasks;

namespace MAAV.Infrastructure.Repository.LiteDB
{
    public class OrganisationRepository : LiteDbRepository<Organisation>, IOrganisationRepository
    {
        public OrganisationRepository(string connectionString) : base(connectionString)
        {
        }

        public override Task<bool> DeleteAsync(Organisation entity)
            => ExecuteAsync((r) => r.Delete<Organisation>(entity.Name));
    }
}