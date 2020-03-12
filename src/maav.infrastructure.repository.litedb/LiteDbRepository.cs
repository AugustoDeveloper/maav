using System;
using MAAV.Domain.Entities;
using MAAV.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB;
using System.Linq.Expressions;

namespace MAAV.Infrastructure.Repository.LiteDB
{
    public abstract class LiteDbRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly string connectionString;
        private bool disposed;

        protected LiteDbRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public virtual Task<TEntity> AddAsync(TEntity entity)
        {
            return ExecuteAsync((r) =>
            {
                r.Insert(entity);

                return entity;
            });
        }

        public abstract Task<bool> DeleteAsync(TEntity entity);

        public virtual Task<TEntity> GetByAsync(Expression<Func<TEntity, bool>> expression)
            => ExecuteAsync((r) => r.Query<TEntity>().Where(expression).FirstOrDefault());

        public virtual Task<List<TEntity>> LoadAllAsync()
            => ExecuteAsync((r) => r.Query<TEntity>().ToList());
        public virtual Task<TEntity> UpdateAsync(TEntity entity)
            => ExecuteAsync((r) => (r.Update<TEntity>(entity) == false) ? null : entity);

        public virtual Task<bool> ExistsByAsync(Expression<Func<TEntity, bool>> expression)
            => ExecuteAsync((r) => r.Query<TEntity>().Where(expression).Exists());

        public Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> expression)
            =>  ExecuteAsync((r) => r.DeleteMany(expression) > 0);
        public virtual Task<List<TEntity>> LoadByAsync(Expression<Func<TEntity, bool>> expression)
            => ExecuteAsync((r) => r.Query<TEntity>().Where(expression).ToList());

        protected Task<TResult> ExecuteAsync<TResult>(Func<LiteRepository, TResult> func)
            => Task.Factory.StartNew(() =>
            {
                TResult result = default(TResult);
                using (var repository = new LiteRepository(connectionString))
                {
                    try
                    {
                        result = func(repository);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                return result;
            });


        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}
