// See https://aka.ms/new-console-template for more information
using NBomber.CSharp;
using System.Net.Http;
using System.Net.NetworkInformation;

Console.WriteLine("Hello, World!");

var httpClient = new HttpClient();

var scenario = Scenario.Create("hello_world_scenario", async context =>
{
    using var response = await httpClient.GetAsync("https://localhost:7280/WeatherForecast");

    return response.IsSuccessStatusCode
        ? Response.Ok()
        : Response.Fail();
})
.WithoutWarmUp()
.WithLoadSimulations(
    Simulation.Inject(rate: 100,
                      interval: TimeSpan.FromSeconds(1),
                      during: TimeSpan.FromSeconds(180))
);

NBomberRunner
    .RegisterScenarios(scenario)
    .Run();

Console.ReadKey();