using System;
using System.Threading.Tasks;

using Testcontainers.MsSql;

namespace ApplicationTests.Fixture {
    sealed class MsSQLDatabaseDependency : IDatabaseDependency, IDisposable {
        readonly TaskCompletionSource<bool> startedCompletionSource = new();

        private readonly MsSqlContainer msSqlContainer = new MsSqlBuilder().WithAutoRemove(true).Build();

        public MsSQLDatabaseDependency() {
            msSqlContainer.StartAsync().ContinueWith(ContainerStarted);
        }

        private void ContainerStarted(Task task) {
            startedCompletionSource.SetResult(true);
        }

        public void Dispose() {
            msSqlContainer.DisposeAsync().AsTask().Wait();
        }

        public async Task Ready() {
            await startedCompletionSource.Task;
        }
    }
}
