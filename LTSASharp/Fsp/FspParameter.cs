using LTSASharp.Fsp.Expressions;

namespace LTSASharp.Fsp
{
    public class FspParameter
    {
        public string Name { get; private set; }
        public FspExpression DefaultValue { get; private set; }

        public FspParameter(string name, FspExpression defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;
        }
    }
}