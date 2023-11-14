using System.Reflection;

using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SimpleDbContextPooling.Data;

class PoolAbleDbContext : DbContext, IDbContextPoolable, IResettableService {
    private DbContextLease _lease = DbContextLease.InactiveLease;
    private int _leaseCount;

    protected PoolAbleDbContext() {
    }

    public PoolAbleDbContext(DbContextOptions options) : base(options) {
    }

    public void ClearLease() => _lease = DbContextLease.InactiveLease;

    public void SetLease(DbContextLease lease) {
        SetLeaseInternal(lease);

        var ss = typeof(DbContext);
        var @interface = ss.GetInterface(nameof(IDbContextPoolable));

        if (@interface is null) return;

        var m = @interface.GetMethod(nameof(IDbContextPoolable.SetLease));
        m?.Invoke(this, [null]);
    }

    public Task SetLeaseAsync(DbContextLease lease, CancellationToken cancellationToken) {
        SetLeaseInternal(lease);
        var ss = typeof(DbContext);
        var @interface = ss.GetInterface(nameof(IDbContextPoolable));

        if (@interface is null) return Task.CompletedTask;

        var m = @interface.GetMethod(nameof(IDbContextPoolable.SetLeaseAsync));
        var task = m?.Invoke(this, [null, cancellationToken]);

        if (task is Task task1) return task1;

        return Task.CompletedTask;
    }

    private void SetLeaseInternal(DbContextLease lease) {
        _lease = lease;
        ++_leaseCount;
    }

    public void SnapshotConfiguration() {
        var ss = typeof(DbContext);
        var @interface = ss.GetInterface(nameof(IDbContextPoolable));
        if (@interface is null) return;
        var m = @interface.GetMethod(nameof(IDbContextPoolable.SnapshotConfiguration));
        m?.Invoke(this, []);
    }

    public override void Dispose() {
        var lease = _lease;
        var contextShouldBeDisposed = lease.IsActive && _lease.IsStandalone;

        if (DisposeSync(lease.IsActive, contextShouldBeDisposed)) {
            base.Dispose();
        }

        lease.ContextDisposed();
        this.Database.CloseConnection();
    }

    public override async ValueTask DisposeAsync() {
        var lease = _lease;
        var contextShouldBeDisposed = lease.IsActive && _lease.IsStandalone;

        if (DisposeSync(lease.IsActive, contextShouldBeDisposed)) {
            await base.DisposeAsync();
        }

        await lease.ContextDisposedAsync();
        await this.Database.CloseConnectionAsync();
    }

    private bool DisposeSync(bool leaseActive, bool contextShouldBeDisposed) {
        var f = typeof(DbContext).GetField("_disposed", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);

        var _disposed = f?.GetValue(this);

        if (_disposed is bool disposed && disposed == false) {
            return true;
        }

        if (leaseActive is false && contextShouldBeDisposed) {
            f?.SetValue(this, true);
            _lease = DbContextLease.InactiveLease;
        }

        return false;
    }
}
