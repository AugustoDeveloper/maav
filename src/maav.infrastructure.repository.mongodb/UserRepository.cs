using System;
using System.Linq.Expressions;
using MAAV.Domain.Entities;
using MAAV.Domain.Repositories;

namespace MAAV.Infrastructure.Repository.MongoDB
{
    public class UserRepository : MongoRepository<User>, IUserRepository
    {
        protected override string collectionName => nameof(User).ToLower();
        
        public UserRepository(string connectionString, string databaseName) : base(connectionString, databaseName)
        {
        }


        protected override Expression<Func<User, bool>> RetrieveUpdateExpression(User entity)
            => u => u.Id == entity.Id;
    }
}