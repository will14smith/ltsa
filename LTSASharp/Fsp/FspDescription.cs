using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LTSASharp.Fsp.Processes;

namespace LTSASharp.Fsp
{
    class FspDescription
    {
        public FspDescription()
        {
            Processes = new List<FspProcess>();
        }

        public List<FspProcess> Processes { get; private set; }

        public override string ToString()
        {
            return "{" + string.Join(", ", Processes.Select(x => x.ToString())) + "}";
        }
    }
}
