using Domain.ApiModels;

namespace Domain.Interfaces;

public interface IWeatherForecastService
{
    Task<WeatherForecast?> GetAsync(CancellationToken cancellationToken);
}
