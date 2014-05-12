using System;

namespace LTSASharp.Fsp.Processes
{
    internal class FspRefProcess : FspBaseProcess
    {
        public string Name { get; private set; }

        public FspRefProcess(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}