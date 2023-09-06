using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracer.Processors;
using Tracer.Processors.TraceToFileProcessor;

namespace Tracer.Factories
{
    internal class ProcessorFactory : IProcessorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ProcessorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider=serviceProvider;
        }

        public ITraceProcessor GetTraceProcessor(TraceProcessorType processorType)
        {
            ITraceProcessor traceProcessor = processorType switch
            {
                TraceProcessorType.TraceToFile => _serviceProvider.GetRequiredService<ITraceToFileProcessor>(),

                _ => _serviceProvider.GetRequiredService<ITraceToFileProcessor>(),
            };

            return traceProcessor;
        }
    }
}
