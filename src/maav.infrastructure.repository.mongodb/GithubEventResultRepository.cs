using MAAV.Domain.Entities;
using MAAV.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MAAV.Infrastructure.Repository.MongoDB
{
    public class GithubEventResultRepository : MongoRepository<GithubEventResult>, IGithubEventResultRepository
    {
        protected override string collectionName => "github_events";

        public GithubEventResultRepository(string connectionString) : base(connectionString)
        {
        }


        protected override Expression<Func<GithubEventResult, bool>> RetrieveUpdateExpression(GithubEventResult entity)
            => e => e.Id == entity.Id;
    }
}
