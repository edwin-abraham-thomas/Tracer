using Domain.Interfaces.Infrastructure;
using Infrastructure.Proxies;
using Microsoft.Extensions.DependencyInjection;
using Tracer.Extensions.ServiceCollectionExtensions;

namespace Infrastructure.DependencyInjection;

public static class RegisterServices
{
    public static void RegisterInfrastructureServices(this IServiceCollection services)
    {
        services.AddTraceableTransient<IWeatherForecastProxy, WeatherForecastProxy>();
    }
}
