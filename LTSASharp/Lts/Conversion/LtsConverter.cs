using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LTSASharp.Fsp;
using LTSASharp.Fsp.Choices;
using LTSASharp.Fsp.Processes;
using LTSASharp.Utilities;

namespace LTSASharp.Lts.Conversion
{
    class LtsConverter
    {
        private readonly FspDescription fspDescription;
        private int stateNumber = LtsState.Initial;

        public LtsConverter(FspDescription fsp)
        {
            fspDescription = fsp;
        }

        public LtsDescription Convert()
        {
            var description = new LtsDescription();

            foreach (var fsp in fspDescription.Processes)
            {
                description.Systems.Add(Convert(fsp));
            }

            return description;
        }

        private LtsSystem Convert(FspProcess fsp)
        {
            var lts = Convert(fsp.Body[fsp.Name]);

            foreach (var action in lts.Actions.ToList())
            {
                if(action == LtsAction.Tau)
                    continue;

                if (action.Destination.Number == LtsState.Ref)
                {
                    if(((LtsRefState)action.Destination).Name != fsp.Name)
                        throw new NotImplementedException();

                    var dest = lts.InitialState;

                    // replace
                    lts.Actions.Remove(action);
                    lts.Actions.Add(new LtsAction(action.Source, action.Action, dest));
                }
            }

            return lts;
        }

        private LtsSystem Convert(FspLocalProcess process)
        {
            var lts = new LtsSystem();

            if (process is FspChoice)
                process = new FspChoices { Children = { (FspChoice)process } };

            if (process is FspChoices)
            {
                var p = new LtsState(GetStateNumber());

                lts.States.Add(p);
                lts.InitialState = p;

                foreach (var c in ((FspChoices)process).Children)
                {
                    var a = new LtsLabel(c.Label);

                    if (c.Process is FspRefProcess)
                    {
                        var d = new LtsRefState(((FspRefProcess)c.Process).Name);

                        lts.Alphabet.Add(a);
                        lts.Actions.Add(new LtsAction(p, a, d));

                        continue;
                    }

                    var ltsC = Convert(c.Process);

                    //S
                    lts.States.AddRange(ltsC.States);

                    //A
                    lts.Alphabet.AddRange(ltsC.Alphabet);
                    lts.Alphabet.Add(a);

                    //Delta
                    lts.Actions.AddRange(ltsC.Actions);
                    lts.Actions.Add(new LtsAction(p, a, ltsC.InitialState));
                }

                return lts;
            }

            //TODO these should be different
            if (process is FspEndProcess || process is FspStopProcess)
            {
                lts.States.Add(LtsState.End);
                lts.Actions.Add(LtsAction.Tau);

                lts.InitialState = LtsState.End;

                return lts;
            }

            if (process is FspRefProcess)
                // for now die.
                throw new InvalidOperationException();

            throw new NotImplementedException();
        }

        private int GetStateNumber()
        {
            return stateNumber++;
        }
    }
}
