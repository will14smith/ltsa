using LTSASharp.Fsp.Expressions;

namespace LTSASharp.Fsp.Simplification
{
    public class FspExpanderEnvironment<TExpand>
        where TExpand : FspBaseProcess
    {
        public string Name { get; private set; }

        public TExpand Process { get; private set; }

        public FspDescription OldDesc { get; private set; }
        public FspDescription NewDesc { get; private set; }

        public FspExpressionEnvironment ExprEnv { get; private set; }

        public FspExpanderEnvironment(TExpand process, FspDescription oldDesc, FspDescription newDesc)
        {
            Process = process;
            OldDesc = oldDesc;
            NewDesc = newDesc;

            ExprEnv = new FspExpressionEnvironment(oldDesc);

            Name = process.Name;
            foreach (var param in process.Parameters)
            {
                var value = param.DefaultValue.GetValue(ExprEnv);

                Name += "." + value;
                ExprEnv.PushVariable(param.Name, value);
            }
        }

        public FspExpanderEnvironment(TExpand process, FspDescription oldDesc, FspDescription newDesc, FspExpressionEnvironment initialEnv, string name)
        {
            Process = process;
            OldDesc = oldDesc;
            NewDesc = newDesc;

            Name = name;
            ExprEnv = initialEnv;
        }

    }
}