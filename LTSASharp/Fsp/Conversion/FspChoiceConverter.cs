using LTSASharp.Fsp.Choices;
using LTSASharp.Fsp.Processes;
using LTSASharp.Parsing;

namespace LTSASharp.Fsp.Conversion
{
    internal class FspChoiceConverter : FspBaseConverter<FspChoice>
    {
        private readonly FspConverterEnvironment env;
        private readonly FspProcess process;
        private readonly FspChoices choices;

        public FspChoiceConverter(FspConverterEnvironment env, FspProcess process, FspChoices choices)
        {
            this.env = env;
            this.process = process;
            this.choices = choices;
        }

        public override FspChoice VisitActionPrefix(FSPActualParser.ActionPrefixContext context)
        {
            //guard? actionLabels Arrow (actionLabels Arrow)* localProcess
            FspChoice head = null;
            FspChoice tail = null;

            Unimpl(context.guard());

            foreach (var action in context.actionLabels())
            {
                var choice = new FspChoice { Label = action.Accept(new FspLabelConverter(env)) };

                if (tail == null)
                {
                    head = choice;
                    tail = choice;
                }
                else
                {
                    tail.Process = choice;
                    tail = choice;
                }
            }

            tail.Process = context.localProcess().Accept(new FspProcessConverter(env, process));

            return head;
        }
    }
}