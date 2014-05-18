using System;
using LTSASharp.Fsp.Ranges;
using LTSASharp.Fsp.Relabelling;
using LTSASharp.Parsing;

namespace LTSASharp.Fsp.Conversion
{
    internal class FspRelabelConverter : FSPActualBaseVisitor<FspRelabel>
    {
        private readonly FspConverterEnvironment env;
        private readonly FspRelabel relabel;

        public FspRelabelConverter(FspConverterEnvironment env)
        {
            this.env = env;

            relabel = new FspRelabel();
        }

        public override FspRelabel VisitRelabel(FSPActualParser.RelabelContext context)
        {
            return context.relabelDefs().Accept(this);
        }

        public override FspRelabel VisitRelabelDefs(FSPActualParser.RelabelDefsContext context)
        {
            foreach (var def in context.relabelDef())
                def.Accept(this);

            return relabel;
        }

        public override FspRelabel VisitRelabelDef(FSPActualParser.RelabelDefContext context)
        {
            if (context.Divide() != null)
            {
                var newLabel = context.actionLabels(0).Accept(new FspLabelConverter(env));
                var oldLabel = context.actionLabels(1).Accept(new FspLabelConverter(env));

                relabel.Entries.Add(new FspRelabel.DirectEntry { New = newLabel, Old = oldLabel });
            }
            else if (context.ForAll() != null)
            {
                var entry = new FspRelabel.IndexedEntry();

                //TODO is order important?
                foreach (var t in context.indexRanges().expression())
                {
                    var expr = t.Accept(new FspExpressionConverter(env));

                    entry.Ranges.Add(new FspExpressionRange(expr));
                }
                foreach (var t in context.indexRanges().actionRange())
                {
                    var index = t.Accept(new FspRangeConverter(env));

                    entry.Ranges.Add(index);
                }
                
                var nestedRelabel = context.relabelDefs().Accept(new FspRelabelConverter(env));

                entry.Entries.AddRange(nestedRelabel.Entries);

                relabel.Entries.Add(entry);
            }
            else
                throw new InvalidOperationException();


            return relabel;
        }
    }
}