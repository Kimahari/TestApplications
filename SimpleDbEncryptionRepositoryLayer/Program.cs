using SimpleDbEncryptionRepositoryLayer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var x = TheGenerator.TheGeneratedClass.TheGeneratedMethod();
Console.WriteLine(x);

var xx = new ForecastDbObject().Encrypt();
var xxxx = new ForecastDbObject2().Encrypt();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public interface IDataProtector {
    public string Protect(string data);
    public string Unprotect(string data);
    public TEntity ProtectObject<TEntity>(TEntity data);
    public TEntity UnProtectObject<TEntity>(TEntity data);
}

public class DataProtector : IDataProtector {
    public string Protect(string data) {
        return data;
    }

    public string Unprotect(string data) {
        return data;
    }

    public TEntity ProtectObject<TEntity>(TEntity data) {
        return data;
    }

    public TEntity UnProtectObject<TEntity>(TEntity data) {
        return data;
    }
}