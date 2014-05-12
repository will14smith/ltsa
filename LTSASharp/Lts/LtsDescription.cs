using System.Collections.Generic;

namespace LTSASharp.Lts
{
    internal class LtsDescription
    {
        public LtsDescription()
        {
            Systems = new List<LtsSystem>();
        }

        public List<LtsSystem> Systems { get; private set; }
    }
}