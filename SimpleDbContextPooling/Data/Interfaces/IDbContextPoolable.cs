using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SimpleDbContextPooling.Data;

interface IDbContextPoolable : IResettableService, IDisposable, IAsyncDisposable {

    void SetLease(DbContextLease lease);


    Task SetLeaseAsync(DbContextLease lease, CancellationToken cancellationToken);


    void ClearLease();

    void SnapshotConfiguration();
}
