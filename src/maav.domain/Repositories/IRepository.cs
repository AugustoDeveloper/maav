using MAAV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MAAV.Domain.Repositories
{
    public interface IRepository<TEntity> where TEntity : IEntity
    {        
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<TEntity> GetByAsync(Expression<Func<TEntity, bool>> expression);
        Task<List<TEntity>> LoadByAsync(Expression<Func<TEntity, bool>> expression);
        Task<List<TEntity>> LoadAllAsync();
        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> expression);
        Task<bool> ExistsByAsync(Expression<Func<TEntity, bool>> expression);        
    }
}