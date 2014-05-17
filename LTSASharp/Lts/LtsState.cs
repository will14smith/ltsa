using System;

namespace LTSASharp.Lts
{
    public class LtsState
    {
        public const int EndNumber = -1;
        public static readonly LtsState End = new LtsState(EndNumber);
        public static int Initial = 0;

        public int Number { get; private set; }

        public LtsState(int number)
        {
            Number = number;
        }

        public override string ToString()
        {
            if (Number >= 0) return "s" + Number;

            switch (Number)
            {
                case -1:
                    return "sEND";
                default:
                    throw new InvalidOperationException();
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (!(obj is LtsState))
                return false;

            var other = (LtsState)obj;
            return other.Number == Number;
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }
    }
}