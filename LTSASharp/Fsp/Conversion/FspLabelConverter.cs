using System;
using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Ranges;
using LTSASharp.Parsing;

namespace LTSASharp.Fsp.Conversion
{
    internal class FspLabelConverter : FspBaseConverter<IFspActionLabel>
    {
        private readonly FspConverterEnvironment env;

        public FspLabelConverter(FspConverterEnvironment env)
        {
            this.env = env;
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
                    tail = t.actionRange().Accept(new FspRangeConverter(env));
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