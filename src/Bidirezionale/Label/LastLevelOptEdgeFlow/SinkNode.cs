namespace Bidirezionale.Label.LastLevelOptEdgeFlow
{
    public class SinkNode : Node
    {
        public SinkNode(string name) : base(name)
        {
            this.SourceSide = false;
            this.Visited = true;
        }
        public override void Reset() { }
    }
}