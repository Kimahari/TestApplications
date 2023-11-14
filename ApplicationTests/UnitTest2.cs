using System.Collections.Generic;
using System.Threading.Tasks;

using ApplicationTests.Fixture;

using Xunit;

namespace ApplicationTests {
    public class UnitTest2 : IClassFixture<DatabaseFixture> {
        class TestClass {

        }

        public UnitTest2(DatabaseFixture databaseFixture) {
            //
            //OR
            IEnumerable<TestClass> x = new List<TestClass>();
        }

        [Fact]
        public async Task Test1() {
            await Task.Delay(10000);
        }
    }
}
