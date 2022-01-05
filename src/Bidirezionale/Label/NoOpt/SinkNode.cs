namespace Bidirezionale.Label.NoOpt
{
    public class SinkNode : Node
    {
        public SinkNode(string name) : base(name)
        {
            this.Label = int.MaxValue;
            this.InFlow = int.MaxValue;
        }
    }
}