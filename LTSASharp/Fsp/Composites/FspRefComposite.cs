using System.Collections.Generic;
using LTSASharp.Fsp.Expressions;

namespace LTSASharp.Fsp.Composites
{
    class FspRefComposite : FspCompositeBody
    {
        public string Name { get; private set; }
        public List<FspExpression> Arguments { get; private set; }


        public FspRefComposite(string name)
        {
            Name = name;
            Arguments = new List<FspExpression>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
