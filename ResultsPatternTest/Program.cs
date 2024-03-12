using ResultsPatternTest;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddLog4Net();
builder.Logging.AddSeq();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<RequestTrackingMiddleWare>();

builder.Services.AddKeyedSingleton<RequestTrackingMiddleWare>("c");

builder.Services.AddKeyedScoped<IRepository, RepositoryImpl>("actual");
builder.Services.AddScoped<IRepository>(sp => {
    return new RepositoryCacheImpl(sp.GetRequiredKeyedService<IRepository>("actual"));
});

var app = builder.Build();

app.UseMiddleware<RequestTrackingMiddleWare>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

//ILogger logger = default;

//var x = 0;
//var y = 0;

////logger.LogDebug($"Value of {x} - {y} = {x - y}");
//logger.LogDebug("Value of {X} - {Y} = {Result}", x, y, x - y);

interface IRepository { }

class RepositoryImpl : IRepository {

}

struct X { }

class RepositoryCacheImpl : IRepository {
    public RepositoryCacheImpl(IRepository repository) {

    }
}


