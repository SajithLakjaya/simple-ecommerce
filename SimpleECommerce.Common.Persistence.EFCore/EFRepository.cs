using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SimpleECommerce.Common.Abstractions.Domain;
using SimpleECommerce.Common.Abstractions.Persistence.EfCore;
using SimpleECommerce.Common.Util;
using System.Linq.Expressions;

namespace SimpleECommerce.Common.Persistence.EFCore
{
    public class EFRepository<TEntity, TId> : IEfRepository<TEntity, TId>
        where TEntity : class, IHaveIdentity<TId>
    {
        #region Private Members

        private readonly DbContext _context;
        private readonly DbSet<TEntity> _entities;

        #endregion

        #region Constructors
          
        public EFRepository(DbContext context)
        {
            _context = context;
            _entities = _context.Set<TEntity>();
        }

        #endregion

        #region IWriteRepository Implementation

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(nameof(entity), entity);

            await _entities.AddAsync(entity, cancellationToken);
            return entity;
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(nameof(predicate), predicate);

            var entities = await _entities.Where(predicate).ToListAsync(cancellationToken);
            await DeleteRangeAsync(entities, cancellationToken);
        }

        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(nameof(entity), entity);

            _entities.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteByIdAsync(TId id, CancellationToken cancellationToken = default)
        {
            var item = await FindByIdAsync(id, cancellationToken);

            if (item == null)
            {
                return;
            }

            await DeleteAsync(item, cancellationToken);
        }

        public async Task DeleteRangeAsync(IReadOnlyList<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(nameof(entities), entities);

            foreach (var entity in entities)
            {
                await DeleteAsync(entity, cancellationToken);
            }
        }

        public async Task<TEntity> UpdateAsync(TEntity entity,
            CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(nameof(entity), entity);

            _entities.Update(entity);
            return await Task.FromResult(entity);
        }

        #endregion

        #region IReadRepository Implementation

        public async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _entities.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken = default)
        {
            return await _entities.FindAsync(id, cancellationToken);
        }

        public async Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(nameof(predicate), predicate);
            return await _entities.SingleAsync(predicate, cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _entities.ToListAsync(cancellationToken);
        }

        #endregion

        #region EFRepository Implementation

        public IEnumerable<TEntity> GetInclude(
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null)
        {
            IQueryable<TEntity> query = _entities;

            if (includes != null)
            {
                query = includes(query);
            }

            return query.AsEnumerable();
        }

        public IEnumerable<TEntity> GetInclude(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null,
            bool withTracking = true)
        {
            IQueryable<TEntity> query = _entities;

            if (includes != null)
            {
                query = includes(query);
            }

            query = query.Where(predicate);

            if (withTracking == false)
            {
                query = query.Where(predicate).AsNoTracking();
            }

            return query.AsEnumerable();
        }

        public async Task<IEnumerable<TEntity>> GetIncludeAsync(
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null)
        {
            IQueryable<TEntity> query = _entities;

            if (includes != null)
            {
                query = includes(query);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetIncludeAsync(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includes = null,
            bool withTracking = true)
        {
            IQueryable<TEntity> query = _entities;

            if (includes != null)
            {
                query = includes(query);
            }

            query = query.Where(predicate);

            if (withTracking == false)
            {
                query = query.Where(predicate).AsNoTracking();
            }

            return await query.ToListAsync();
        }

        #endregion

        #region IDisposal Implementation

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}