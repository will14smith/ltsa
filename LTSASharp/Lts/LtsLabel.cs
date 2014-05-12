using System;
using LTSASharp.Fsp.Labels;

namespace LTSASharp.Lts
{
    internal class LtsLabel
    {
        public LtsLabel(IFspActionLabel label)
        {
            if (!(label is FspActionName))
                throw new InvalidOperationException();

            Name = ((FspActionName)label).Name;
        }

        public string Name { get; private set; }

        public override string ToString()
        {
            return "a" + Name;
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