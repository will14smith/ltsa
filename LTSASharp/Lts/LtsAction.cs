namespace LTSASharp.Lts
{
    public class LtsAction
    {
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
            return Equals(other.Source, Source) && Equals(other.Action, Action) && Equals(other.Destination, Destination);
        }

        public override int GetHashCode()
        {
            if (ReferenceEquals( Action, LtsLabel.Tau))
                return 0;

            return Source.GetHashCode() ^ Action.GetHashCode() ^ Destination.GetHashCode();
        }
    }
}