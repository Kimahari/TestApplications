
using System.Diagnostics;
using System.Reflection;

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

public sealed class RequestTrackingMiddleWare(ILoggerFactory loggerFactory) : IMiddleware {
    private readonly ILogger logger = loggerFactory.CreateLogger(Assembly.GetExecutingAssembly().GetName().Name!);

    public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
        context.TraceIdentifier = Ulid.NewUlid().ToString();
        var start = Stopwatch.GetTimestamp();
        await next.Invoke(context);
        logger.LogInformation("Starting Request {Request} Completed with {Code} {RequestTime} {TrackerType}",
            context.Request.Path, context.Response.StatusCode, Stopwatch.GetElapsedTime(start), "Request");
    }
}