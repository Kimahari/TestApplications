using Microsoft.EntityFrameworkCore;

namespace SimpleDbEncryptionRepositoryLayer.EntityFramework; 

public class ForecastDbContext : DbContext {
    private readonly IDataProtector dataProtector;

    public ForecastDbContext(DbContextOptions<ForecastDbContext> options, IDataProtector dataProtector) : base(options) {
        this.dataProtector = dataProtector;
    }

    public DbSet<ForecastDbObject> WeatherForecasts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ForecastDbObject>().Property(p => p.SecureData).HasConversion(new EncryptedConverter(dataProtector));
    }
}