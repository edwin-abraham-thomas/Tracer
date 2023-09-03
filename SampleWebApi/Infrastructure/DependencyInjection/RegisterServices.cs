using Domain.Interfaces.Infrastructure;
using Infrastructure.Proxies;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class RegisterServices
{
    public static void RegisterInfrastructureServices(this IServiceCollection services)
    {
        services.AddTransient<IWeatherForecastProxy, WeatherForecastProxy>();
    }
}
