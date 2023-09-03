using Microsoft.AspNetCore.Http;
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

        await processor.ProcessRequestAsync(context);

        // Call the next delegate/middleware in the pipeline.
        await _next(context);

        await processor.ProcessResponseAsync(context);
    }
}