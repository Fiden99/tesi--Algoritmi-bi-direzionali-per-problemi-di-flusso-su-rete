namespace Bidirezionale.NodePropagation.LastLevelOptEdgeFlow
{
    public class SourceNode : Node
    {
        public SourceNode(string name) : base(name)
        {
            this.Visited = true;
        }
        public override void Reset() { }
    }
}