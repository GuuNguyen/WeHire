using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WeHire.Domain.Entities;
using WeHire.Entity.IRepositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WeHire.Entity.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly WeHireDBContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(WeHireDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task<TEntity> GetByIdAsync(
            object id, 
            Expression<Func<TEntity, object>>[]? includeProperties = null)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            DbSet<TEntity> query = _dbSet;

            if (includeProperties != null)
            {
                query = includeProperties.Aggregate(query, (current, include) => (DbSet<TEntity>)current.Include(include));
            }

            return await query.FindAsync(id);
        }

        public IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> expression = null, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, 
            Expression<Func<TEntity, object>>[]? includeProperties = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (expression != null)
            {
                query = query.Where(expression);
            }

            if (includeProperties != null && includeProperties.Length > 0)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query).AsQueryable();
            }

            return query;
        }

        public async Task<TEntity> GetFirstOrDefaultAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, object>>[]? includeProperties = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                query = includeProperties.Aggregate(query, (current, include) => current.Include(include));
            }
            return await query.FirstOrDefaultAsync();
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbSet.AsQueryable().AsNoTracking();
        }

        public async Task InsertAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task InsertRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            await _dbSet.AddRangeAsync(entities);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            _dbSet.Remove(entity);
        }

        
    }
}
