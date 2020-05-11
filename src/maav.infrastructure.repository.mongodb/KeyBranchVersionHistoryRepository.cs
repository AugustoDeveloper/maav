using MAAV.Domain.Entities;
using MAAV.Domain.Repositories;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MAAV.Infrastructure.Repository.MongoDB
{
    public class KeyBranchVersionHistoryRepository : MongoRepository<KeyBranchVersionHistory>, IKeyBranchVersionHistoryRepository
    {
        public KeyBranchVersionHistoryRepository(string connectionString) : base(connectionString)
        {
        }

        protected override string collectionName => "version_history";

        protected override Expression<Func<KeyBranchVersionHistory, bool>> RetrieveUpdateExpression(KeyBranchVersionHistory entity)
            => c => c.Id == entity.Id;

        public override Task<KeyBranchVersionHistory> AddAsync(KeyBranchVersionHistory entity)
        {
            entity.VersionHistory.Where(x => string.IsNullOrWhiteSpace(x.Id)).ToList().ForEach(_ =>
            {
                _.Id = StringObjectIdGenerator.Instance.GenerateId(this, _).ToString();
                entity.LastHistoryId = _.Id;
            });

            return base.AddAsync(entity);
        }

        public override Task<KeyBranchVersionHistory> UpdateAsync(KeyBranchVersionHistory entity)
        {
            var lastHistory = entity.VersionHistory.FirstOrDefault(v => v.Id == entity.LastHistoryId);
            entity.VersionHistory.Where(x => string.IsNullOrWhiteSpace(x.Id)).ToList().ForEach(_ =>
            {
                _.Id = StringObjectIdGenerator.Instance.GenerateId(this, _).ToString();
                _.PreviousId = lastHistory?.Id;
                entity.LastHistoryId = _.Id;
            });

            return base.UpdateAsync(entity);
        }
    }
}
