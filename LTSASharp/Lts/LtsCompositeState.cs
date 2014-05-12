namespace LTSASharp.Lts
{
    internal class LtsCompositeState : LtsState
    {
        public LtsState A { get; private set; }
        public LtsState B { get; private set; }

        public LtsCompositeState(LtsState a, LtsState b) : base(Composite)
        {
            A = a;
            B = b;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (!(obj is LtsCompositeState))
                return false;

            var other = (LtsCompositeState)obj;
            return other.A == A && other.B == B;
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() ^ B.GetHashCode();
        }

        public override string ToString()
        {
            return "s(" + A + ", " + B + ")";
        }
    }
}