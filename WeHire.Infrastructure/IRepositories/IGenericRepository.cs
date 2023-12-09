using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Infrastructure.IRepositories
{
    public interface IGenericRepository<TEntity>
    {

        IQueryable<TEntity> GetAll();

        Task<TEntity> GetByIdAsync(
            object id,
            Expression<Func<TEntity, object>>[] includeProperties = null);

        IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> expression = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Expression<Func<TEntity, object>>[] includeProperties = null);

        Task<TEntity> GetFirstOrDefaultAsync(
          Expression<Func<TEntity, bool>> filter = null,
          Expression<Func<TEntity, object>>[] includeProperties = null);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

        Task InsertAsync(TEntity entity);

        Task InsertRangeAsync(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        Task DeleteAsync(int id);

    }
}
