using System;
using System.Collections.Generic;
using System.Linq;
using LTSASharp.Fsp.Composites;
using LTSASharp.Fsp.Expressions;
using LTSASharp.Fsp.Ranges;

namespace LTSASharp.Fsp.Simplification
{
    class FspCompositeExpander : IFspExpander<FspComposite>
    {
        private readonly FspComposite composite;

        private readonly FspDescription oldDesc;
        private readonly FspDescription newDesc;

        private readonly FspExpressionEnvironment env;
        private readonly string name;

        public FspCompositeExpander(FspComposite composite, FspDescription oldDesc, FspDescription newDesc)
        {
            this.composite = composite;
            this.oldDesc = oldDesc;
            this.newDesc = newDesc;

            env = new FspExpressionEnvironment(oldDesc);
            name = composite.Name;

            foreach (var param in composite.Parameters)
            {
                var value = param.DefaultValue.GetValue(env);

                name += "." + value;
                env.PushVariable(param.Name, value);
            }
        }

        public FspCompositeExpander(FspComposite composite, string name, FspExpressionEnvironment env, FspDescription oldDesc, FspDescription newDesc)
        {
            this.composite = composite;
            this.oldDesc = oldDesc;
            this.newDesc = newDesc;

            this.env = env;
            this.name = name;
        }

        public FspComposite Expand()
        {
            var newComposite = new FspComposite { Name = name };


            foreach (var result in composite.Body.Select(Expand).SelectMany(r => r))
            {
                newComposite.Body.Add(result);
            }

            return newComposite;
        }

        /// <summary>
        /// Given:
        ///     RESOURCE = (acquire->release->RESOURCE).
        ///     USER = (acquire->use->release->USER).
        /// 
        /// a:USER == USER/{a.acquire/acquire, a.use/use, a.release/release}
        /// {a,b}:USER == (a:USER || b:USER)
        /// {a,b}::RESOURCE == RESOURCE/{a.acquire/acquire, b.acquire/acquire, a.release/release, b.release/release}
        /// {a,b}::c:RESOURCE == RESOURCE/{a.c.acquire/acquire, b.c.acquire/acquire, a.c.release/release, b.c.release/release}
        /// </summary>
        private IList<FspCompositeBody> Expand(FspCompositeBody body)
        {
            if (body is FspRefComposite)
            {
                var compRef = (FspRefComposite)body;
                if (!compRef.Arguments.Any())
                    return new[] { body };

                var newRef = compRef.Arguments.Aggregate(compRef.Name, (n, arg) => n + ("." + arg.GetValue(env)));

                if (!newDesc.Processes.ContainsKey(newRef))
                {
                    var paramProc = oldDesc.Processes[compRef.Name];

                    // populate arguments
                    var paramEnv = new FspExpressionEnvironment(oldDesc);
                    for (int i = 0; i < paramProc.Parameters.Count; i++)
                        paramEnv.PushVariable(paramProc.Parameters[i].Name, compRef.Arguments[i].GetValue(env));

                    var expander = FspExpanderFactory.GetExpander(paramProc, newRef, paramEnv, oldDesc, newDesc);

                    // add expanded ref'd process to description
                    newDesc.Processes.Add(newRef, expander.Expand());
                }

                // return ref to expanded process
                return new FspCompositeBody[] { new FspRefComposite(newRef) };
            }

            if (body is FspLabelComposite)
            {
                var compLabel = (FspLabelComposite)body;
                var results = new List<FspCompositeBody>();

                // Expand label
                if (compLabel.Label is FspRange)
                {
                    var range = (FspRange)compLabel.Label;
                    range.Expand(env, label =>
                                       {
                                           var inner = new FspLabelComposite(compLabel.Body, label);
                                           results.AddRange(Expand(inner));
                                       });

                    return results;
                }
                // Convert to relabel
                compLabel.Label.Expand(env, label => results.AddRange(Expand(compLabel.Body).Select(x => new FspPrefixRelabel(x, label))));

                return results;
            }

            if (body is FspShareComposite)
            {
                var compShare = (FspShareComposite)body;

                // Convert to relabel
                return Expand(compShare.Body).Select(x => new FspPrefixRelabel(x, compShare.Label)).ToList<FspCompositeBody>();
            }

            if (body is FspRangeComposition)
            {
                // forall[i:0..2] S[i] == (S[0] || S[1] || S[2])
                var compRange = (FspRangeComposition)body;

                var results = new List<FspCompositeBody> { compRange.Body };

                foreach (var range in compRange.Range)
                {
                    var newResults = new List<FspCompositeBody>();
                    range.Iterate(env, i =>
                    {
                        foreach (var result in results)
                        {
                            newResults.AddRange(Expand(result));
                        }
                    });
                    results = newResults;
                }

                return results;
            }

            throw new ArgumentException("Unexpected FspCompositeBody type", "body");
        }
    }
}
