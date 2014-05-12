namespace LTSASharp.Fsp.Composites
{
    class FspRefComposite : FspCompositeBody
    {
        public string Name { get; private set; }

        public FspRefComposite(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
