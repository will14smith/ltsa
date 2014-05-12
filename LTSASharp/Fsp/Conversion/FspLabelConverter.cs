using System;
using Antlr4.Runtime.Tree;
using LTSASharp.Fsp.Choices;
using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Processes;
using LTSASharp.Fsp.Ranges;
using LTSASharp.Parsing;

namespace LTSASharp.Fsp.Conversion
{
    internal class FspLabelConverter : FspBaseConverter<IFspActionLabel>
    {
        private readonly FspConverterEnvironment env;
        private readonly FspProcess process;
        private readonly FspChoices choices;
        private readonly FspChoice choice;

        public FspLabelConverter(FspConverterEnvironment env, FspProcess process, FspChoices choices, FspChoice choice)
        {
            this.env = env;
            this.process = process;
            this.choices = choices;
            this.choice = choice;
        }

        public override IFspActionLabel VisitActionLabels(FSPActualParser.ActionLabelsContext context)
        {
            // (actionLabel | set | LSquare actionRange RSquare) (actionLabelsTail)*
            IFspActionLabel head;

            if (context.actionLabel() != null)
            {
                head = context.actionLabel().Accept(this);
            }
            else if (context.set() != null)
            {
                head = context.set().Accept(this);
            }
            else
            {
                head = context.actionRange().Accept(this);
            }

            foreach (var t in context.actionLabelsTail())
            {
                // Dot (actionLabel | set) | LSquare (actionRange | expression) RSquare
                IFspActionLabel tail = null;

                if (t.actionLabel() != null)
                {
                    tail = t.actionLabel().Accept(this);
                }
                else if (t.actionRange() != null)
                {
                    var range = t.actionRange().range() != null
                        ? t.actionRange().range().Accept(new FspRangeConverter(env))
                        : t.actionRange().set().Accept(new FspRangeConverter(env));

                    var name = t.actionRange().LowerCaseIdentifier();

                    tail = name != null
                        ? new FspActionRange(name.GetText(), range)
                        : new FspActionRange(range);
                }
                else if (t.expression() != null)
                {
                    var expr = t.expression().Accept(new FspExpressionConverter(env));

                    tail = new FspExpressionRange(expr);
                }

                Unimpl(t.set());
               
                head = new FspFollowAction(head, tail);
            }

            return head;
        }

        public override IFspActionLabel VisitSet(FSPActualParser.SetContext context)
        {
            Unimpl(context.UpperCaseIdentifier());

            if (context.setElements().actionLabels() != null)
            {
                var labelSet = new FspActionSet();

                foreach (var label in context.setElements().actionLabels())
                {
                    labelSet.Items.Add(label.Accept(this));
                }

                return labelSet;
            }

            throw new InvalidOperationException();
        }

        public override IFspActionLabel VisitActionLabel(FSPActualParser.ActionLabelContext context)
        {
            // (LowerCaseIdentifier | LSquare expression RSquare) (actionLabelTail)*;
            IFspActionLabel head = null;

            if (context.LowerCaseIdentifier() != null)
            {
                head = new FspActionName(context.LowerCaseIdentifier().GetText());
            }
            else
            {
                Unimpl(context.expression());
            }

            foreach (var t in context.actionLabelTail())
            {
                // Dot LowerCaseIdentifier | LSquare expression RSquare
                IFspActionLabel tail = null;

                if (t.LowerCaseIdentifier() != null)
                {
                    tail = new FspActionName(t.LowerCaseIdentifier().GetText());
                }
                else if (t.expression() != null)
                {
                    var expr = t.expression().Accept(new FspExpressionConverter(env));

                    tail = new FspExpressionRange(expr);
                }

                head = new FspFollowAction(head, tail);
            }

            return head;
        }
    }
}