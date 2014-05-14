using System;
using System.Collections.Generic;
using System.Linq;
using LTSASharp.Fsp.Expressions;
using LTSASharp.Fsp.Ranges;

namespace LTSASharp.Fsp.Labels
{
    class FspFollowAction : IFspActionLabel
    {
        public IFspActionLabel Head { get; set; }
        public IFspActionLabel Tail { get; set; }

        public FspFollowAction(IFspActionLabel head, IFspActionLabel tail)
        {
            Head = head;
            Tail = tail;
        }

        public override string ToString()
        {
            return Tail is FspRange
                ? Head + "[" + Tail + "]"
                : Head + "." + Tail;
        }

        public void Expand(FspExpressionEnvironment env, Action<IFspActionLabel> action)
        {
            Head.Expand(env, h => Tail.Expand(env, t => action(new FspFollowAction(h, t).MergeDown())));
        }

        public FspFollowAction MakeTailHeavy()
        {
            var children = GetChildren().AsQueryable();

            return (FspFollowAction)children.Reverse().Aggregate((a, b) => new FspFollowAction(b, a));
        }

        private IEnumerable<IFspActionLabel> GetChildren()
        {
            var children = new List<IFspActionLabel>();

            if (Head is FspFollowAction)
                children.AddRange(((FspFollowAction)Head).GetChildren());
            else
                children.Add(Head);

            if (Tail is FspFollowAction)
                children.AddRange(((FspFollowAction)Tail).GetChildren());
            else
                children.Add(Tail);

            return children;
        }

        public IFspActionLabel MergeDown()
        {
            IFspActionLabel h, t;

            if (Head is FspFollowAction)
                h = ((FspFollowAction)Head).MergeDown();
            else
                h = Head;

            if (Tail is FspFollowAction)
                t = ((FspFollowAction)Tail).MergeDown();
            else
                t = Tail;

            if (h is FspActionName && t is FspActionName)
                return new FspActionName(((FspActionName)h).Name + "." + ((FspActionName)t).Name);

            if ((h is FspActionName && h != Head) || (t is FspActionName && t != Tail))
                return new FspFollowAction(h, t);

            return this;
        }
    }
}
