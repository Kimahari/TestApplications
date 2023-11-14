using System.Diagnostics;
using System.Reflection;

namespace ResultsPatternTest;

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