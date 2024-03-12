using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

using SimpleDbContextPooling.Data.Interfaces;

namespace SimpleDbContextPooling.Data.Internal;

class DbContextPool<TDbContext> : DisposableBase, IDbContextPool<TDbContext> where TDbContext : PoolAbleDbContext {
    public const int DefaultPoolSize = 1024;

    private readonly ConcurrentQueue<IDbContextPoolable> _pool = new();

    private readonly Func<PoolAbleDbContext> _activator;

    private int _maxSize;
    private int _count;

    public DbContextPool(DbContextOptions options) {
        _maxSize = options.FindExtension<CoreOptionsExtension>()?.MaxPoolSize ?? DefaultPoolSize;

        if (_maxSize <= 0) {
            throw new ArgumentOutOfRangeException(nameof(CoreOptionsExtension.MaxPoolSize), CoreStrings.InvalidPoolSize);
        }

        options.Freeze();

        _activator = CreateActivator(options);
    }

    private static Func<PoolAbleDbContext> CreateActivator(DbContextOptions options) {
        var constructors = typeof(TDbContext).GetTypeInfo().DeclaredConstructors.Where(c => !c.IsStatic && c.IsPublic).ToArray();

        if (constructors.Length == 1) {
            var parameters = constructors[0].GetParameters();
            if (parameters.Length == 1
                && (parameters[0].ParameterType == typeof(DbContextOptions)
                    || parameters[0].ParameterType == typeof(DbContextOptions<TDbContext>))) {
                return Expression.Lambda<Func<TDbContext>>(Expression.New(constructors[0], Expression.Constant(options))).Compile();
            }
        }

        throw new InvalidOperationException(CoreStrings.PoolingContextCtorError(typeof(TDbContext).ShortDisplayName()));
    }

    public virtual IDbContextPoolable Rent() {
        if (_pool.TryDequeue(out var context)) {
            Interlocked.Decrement(ref _count);

            return context;
        }

        context = _activator();

        context.SnapshotConfiguration();

        return context;
    }

    public virtual void Return(IDbContextPoolable context) {
        if (Interlocked.Increment(ref _count) <= _maxSize) {
            context.ResetState();

            _pool.Enqueue(context);
        } else {
            PooledReturn(context);
        }
    }

    public virtual async ValueTask ReturnAsync(IDbContextPoolable context, CancellationToken cancellationToken = default) {
        if (Interlocked.Increment(ref _count) <= _maxSize) {
            await context.ResetStateAsync(cancellationToken).ConfigureAwait(false);

            _pool.Enqueue(context);
        } else {
            PooledReturn(context);
        }
    }

    private void PooledReturn(IDbContextPoolable context) {
        Interlocked.Decrement(ref _count);

        context.ClearLease();
        context.Dispose();
    }

    protected override void DisposeManaged() {
        _maxSize = 0;
        while (_pool.TryDequeue(out var context)) {
            context.ClearLease();
            context.Dispose();
        }
    }

    protected override async Task DisposeManagedAsync() {
        _maxSize = 0;

        while (_pool.TryDequeue(out var context)) {
            context.ClearLease();
            await context.DisposeAsync().ConfigureAwait(false);
        }
    }
}
