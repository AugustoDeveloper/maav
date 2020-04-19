using System;
using System.Linq.Expressions;
using MAAV.Domain.Entities;
using MAAV.Domain.Repositories;

namespace MAAV.Infrastructure.Repository.MongoDB
{
    public class OrganisationRepository : MongoRepository<Organisation>, IOrganisationRepository
    {
        protected override string collectionName => nameof(Organisation).ToLower();
        
        public OrganisationRepository(string connectionString) : base(connectionString)
        {
        }


        protected override Expression<Func<Organisation, bool>> RetrieveUpdateExpression(Organisation entity)
            => c => c.Name == entity.Name;
    }
}