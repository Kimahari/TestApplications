using Microsoft.AspNetCore.Mvc;

using SimpleDbContextPooling.Data;

namespace SimpleDbContextPooling.Controllers; 
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase {
    private static readonly string[] Summaries =
    [
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
];

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly TestDbContext dbContext;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IServiceProvider serviceProvider) {
        _logger = logger;
        this.dbContext = serviceProvider.GetRequiredService<TestDbContext>();
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get() {
        System.Threading.Thread.Sleep(250);
        var data = this.dbContext.Classes.ToList();

        dbContext.Classes.Add(new MyClass { });

        dbContext.SaveChanges();

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast(DateTime.Now.AddDays(index), Random.Shared.Next(-20, 55), Summaries[Random.Shared.Next(Summaries.Length)]))
        .ToArray();
    }
}