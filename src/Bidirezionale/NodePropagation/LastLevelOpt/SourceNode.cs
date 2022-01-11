namespace Bidirezionale.NodePropagation.LastLevelOpt
{
    public class SourceNode : Node
    {
        public SourceNode(string name) : base(name)
        {
            this.SourceSide = true;
            this.InFlow = int.MaxValue;
        }
        public override void Reset() { }
    }
}