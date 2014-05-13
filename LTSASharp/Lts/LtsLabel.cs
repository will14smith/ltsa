using System;
using LTSASharp.Fsp.Labels;

namespace LTSASharp.Lts
{
    public class LtsLabel
    {
        public static readonly LtsLabel Tau = new LtsLabel("tau");

        public LtsLabel(IFspActionLabel label)
        {
            if (label is FspFollowAction)
                label = ((FspFollowAction) label).MergeDown();

            if (!(label is FspActionName))
                throw new InvalidOperationException();

            Name = ((FspActionName)label).Name;
        }
        public LtsLabel(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (!(obj is LtsLabel))
                return false;

            var other = (LtsLabel)obj;
            return other.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}