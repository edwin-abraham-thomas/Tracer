using Domain.Interfaces;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.DependencyInjection;

public static class RegisterServices
{
    public static void RegisterDomainServices(this IServiceCollection services)
    {
        services.AddTransient<IWeatherForecastService, WeatherForecastService>();
    }
}
