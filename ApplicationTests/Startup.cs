using ApplicationTests.Fixture;

using Microsoft.Extensions.DependencyInjection;

namespace ApplicationTests; 
internal class Startup {
    public void ConfigureServices(IServiceCollection services) {
#if DEBUG
        services.AddSingleton<IDatabaseDependency, MsSQLDatabaseDependency>();
#elif POSTGRESQL
        services.AddSingleton<IDatabaseDependency, PostgreSQLDatabaseDependency>();
#endif
    }
}
