namespace LTSASharp.Fsp.Processes
{
    class FspRefProcess : FspLocalProcess
    {
        public string Name { get; private set; }

        public FspRefProcess(string name)
        {
            Name = name;
        }
    }
}
