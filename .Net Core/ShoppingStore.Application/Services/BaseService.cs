using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.Application.Contracts;
using ShoppingStore.Domain.Common.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ShoppingStore.Application.Services
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class
    {
        private DbSet<TEntity> _dbSet;
        protected IDataBaseContext _context { get; set; }
        public BaseService(IDataBaseContext context)
        {
            _context = context;
            _context.CheckArgumentIsNull(nameof(_context));
            _dbSet = _context.Set<TEntity>();
        }

        public async Task Commit() => await _context.SaveChangesAsync();
        public async Task CreateAsync(TEntity entity) => await _dbSet.AddAsync(entity);
        public void Update(TEntity entity) => _dbSet.Update(entity);
        public void Delete(TEntity entity) => _dbSet.Remove(entity);
        public async Task CreateRangeAsync(IEnumerable<TEntity> entities) => await _dbSet.AddRangeAsync(entities);
        public void UpdateRange(IEnumerable<TEntity> entities) => _dbSet.UpdateRange(entities);
        public void DeleteRange(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);

        public int CountEntities() => _dbSet.Count();
        public IEnumerable<TEntity> FindAll() => _dbSet.AsNoTracking().ToList();
        public async Task<TEntity> FindByIdAsync(Object id) => await _dbSet.FindAsync(id);
        public async Task<IEnumerable<TEntity>> FindAllAsync() => await _dbSet.AsNoTracking().ToListAsync();

        public async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter) => await _dbSet.FirstOrDefaultAsync(filter);

        public async Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            foreach (var include in includes)
                query = query.Include(include);

            if (orderBy != null)
                query = orderBy(query);

            if (filter != null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }
    }
}
