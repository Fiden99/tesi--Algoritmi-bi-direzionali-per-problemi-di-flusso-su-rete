namespace Bidirezionale.Label.SickPropagation
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