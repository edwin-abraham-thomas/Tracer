using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System.Globalization;
using Tracer.Factories;

namespace Tracer;

internal class TracerMiddleware
{
    private readonly RequestDelegate _next;

    public TracerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IProcessorFactory processorFactory)
    {
        var processor = processorFactory.GetTraceProcessor(Processors.TraceProcessorType.TraceToFile);

        try
        {
            HttpRequestRewindExtensions.EnableBuffering(context.Request);
            await processor.ProcessRequestAsync(context);

            await _next(context);

            await processor.ProcessResponseAsync(context);
        }
        catch (Exception ex)
        {
            await processor.ProcessUnhandledException(context, ex);
        }
        
    }
}