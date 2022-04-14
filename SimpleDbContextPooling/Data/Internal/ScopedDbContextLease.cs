namespace SimpleDbContextPooling.Data;

sealed class ScopedDbContextLease<TContext> : IScopedDbContextLease<TContext>, IDisposable, IAsyncDisposable
   where TContext : DbContext {
   private DbContextLease _lease;

    public ScopedDbContextLease(IDbContextPool<TContext> contextPool) {
        _lease = new DbContextLease(contextPool, standalone: false);
        _lease.Context.SetLease(_lease);
    }

    public TContext Context
        => (TContext)_lease.Context;


    void IDisposable.Dispose()
        => _lease.Release();


    ValueTask IAsyncDisposable.DisposeAsync()
        => _lease.ReleaseAsync();
}
