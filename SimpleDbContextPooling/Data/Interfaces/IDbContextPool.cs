namespace SimpleDbContextPooling.Data.Interfaces;

interface IDbContextPool {
    IDbContextPoolable Rent();

    void Return(IDbContextPoolable context);

    ValueTask ReturnAsync(IDbContextPoolable context, CancellationToken cancellationToken = default);
}

interface IDbContextPool<TDbContext> : IDbContextPool where TDbContext : DbContext {

}