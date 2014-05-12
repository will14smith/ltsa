namespace LTSASharp.Lts
{
    /// <summary>
    /// Dummy state for temporary recursive referencing
    /// </summary>
    internal class LtsRefState : LtsState
    {
        public string Name { get; set; }

        public LtsRefState(string name)
            : base(LtsState.Ref)
        {
            Name = name;
        }
    }
}