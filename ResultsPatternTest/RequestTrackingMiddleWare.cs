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

//public static class LoggerExtensions {
//    //public static void LogInformation<T1, T2, T3, T4>(this ILogger logger, string message, T1 p1, T2 p2, T3 p3, T4 p4) {
//    //    if (!logger.IsEnabled(LogLevel.Information)) return;
//    //}

//    //public static int test() {
//    //    var p1 = 0;
//    //    var p2 = 0;

//    //    return p1 + p2;
//    //}
