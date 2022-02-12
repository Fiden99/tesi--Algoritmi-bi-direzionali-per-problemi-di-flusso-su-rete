namespace Bidirezionale.NodeCount.LastLevelOptEdgeFlow
{
    public class SourceNode : Node
    {
        public SourceNode(string name) : base(name)
        {
            this.SourceSide = true;
            this.Visited = true;
        }
        public override void Reset() { }
    }
}