namespace Bidirezionale.NodeCount.NoOpt
{
    public class SourceNode : Node
    {
        public SourceNode(string name) : base(name)
        {
            this.Visited = true;
        }

        public override void Reset()
        {
            /*             this.SetPreviousEdge(null);
                        this.SetPreviousNode(null);
             */
        }

    }
}