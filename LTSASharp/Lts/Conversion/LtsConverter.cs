﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LTSASharp.Fsp;
using LTSASharp.Fsp.Choices;
using LTSASharp.Fsp.Composites;
using LTSASharp.Fsp.Processes;
using LTSASharp.Utilities;

namespace LTSASharp.Lts.Conversion
{
    class LtsConverter
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
                description.Systems.Add(fsp.Name, Convert(fsp));
            }
            foreach (var fsp in fspDescription.Composites)
            {
                description.Systems.Add(fsp.Name, Convert(fsp));
            }

            return description;
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
                    var state = new LtsState(GetStateNumber());

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
            
            var count = -1;
            while (lts.Transitions.Count != count)
            {
                foreach (var transition in lts.Transitions.ToList())
                {
                    // prune unreachable transition
                    if (lts.InitialState == transition.Source)
                        continue;

                    if (!lts.Transitions.Any(x => x.Destination == transition.Source))
                        lts.Transitions.Remove(transition);
                }

                count = lts.Transitions.Count;
            }

            foreach (var state in lts.States.ToList())
            {
                // prune unreachable state
                if (lts.InitialState == state)
                    continue;

                if (!lts.Transitions.Any(x => x.Destination == state))
                    lts.States.Remove(state);
            }

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
        private LtsSystem Convert(FspProcess fsp)
        {
            stateNumber = 0;
            var lts = Convert(fsp.Body[fsp.Name]);

            foreach (var action in lts.Transitions.ToList())
            {
                if (action.Action == LtsLabel.Tau)
                    continue;

                if (action.Destination.Number == LtsState.Ref)
                {
                    if (((LtsRefState)action.Destination).Name != fsp.Name)
                        throw new NotImplementedException();

                    var dest = lts.InitialState;

                    // replace
                    lts.Transitions.Remove(action);
                    lts.Transitions.Add(new LtsAction(action.Source, action.Action, dest));
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
                        lts.Transitions.Add(new LtsAction(p, a, d));

                        continue;
                    }

                    var ltsC = Convert(c.Process);

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
               //TODO lts.Transitions.Add(Tau);

                lts.InitialState = LtsState.End;

                return lts;
            }

            if (process is FspRefProcess)
                // for now die.
                throw new InvalidOperationException();

            throw new NotImplementedException();
        }
        #endregion

        private int GetStateNumber()
        {
            return stateNumber++;
        }
    }
}
