using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Tracer.Factories;
using Tracer.Processors.TraceToFileProcessor;

namespace Tracer.Extentions;

public static class RegisterationExtention
{
    public static IApplicationBuilder UseTracerMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TracerMiddleware>();
    }

    public static void RegisterTracerMiddleware(this IServiceCollection services)
    {
        services.AddTransient<ITraceToFileProcessor, TraceToFileProcessor>();
        services.AddTransient<IProcessorFactory, ProcessorFactory>();
    }
}
