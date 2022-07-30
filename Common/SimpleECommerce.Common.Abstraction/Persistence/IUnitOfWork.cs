namespace SimpleECommerce.Common.Abstractions.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    }

    public interface IUnitOfWork<out TContext> : IUnitOfWork where TContext : class
    {
        TContext Context { get; }
    }
}