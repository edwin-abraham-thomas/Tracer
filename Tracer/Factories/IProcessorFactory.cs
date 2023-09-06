using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracer.Processors;

namespace Tracer.Factories
{
    internal interface IProcessorFactory
    {
        ITraceProcessor GetTraceProcessor(TraceProcessorType processorType);
    }
}
