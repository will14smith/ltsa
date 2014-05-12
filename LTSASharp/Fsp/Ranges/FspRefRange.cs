using LTSASharp.Fsp.Expressions;

namespace LTSASharp.Fsp.Ranges
{
    class FspRefRange : FspRange
    {
        public FspRefRange(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public override FspRangeBounds GetBounds(FspExpressionEnvironment env)
        {
            throw new System.NotImplementedException();
        }
    }
}