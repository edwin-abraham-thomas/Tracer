using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracer.Models;

internal class HttpTrace
{
    public string? RequestPath { get; set; }
    public object? RequestBody { get; set; }
    public object? ResponseBody { get; set; }
    public Int16 ResponseStatusCode { get; set; }
    public List<TraceEvent> TraceEvents { get; set; }
    public Error? ApplicationError { get; set; }
    public Error? TracerError { get; set; }
}

internal class TraceEvent
{

}

internal class MethodEvent : TraceEvent
{
    public string ClassName { get; set; }
    public string MethodName { get; set; }
    public MethodEventType Type { get; set; }
    public object[] Arguments { get; set; }
    public object ReturnValue { get; set; }
}

internal enum MethodEventType
{
    Invoke,
    Return
}