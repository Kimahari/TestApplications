using System.Threading.Tasks;

namespace ApplicationTests.Fixture; 
public interface IDatabaseDependency {
    Task Ready();
}
