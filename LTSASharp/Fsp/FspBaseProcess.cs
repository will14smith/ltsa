using System.Collections.Generic;
using LTSASharp.Fsp.Labels;

namespace LTSASharp.Fsp
{
    public abstract class FspBaseProcess
    {
        protected FspBaseProcess()
        {
            Parameters = new List<FspParameter>();

            Hiding = new List<IFspActionLabel>();
        }

        public string Name { get; set; }
        public List<FspParameter> Parameters { get; private set; }
        
        public List<IFspActionLabel> Hiding { get; private set; }
        public FspHidingMode HidingMode { get; set; } 

    }
}
