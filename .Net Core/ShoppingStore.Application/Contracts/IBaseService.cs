using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace ShoppingStore.Application.Contracts
{
    public interface IBaseService<TEntity> where TEntity : class
    {
        Task Commit();
        int CountEntities();
        void Update(TEntity entity);
        void Delete(TEntity entity);
        IEnumerable<TEntity> FindAll();
        Task CreateAsync(TEntity entity);
        Task<TEntity> FindByIdAsync(Object id);
        Task<IEnumerable<TEntity>> FindAllAsync();
        void UpdateRange(IEnumerable<TEntity> entities);
        void DeleteRange(IEnumerable<TEntity> entities);
        Task CreateRangeAsync(IEnumerable<TEntity> entities);
        Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter);
        Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includes);
    }
}
