using System;
using System.Linq.Expressions;
using MAAV.Domain.Entities;
using MAAV.Domain.Repositories;

namespace MAAV.Infrastructure.Repository.MongoDB
{
    public class TeamRepository : MongoRepository<Team>, ITeamRepository
    {
        protected override string collectionName => nameof(Team).ToLower();
        
        public TeamRepository(string connectionString) : base(connectionString)
        {
        }


        protected override Expression<Func<Team, bool>> RetrieveUpdateExpression(Team entity)
            => c => c.TeamCode == entity.TeamCode;
    }
}