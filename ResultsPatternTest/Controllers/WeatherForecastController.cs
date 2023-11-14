using LanguageExt.Common;

using Microsoft.AspNetCore.Mvc;

namespace ResultsPatternTest.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase {
    private static readonly string[] Summaries = [
        "Freezing",
        "Bracing",
        "Chilly",
        "Cool",
        "Mild",
        "Warm",
        "Balmy",
        "Hot",
        "Sweltering",
        "Scorching"
    ];

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger) {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public ActionResult<IEnumerable<WeatherForecast>> Get() {
        Result<IEnumerable<WeatherForecast>> data = Enumerable.Range(1, 5).Select(index => new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(index)), Random.Shared.Next(-20, 55), Summaries[Random.Shared.Next(Summaries.Length)]))
        .ToArray();

        return data.ToServerResponse(this);
    }
}

public static class ResultExtensions {
    public static ActionResult<TResult> ToServerResponse<TResult>(this Result<TResult> result, ControllerBase controller) {
        return result.Match<ActionResult<TResult>>(suc => controller.Ok(suc), err => controller.BadRequest(err));
    }
}

//public record RequestFailure();

//public readonly struct Result<TValue, TError> {
//    private readonly TValue? value;
//    private readonly TError? error;
//    private readonly bool isError;

//    public Result(TValue value) {
//        this.value = value;
//        error = default;
//        isError = false;
//    }

//    public Result(TError error) {
//        value = default;
//        this.error = error;
//        isError = true;
//    }

//    public static implicit operator Result<TValue, TError>(TValue value) => new(value);
//    public static implicit operator Result<TValue, TError>(TError error) => new(error);

//    public bool IsError => isError;
//    public bool HasValue => !isError;

//    public TValue Value => value!;
//    public TError Error => error!;

//    public override string ToString() => HasValue ? $"Value: {Value}" : $"Error: {Error}";

//    public TResult Match<TResult>(Func<TValue, TResult> success, Func<TError, TResult> failure)
//        => IsError ? success(value!) : failure(error!);
//}
