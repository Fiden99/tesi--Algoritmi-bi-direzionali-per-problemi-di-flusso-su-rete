namespace Bidirezionale.NodePropagation.NoOpt
{
    public class SinkNode : Node
    {
        public SinkNode(string name) : base(name)
        {
            this.Label = int.MaxValue;
            this.Visited = true;
            this.SourceSide = false;
        }

        public override void Reset()
        {
            /*             this.SetNextEdge(null);
                        this.SetNextNode(null);
             */
        }
    }

}