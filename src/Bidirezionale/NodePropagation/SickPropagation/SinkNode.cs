namespace Bidirezionale.NodePropagation.SickPropagation
{
    public class SinkNode : Node
    {
        public SinkNode(string name) : base(name)
        {
            this.SourceSide = false;
            this.InFlow = int.MaxValue;
        }
        public override void Reset() { }
    }
}