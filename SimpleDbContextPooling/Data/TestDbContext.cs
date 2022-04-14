namespace SimpleDbContextPooling.Data;

class TestDbContext : PoolableDbContext {

    public TestDbContext(DbContextOptions options) : base(options) {

    }

    protected TestDbContext() {

    }

    public DbSet<MyClass> Classes { get; set; }
}
