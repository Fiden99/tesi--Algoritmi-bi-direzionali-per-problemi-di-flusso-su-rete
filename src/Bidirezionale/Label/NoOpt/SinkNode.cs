namespace Bidirezionale.Label.NoOpt
{
    public class SinkNode : Node
    {
        public SinkNode(string name) : base(name)
        {
            this.Label = int.MaxValue;
            this.Visited = true;
        }

        public override void Reset()
        {
            /*             this.SetNextEdge(null);
                        this.SetNextNode(null);
             */
        }
    }

}