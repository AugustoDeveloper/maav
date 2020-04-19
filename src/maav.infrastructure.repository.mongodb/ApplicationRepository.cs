using System;
using System.Linq.Expressions;
using MAAV.Domain.Entities;
using MAAV.Domain.Repositories;

namespace MAAV.Infrastructure.Repository.MongoDB
{
    public class ApplicationRepository : MongoRepository<Application>, IApplicationRepository
    {
        protected override string collectionName => nameof(Application).ToLower();
        
        public ApplicationRepository(string connectionString) : base(connectionString)
        {
        }


        protected override Expression<Func<Application, bool>> RetrieveUpdateExpression(Application entity)
            => c => c.Id == entity.Id;
    }
}