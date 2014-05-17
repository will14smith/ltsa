using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using LTSASharp.Fsp;
using LTSASharp.Fsp.Choices;
using LTSASharp.Fsp.Composites;
using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Processes;
using LTSASharp.Utilities;
using FspBaseProcess = LTSASharp.Fsp.FspBaseProcess;

namespace LTSASharp.Lts.Conversion
{
    public class LtsConverter
    {
        private readonly FspDescription fspDescription;
        private int stateNumber = LtsState.Initial;
        private LtsDescription description;

        public LtsConverter(FspDescription fsp)
        {
            fspDescription = fsp;
        }

        public LtsDescription Convert()
        {
            description = new LtsDescription();

            foreach (var fsp in fspDescription.Processes)
            {
                description.Systems.Add(fsp.Key, Convert(fsp.Value).Prune());
            }

            return description;
        }

        private LtsSystem Convert(FspBaseProcess process)
        {
            if (process is FspProcess)
                return Convert((FspProcess)process);

            if (process is FspComposite)
                return Convert((FspComposite)process);

            throw new ArgumentException("Unexpected process type", "process");
        }

        #region Composities
        private LtsSystem Convert(FspComposite composite)
        {
            stateNumber = 0;
            var systems = composite.Body.Select(Convert);

            return systems.Aggregate(MergeComposites);
        }

        private LtsSystem Convert(FspCompositeBody composite)
        {
            if (composite is FspRefComposite)
            {
                //TODO clone?
                return description.Systems[((FspRefComposite)composite).Name];
            }

            if (composite is FspPrefixRelabel)
            {
                var relabel = (FspPrefixRelabel)composite;
                var lts = Convert(relabel.Body);

                var relabelMap = new MultiMap<LtsLabel, LtsLabel>();

                // Given:
                //     USER = (acquire->use->release->USER).
                //
                // a:USER == USER/{a.acquire/acquire, a.use/use, a.release/release}

                foreach (var label in lts.Alphabet)
                {
                    BuildMap(label, relabel.Label, relabelMap);
                }

                return new LtsRelabeler(relabelMap).Relabel(lts);
            }

            throw new InvalidOperationException();
        }

        private void BuildMap(LtsLabel name, IFspActionLabel label, MultiMap<LtsLabel, LtsLabel> relabelMap)
        {
            if (label is FspActionName)
                relabelMap.Map(name, new LtsLabel(((FspActionName)label).Name + "." + name.Name));
            else if (label is FspActionSet)
            {
                var set = ((FspActionSet)label).Items;
                foreach (var item in set)
                    BuildMap(name, item, relabelMap);
            }
            else
                throw new InvalidOperationException();
        }

        private LtsSystem MergeComposites(LtsSystem p, LtsSystem q)
        {
            //TODO if P == error || Q == error => error

            var stateMap = new Dictionary<LtsState, Tuple<LtsState, LtsState>>();

            var lts = new LtsSystem();

            // S = Sp X Sq
            foreach (var sp in p.States)
                foreach (var sq in q.States)
                {
                    var state = new LtsState(GetNextStateNumber());

                    lts.States.Add(state);
                    stateMap.Add(state, Tuple.Create(sp, sq));

                    // Q = (Qp, Qq)
                    if (p.InitialState == sp && q.InitialState == sq)
                        lts.InitialState = state;
                }

            // A = Ap U Aq
            lts.Alphabet.AddRange(p.Alphabet);
            lts.Alphabet.AddRange(q.Alphabet);

            // delta == special
            //TODO optimise?
            foreach (var s1 in lts.States)
                foreach (var a in lts.Alphabet)
                    foreach (var s2 in lts.States)
                        if (ShouldAddTransition(p, q, Tuple.Create(stateMap[s1], a, stateMap[s2])))
                            lts.Transitions.Add(new LtsAction(s1, a, s2));

            return lts;
        }

        private bool ShouldAddTransition(LtsSystem p, LtsSystem q, Tuple<Tuple<LtsState, LtsState>, LtsLabel, Tuple<LtsState, LtsState>> transition)
        {
            var inP = p.Transitions.Contains(new LtsAction(transition.Item1.Item1, transition.Item2, transition.Item3.Item1));
            var inQ = q.Transitions.Contains(new LtsAction(transition.Item1.Item2, transition.Item2, transition.Item3.Item2));

            if (inP && inQ && transition.Item2 != LtsLabel.Tau)
                return true;

            if (inP && !q.Alphabet.Contains(transition.Item2) && transition.Item1.Item2 == transition.Item3.Item2)
                return true;

            if (inQ && !p.Alphabet.Contains(transition.Item2) && transition.Item1.Item1 == transition.Item3.Item1)
                return true;

            return false;
        }

