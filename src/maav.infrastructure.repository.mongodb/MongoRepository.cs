using System;
using MAAV.Domain.Repositories;
using MAAV.Domain.Entities;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using MongoDB.Driver;

namespace MAAV.Infrastructure.Repository.MongoDB
{
	public abstract class MongoRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
	{
        private readonly MongoUrl mongoUrl;
        private readonly string connectionString;
		private readonly string databaseName;
		private Lazy<MongoClient> lazyClient;

		protected abstract string collectionName { get; }
		private IMongoDatabase Database => lazyClient.Value.GetDatabase(this.databaseName);
		private IMongoCollection<TEntity> Collection => Database.GetCollection<TEntity>(this.collectionName);

        protected MongoRepository(string connectionString)
        {
            this.mongoUrl = new MongoUrl(connectionString);
            this.connectionString = connectionString;
            this.databaseName = this.mongoUrl.DatabaseName;
            lazyClient = new Lazy<MongoClient>(() => new MongoClient(this.mongoUrl));
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await Collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            var result = await Collection.ReplaceOneAsync(RetrieveUpdateExpression(entity), entity);
            return entity;

        }
        protected abstract Expression<Func<TEntity, bool>> RetrieveUpdateExpression(TEntity entity);

        public async Task<TEntity> GetByAsync(Expression<Func<TEntity, bool>> expression)
        {
            var cursor = await Collection.FindAsync(expression);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<List<TEntity>> LoadByAsync(Expression<Func<TEntity, bool>> expression)
        {
            var cursor = await Collection.FindAsync(expression);
            return await cursor.ToListAsync();
        }

        public async Task<List<TEntity>> LoadAllAsync()
        {
            var cursor = await Collection.FindAsync(_ => true);
            return await cursor.ToListAsync();
        }
        public async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> expression)
        {
            var result = await Collection.DeleteOneAsync(expression);
            return result.DeletedCount > 0;
        }
        public async Task<bool> ExistsByAsync(Expression<Func<TEntity, bool>> expression)
        {
            var cursor = await Collection.FindAsync(expression);
            return await cursor.AnyAsync();
        }
    }
}
