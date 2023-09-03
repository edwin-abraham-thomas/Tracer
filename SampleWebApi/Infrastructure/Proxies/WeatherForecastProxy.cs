using Domain.Interfaces.Infrastructure;
using Entities;

namespace Infrastructure.Proxies;

public class WeatherForecastProxy : IWeatherForecastProxy
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public Task<WeatherForecast?> GetAsync(CancellationToken cancellationToken)
    {

        return Task.FromResult<WeatherForecast?>(new WeatherForecast
        {
            Date = DateTime.Now,
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        });
    }
}
