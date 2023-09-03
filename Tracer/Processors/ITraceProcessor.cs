using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracer.Processors;

public interface ITraceProcessor
{
    Task ProcessRequestAsync(HttpContext httpContext);
    Task ProcessResponseAsync(HttpContext httpContext);
}
