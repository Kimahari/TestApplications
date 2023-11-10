using System.Threading.Tasks;

using Xunit;

namespace ApplicationTests.Fixture {
    public class DatabaseFixture : IAsyncLifetime {
        private readonly IDatabaseDependency dependency;

        public DatabaseFixture(IDatabaseDependency dependency) {
            this.dependency = dependency;
        }
        
        public Task DisposeAsync() {
            return Task.CompletedTask;
        }

        public async Task InitializeAsync() {
            await dependency.Ready();
        }
    }
}