        #endregion

        #region Processes

        private Dictionary<string, LtsState> initialStates;

        private LtsSystem Convert(FspProcess fsp)
        {
            stateNumber = 0;
            initialStates = new Dictionary<string, LtsState> { { fsp.Name, new LtsState(stateNumber) } };

            var lts = Convert(fsp.Body[fsp.Name].Single(), fsp);

            return lts;
        }
        private LtsSystem Convert(FspLocalProcess process, FspProcess fsp)
        {
            var lts = new LtsSystem();

            if (process is FspChoice)
                process = new FspChoices { Children = { (FspChoice)process } };

            if (process is FspChoices)
            {
                var p = new LtsState(GetNextStateNumber());

                lts.States.Add(p);
                lts.InitialState = p;

                foreach (var c in ((FspChoices)process).Children)
                {
                    var a = new LtsLabel(c.Label);

                    var ltsC = Convert(c.Process, fsp);

                    //S
                    lts.States.AddRange(ltsC.States);

                    //A
                    lts.Alphabet.AddRange(ltsC.Alphabet);
                    lts.Alphabet.Add(a);

                    //Delta
                    lts.Transitions.AddRange(ltsC.Transitions);
                    lts.Transitions.Add(new LtsAction(p, a, ltsC.InitialState));
                }

                return lts;
            }

            //TODO these should be different
            if (process is FspEndProcess || process is FspStopProcess)
            {
                lts.States.Add(LtsState.End);
                // lts.Transitions.Add(Tau);
                lts.InitialState = LtsState.End;

                return lts;
            }

            if (process is FspLocalRefProcess)
            {
                var target = ((FspLocalRefProcess)process).Name;

                if (!initialStates.ContainsKey(target))
                {
                    initialStates.Add(target, new LtsState(stateNumber));
                    var refLts = Convert(fsp.Body[target].Single(), fsp);

                    return refLts;
                }

                lts.InitialState = initialStates[target];
                return lts;
            }

            if (process is FspSequenceProcess)
            {
                var seq = (FspSequenceProcess)process;

                lts = Convert(seq.Processes.First(), fsp);

                foreach (var sub in seq.Processes.Skip(1))
                {
                    var subLts = Convert(sub, fsp);


                    lts.States.AddRange(subLts.States);
                    lts.Alphabet.AddRange(subLts.Alphabet);

                    foreach (var trans in lts.Transitions.Where(x => x.Destination == LtsState.End).ToList())
                    {
                        lts.Transitions.Remove(trans);
                        lts.Transitions.Add(new LtsAction(trans.Source, trans.Action, subLts.InitialState));
                    }
                    lts.Transitions.AddRange(subLts.Transitions);

                    // lts.InitialState doesn't change;
                }

                return lts;
            }

            if (process is FspRefProcess)
            {
                //TODO handle when process is ref'd before convertion has happened
                lts = description.Systems[((FspRefProcess)process).Name];

                // remap the referenced lts so that nothing conflicts
                var mappedLts = new LtsSystem();
                var stateMap = RemapStates(lts.States);
                
                mappedLts.States.AddRange(stateMap.Values);
                mappedLts.Alphabet.AddRange(lts.Alphabet);
                foreach (var trans in lts.Transitions)
                {
                    mappedLts.Transitions.Add(new LtsAction(stateMap[trans.Source], trans.Action, stateMap[trans.Destination]));
                }
                mappedLts.InitialState = stateMap[lts.InitialState];

                return mappedLts;
            }

            throw new ArgumentException("Unexpected local process type", "process");
        }

        private Dictionary<LtsState, LtsState> RemapStates(IEnumerable<LtsState> states)
        {
            var map = new Dictionary<LtsState, LtsState>();

            foreach (var state in states.OrderBy(x => x.Number))
            {
                switch (state.Number)
                {
                    case LtsState.EndNumber:
                        map.Add(LtsState.End, LtsState.End);
                        break;
                    default:
                        if (state.Number < 0)
                            throw new InvalidOperationException();

                        map.Add(state, new LtsState(GetNextStateNumber()));
                        break;
                }
            }

            return map;
        }

        #endregion

        private int GetNextStateNumber()
        {
            return stateNumber++;
        }
    }
}
