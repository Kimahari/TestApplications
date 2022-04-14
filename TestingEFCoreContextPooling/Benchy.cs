using BenchmarkDotNet.Attributes;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

class MyClass {
    public int Id { get; set; }
}

public class MyContext : DbContext {
    public MyContext(DbContextOptions options) : base(options) {
    }

    DbSet<MyClass> MyClasses { get; set; }
}

public class MyContext2 : DbContext {

    public MyContext2(DbContextOptions options) : base(options) {
    }

    DbSet<MyClass> MyClasses { get; set; }
}

[LongRunJob]
public class Benchy {
    private ServiceProvider sp1;
    private ServiceProvider sp2;

    public Benchy() {
        var serviceCollection1 = new ServiceCollection();
        serviceCollection1.AddDbContext<MyContext>(oo => oo.UseSqlite("Data Source=mydb1.db;"));
        this.sp1 = serviceCollection1.BuildServiceProvider();

        var serviceCollection2 = new ServiceCollection();
        serviceCollection2.AddDbContextPool<MyContext2>(oo => {
            oo.UseSqlite("Data Source=mydb2.db;");
        });
        this.sp2 = serviceCollection2.BuildServiceProvider();
    }


    [Benchmark()]
    public void GetDbContextWithoutPooling() {
        using var scope = sp1.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<MyContext>();
        var ss = context.Database.ProviderName;
    }

    [Benchmark(Baseline = true)]
    public void GetDbContextWithPooling() {
        using var scope = sp2.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<MyContext2>();
        var ss = context.Database.ProviderName;
    }
}