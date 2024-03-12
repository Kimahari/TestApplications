using Microsoft.Extensions.DependencyInjection.Extensions;

using SimpleDbContextPooling.Data.Interfaces;
using SimpleDbContextPooling.Data.Internal;

namespace SimpleDbContextPooling.Data.Extensions;

static class DependencyInjection {
    public static IServiceCollection RegisterDbContextPooling<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> configure) where TContext : PoolAbleDbContext {
        var config = new DbContextOptionsBuilder();

        services.TryAddSingleton<IDbContextPool<TContext>>(sp => {
            configure(config);
            return new DbContextPool<TContext>(config.Options);
        });

        services.TryAddScoped(sp => {
            var lease = sp.GetRequiredService<IScopedDbContextLease<TContext>>();
            return lease.Context;
        });

        services.TryAddScoped<IScopedDbContextLease<TContext>, ScopedDbContextLease<TContext>>();

        return services;
    }
}
