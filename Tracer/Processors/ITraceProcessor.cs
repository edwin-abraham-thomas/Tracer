using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracer.Models;

namespace Tracer.Processors;

internal interface ITraceProcessor
{
    Task ProcessRequestAsync(HttpContext httpContext);
    Task ProcessResponseAsync(HttpContext httpContext);
    Task ProcessUnhandledException(HttpContext httpContext, Exception exception);
    void AddTraceEvent(TraceEvent traceEvent);
}
