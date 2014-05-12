using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LTSASharp.Fsp.Expressions;
using LTSASharp.Fsp.Labels;

namespace LTSASharp.Fsp.Ranges
{
    abstract class FspRange : IFspActionLabel
    {
        public abstract FspRangeBounds GetBounds(FspExpressionEnvironment env);
    }

    internal class FspRangeBounds
    {
        public FspRangeBounds(int lower, int upper)
        {
            Upper = upper;
            Lower = lower;
        }

        public int Lower { get; private set; }
        public int Upper { get; private set; }
    }
}
