using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracer.Models
{
    internal class HttpTrace
    {
        public string RequestPath { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
    }
}
