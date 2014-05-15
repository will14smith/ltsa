using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTSASharp.Fsp
{
    public abstract class FspBaseProcess
    {
        protected FspBaseProcess()
        {
            Parameters = new List<FspParameter>();
        }

        public string Name { get; set; }
        public List<FspParameter> Parameters { get; private set; }

    }
}
