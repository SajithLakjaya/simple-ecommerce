using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleECommerce.Common.Abstractions.Persistence;
using System.Data;

namespace SimpleECommerce.Common.Persistence.EFCore
{
    public class EfUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        private readonly ILogger<EfUnitOfWork<TContext>> _logger;

        public EfUnitOfWork(
            TContext context,
            ILogger<EfUnitOfWork<TContext>> logger)
        {
            DbContext = context;
            _logger = logger;
        }

        public TContext DbContext { get; }

        public DbSet<TEntity> Set<TEntity>()
            where TEntity : class
        {
            return DbContext.Set<TEntity>();
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            await DbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            await DbContext.Database.RollbackTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await DbContext.Database.CommitTransactionAsync(cancellationToken);
        } 
    }
}