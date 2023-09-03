using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System.Globalization;
using Tracer.Factories;

namespace Tracer;

public class TracerMiddleware
{
    private readonly RequestDelegate _next;

    public TracerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IProcessorFactory processorFactory)
    {
        var processor = processorFactory.GetTraceProcessor(Processors.TraceProcessorType.TraceToFile);

        HttpRequestRewindExtensions.EnableBuffering(context.Request);
        await processor.ProcessRequestAsync(context);

        try
        {
            await _next(context);

            await processor.ProcessResponseAsync(context);
        }
        catch (Exception ex)
        {

        }
        
    }
}