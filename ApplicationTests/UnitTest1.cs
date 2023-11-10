using System.Threading.Tasks;

using ApplicationTests.Fixture;

using Xunit;

namespace ApplicationTests {

    public class UnitTest1 : IClassFixture<DatabaseFixture> {
        public UnitTest1(DatabaseFixture databaseFixture) {
        }

        [Fact]
        public async Task Test1() {
            await Task.Delay(10000);
        }
    }
}
