namespace LTSASharp.Lts
{
    internal class LtsAction
    {
        //TODO
        public static readonly LtsAction Tau = new LtsAction(null, null, null);

        public LtsState Source { get; private set; }
        public LtsLabel Action { get; private set; }
        public LtsState Destination { get; private set; }

        public LtsAction(LtsState source, LtsLabel action, LtsState destination)
        {
            Source = source;
            Action = action;
            Destination = destination;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", Source, Action, Destination);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (!(obj is LtsAction))
                return false;

            var other = (LtsAction) obj;
            return other.Source == Source && other.Action == Action && other.Destination == Destination;
        }

        public override int GetHashCode()
        {
            if (ReferenceEquals(this, Tau))
                return 0;

            return Source.GetHashCode() ^ Action.GetHashCode() ^ Destination.GetHashCode();
        }
    }
}