using System;
using System.Collections.Generic;
using System.Linq;
using LTSASharp.Fsp.Composites;
using LTSASharp.Fsp.Expressions;
using LTSASharp.Fsp.Labels;
using LTSASharp.Fsp.Ranges;

namespace LTSASharp.Fsp.Simplification
{
    class FspCompositeExpander
    {
        private readonly FspComposite composite;
        private FspExpressionEnvironment env;

        public FspCompositeExpander(FspComposite composite)
        {
            this.composite = composite;
        }

        public FspComposite Expand()
        {
            var newComposite = new FspComposite { Name = composite.Name };

            env = new FspExpressionEnvironment();

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
                return new[] { body };

            if (body is FspLabelComposite)
            {
                var compLabel = (FspLabelComposite)body;
                var results = new List<FspCompositeBody>();

                // Expand label
                if (compLabel.Label is FspRange)
                {
                    var range = (FspRange)compLabel.Label;
                    range.Iterate(env, i =>
                                       {
                                           var inner = new FspLabelComposite(compLabel.Body, new FspActionName(i.ToString()));
                                           results.AddRange(Expand(inner));
                                       });

                    return results;
                }
                // Convert to relabel
                compLabel.Label.Expand(env, label => results.Add(new FspPrefixRelabel(compLabel.Body, label)));

                return results;
            }

            if (body is FspShareComposite)
            {
                var compShare = (FspShareComposite)body;

                // Convert to relabel
                return new FspCompositeBody[] { new FspPrefixRelabel(compShare.Body, compShare.Label) };
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
