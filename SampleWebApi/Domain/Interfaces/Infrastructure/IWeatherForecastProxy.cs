using Entities;

namespace Domain.Interfaces.Infrastructure;

public interface IWeatherForecastProxy
{
    Task<WeatherForecast?> GetAsync(CancellationToken cancellationToken);
}
