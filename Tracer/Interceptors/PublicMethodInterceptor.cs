using Castle.DynamicProxy;
using Newtonsoft.Json;
using Tracer.Factories;
using Tracer.Models;
using Tracer.Processors;

namespace Tracer.Interceptors;

internal class PublicMethodInterceptor : IInterceptor
{

    private readonly ITraceProcessor _traceProcessor;

    public PublicMethodInterceptor(IProcessorFactory processorFactory)
    {
        _traceProcessor = processorFactory.GetTraceProcessor(Processors.TraceProcessorType.TraceToFile);
    }
    public void Intercept(IInvocation invocation)
    {
        _traceProcessor.AddTraceEvent(new MethodEvent
        {
            ClassName = invocation.Method.DeclaringType.Name,
            MethodName = invocation.Method.Name,
            Arguments = invocation.Arguments,
            ReturnValue = invocation.ReturnValue,
            Type = MethodEventType.Invoke
        });

        invocation.Proceed();

        _traceProcessor.AddTraceEvent(new MethodEvent
        {
            ClassName = invocation.Method.DeclaringType.Name,
            MethodName = invocation.Method.Name,
            Arguments = invocation.Arguments,
            ReturnValue = invocation.ReturnValue,
            Type = MethodEventType.Return
        });
    }
}
