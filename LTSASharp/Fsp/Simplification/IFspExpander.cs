namespace LTSASharp.Fsp.Simplification
{
    internal interface IFspExpander<out TProcess>
        where TProcess : FspBaseProcess
    {
        TProcess Expand();
    }
}