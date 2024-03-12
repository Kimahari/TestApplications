using Microsoft.EntityFrameworkCore.Infrastructure;

using SimpleDbContextPooling.Data.Internal;

namespace SimpleDbContextPooling.Data.Interfaces;

interface IDbContextPoolable : IResettableService, IDisposable, IAsyncDisposable {

    void SetLease(DbContextLease lease);


    Task SetLeaseAsync(DbContextLease lease, CancellationToken cancellationToken);


    void ClearLease();

    void SnapshotConfiguration();
}
