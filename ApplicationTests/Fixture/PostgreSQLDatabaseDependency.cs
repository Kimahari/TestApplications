using System;
using System.Threading.Tasks;

using Testcontainers.PostgreSql;

namespace ApplicationTests.Fixture; 
sealed class PostgreSQLDatabaseDependency : IDatabaseDependency, IDisposable {
    readonly TaskCompletionSource<bool> startedCompletionSource = new();

    private readonly PostgreSqlContainer container = new PostgreSqlBuilder().WithAutoRemove(true).Build();

    public PostgreSQLDatabaseDependency() {
        container.StartAsync().ContinueWith(ContainerStarted);
    }

    private void ContainerStarted(Task task) {
        startedCompletionSource.SetResult(true);
    }

    public void Dispose() {
        //msSqlContainer.Started -= MsSqlContainer_Started;
        container.DisposeAsync().AsTask().Wait();
    }

    public async Task Ready() {
        await startedCompletionSource.Task;
    }
}
