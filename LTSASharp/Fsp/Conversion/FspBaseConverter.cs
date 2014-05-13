using System;
using System.Runtime.CompilerServices;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using LTSASharp.Parsing;

namespace LTSASharp.Fsp.Conversion
{
    public abstract class FspBaseConverter<TReturn> : FSPActualBaseVisitor<TReturn>
    {
        protected override bool ShouldVisitNextChild(IRuleNode node, TReturn currentResult)
        {
            // anything handled by this is done explicitly
            var rule = node.GetType().Name;
            var handler = GetType().Name;

            if (rule.EndsWith("Context"))
                rule = rule.Substring(0, rule.Length - 7);

            throw new InvalidOperationException(string.Format("Unhandled rule '{0}' in '{1}'", rule, handler));
        }

        protected void Check(bool test)
        {
            if (!test)
                throw new InvalidOperationException();
        }
        protected T Check<T>(T obj) 
            where T : class
        {
            if (obj == default(T))
                throw new InvalidOperationException();

            return obj;
        }

        protected void Unimpl<T>(T ctx, [CallerMemberName] string callerName = "")
            where T : ParserRuleContext
        {
            var rule = typeof(T).Name;
            var handler = GetType().Name;

            if (rule.EndsWith("Context"))
                rule = rule.Substring(0, rule.Length - 7);

            if (ctx != null)
                throw new NotImplementedException(string.Format("Unimplemented subcontext '{0}' in '{1}.{2}(...)'", rule, handler, callerName));
        }
        protected void Unimpl(ITerminalNode ctx)
        {
            if (ctx != null)
                throw new NotImplementedException();
        }
    }

    public abstract class FspBaseConverter : FspBaseConverter<bool>
    {

    }
}