namespace LTSASharp.Fsp.Simplification
{
    internal interface IFspExpandable<TExpand, out TLocal> where TExpand : FspBaseProcess
    {
        TLocal ExpandProcess(FspExpanderEnvironment<TExpand> env);
    }
}