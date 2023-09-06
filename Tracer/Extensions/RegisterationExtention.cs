using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Tracer.Factories;
using Tracer.Processors.TraceToFileProcessor;

namespace Tracer.Extensions;

public static class RegistrationExtensions
{
    public static IApplicationBuilder UseTracerMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TracerMiddleware>();
    }

    public static void RegisterTracerMiddleware(this IServiceCollection services)
    {
        services.AddScoped<ITraceToFileProcessor, TraceToFileProcessor>();
        services.AddTransient<IProcessorFactory, ProcessorFactory>();
    }
}
