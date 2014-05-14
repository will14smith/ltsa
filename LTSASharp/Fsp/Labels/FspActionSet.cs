using System;
using System.Collections.Generic;
using System.Linq;
using LTSASharp.Fsp.Expressions;

namespace LTSASharp.Fsp.Labels
{
    class FspActionSet : IFspActionLabel
    {
        public FspActionSet()
        {
            Items = new List<IFspActionLabel>();
        }

        public List<IFspActionLabel> Items { get; private set; }

        public override string ToString()
        {
            return "{" + string.Join(", ", Items.Select(x => x.ToString())) + "}";
        }

        public void Expand(FspExpressionEnvironment env, Action<IFspActionLabel> action)
        {
            foreach (var item in Items)
            {
                action(item);
            }
        }
    }
}
