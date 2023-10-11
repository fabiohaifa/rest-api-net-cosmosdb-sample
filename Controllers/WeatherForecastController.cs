using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace SampleCosmosDB.Controllers;

[ApiController]
[Route("/")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
        Env.Load();
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        return await getWheater();
        // return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        // {
        //     Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //     TemperatureC = Random.Shared.Next(-20, 55),
        //     Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        // })
        // .ToArray();
    }

    private async Task<IEnumerable<WeatherForecast>> getWheater() {

        var connectionString = Environment.GetEnvironmentVariable("COSMOSDB_CONNECTION_STRING");

        using CosmosClient client = new(
            connectionString: connectionString!
        );

        var container = client.GetContainer("cosmos","first");

        // Query multiple items from container
        //using FeedIterator<WeatherForecast> feed = container.GetItemQueryIterator<WeatherForecast>(
        //    queryText: "SELECT * FROM first"
        //);

        // Execute query and return results as list
        var results = new List<WeatherForecast>();
        var iterator = container.GetItemQueryIterator<WeatherForecast>("SELECT * FROM first");

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response);
        }

        return results;

        // Iterate query result pages
        // while (feed.HasMoreResults)
        // {
        //     FeedResponse<WeatherForecast> response = await feed.ReadNextAsync();

        //     // Iterate query results
        //     foreach (WeatherForecast item in response)
        //     {
        //         Console.WriteLine($"Found item:\t{item.name}");
        //     }
        // }
    }
}
