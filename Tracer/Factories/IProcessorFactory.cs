using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracer.Processors;

namespace Tracer.Factories
{
    public interface IProcessorFactory
    {
        ITraceProcessor GetTraceProcessor(TraceProcessorType processorType);
    }
}
