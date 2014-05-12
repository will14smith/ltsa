using LTSASharp.Fsp.Choices;
using LTSASharp.Fsp.Processes;
using LTSASharp.Parsing;

namespace LTSASharp.Fsp.Conversion
{
    internal class FspProcessConverter : FspBaseConverter<FspLocalProcess>
    {
        private readonly FspConverterEnvironment env;
        private readonly FspProcess process;

        public FspProcessConverter(FspConverterEnvironment env, FspProcess process)
        {
            this.env = env;
            this.process = process;
        }

        public override FspLocalProcess VisitProcessBody(FSPActualParser.ProcessBodyContext context)
        {
            // localProcess (Comma localProcessDefs)?;
            var parent = Check(context.localProcess().Accept(this));
            process.Body.Add(process.Name, parent);

            if (context.localProcessDefs() != null)
                Check(context.localProcessDefs().Accept(this));

            return new FspEndProcess();
        }

        public override FspLocalProcess VisitLocalProcessDefs(FSPActualParser.LocalProcessDefsContext context)
        {
            // localProcessDef (Comma localProcessDef)*;
            FspLocalProcess first = null;
            var firstSet = false;

            foreach (var x in context.localProcessDef())
            {
                if (!firstSet)
                {
                    first = x.Accept(this);
                    firstSet = true;
                }
                else
                {
                    x.Accept(this);
                }
            }

            return first;
        }

        public override FspLocalProcess VisitLocalProcessDef(FSPActualParser.LocalProcessDefContext context)
        {
            // UpperCaseIdentifier indexRanges? Equal localProcess
            var name = context.UpperCaseIdentifier().GetText();
            Unimpl(context.indexRanges());
            var local = context.localProcess().Accept(this);
            
            process.Body.Add(name, local);

            return local;
        }

        public override FspLocalProcess VisitLocalProcess(FSPActualParser.LocalProcessContext context)
        {
            //   baseLocalProcess
            // | sequentialComposition
            // | If expression Then localProcess
            // | If expression Then localProcess Else localProcess
            // | LRound choice RRound

            if (context.baseLocalProcess() != null)
            {
                return context.baseLocalProcess().Accept(this);
            }

            if (context.sequentialComposition() != null)
            {
                return context.sequentialComposition().Accept(this);
            }

            Unimpl(context.If());

            if (context.choice() != null)
            {
                return context.choice().Accept(this);
            }

            return null;
        }

        public override FspLocalProcess VisitBaseLocalProcess(FSPActualParser.BaseLocalProcessContext context)
        {
            // UpperCaseIdentifier indices?
            var name = context.UpperCaseIdentifier().GetText();

            switch (name)
            {
                case "STOP":
                    return new FspStopProcess();
                case "END":
                    return new FspEndProcess();
                case "ERROR":
                    return new FspErrorProcess();
                default:
                    return new FspRefProcess(name);
            }
        }

        public override FspLocalProcess VisitChoice(FSPActualParser.ChoiceContext context)
        {
            var choices = new FspChoices();

            foreach (var choice in context.actionPrefix())
            {
                var c = choice.Accept(new FspChoiceConverter(env, process, choices));

                choices.Children.Add(c);
            }

            return choices;
        }
    }
}