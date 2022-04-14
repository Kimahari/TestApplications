using System.Diagnostics.CodeAnalysis;

namespace SimpleDbContextPooling.Data;

struct DbContextLease {
    private IDbContextPool? _contextPool;


    public bool IsStandalone { get; }


    public static DbContextLease InactiveLease { get; } = new();


    public DbContextLease(IDbContextPool contextPool, bool standalone) {
        _contextPool = contextPool;
        IsStandalone = standalone;

        var context = _contextPool.Rent();
        Context = context;
    }


    public IDbContextPoolable Context { get; private set; }


    public bool IsActive
        => _contextPool != null;


    public void ContextDisposed() {
        if (IsStandalone) {
            Release();
        }
    }


    public ValueTask ContextDisposedAsync()
        => IsStandalone ? ReleaseAsync() : default;

    public void Release() {
        if (Release(out var pool, out var context)) {
            pool.Return(context);
        }
    }


    public ValueTask ReleaseAsync()
        => Release(out var pool, out var context)
            ? pool.ReturnAsync(context)
            : default;

    private bool Release([NotNullWhen(true)] out IDbContextPool? pool, [NotNullWhen(true)] out IDbContextPoolable? context) {
        pool = _contextPool;
        context = Context;
        _contextPool = null;
        Context = null!;

        return pool != null;
    }
}
