namespace Monodirezionale.MaxFlow.SickPropagationGraphOpt
{
    public class SourceNode : Node
    {
        public SourceNode(string name) : base(name)
        {
            this.InFlow = int.MaxValue;
        }
    }
}