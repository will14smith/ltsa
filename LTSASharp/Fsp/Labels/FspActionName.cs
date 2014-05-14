using System;
using LTSASharp.Fsp.Expressions;

namespace LTSASharp.Fsp.Labels
{
    class FspActionName : IFspActionLabel
    {
        public string Name { get; private set; }

        public FspActionName(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public void Expand(FspExpressionEnvironment env, Action<IFspActionLabel> action)
        {
            action(this);
        }
    }
}
