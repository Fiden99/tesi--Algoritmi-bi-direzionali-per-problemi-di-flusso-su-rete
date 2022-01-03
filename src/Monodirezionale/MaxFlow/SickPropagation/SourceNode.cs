
namespace Monodirezionale.MaxFlow.SickPropagation
{
    public class SourceNode : Node
    {
        public SourceNode(string name) : base(name)
        {
            this.Visited = true;
        }
        /*
                public override void addEdge(Node node, int cap)
                {
                    base.addEdge(node, cap);
                    this.inFlow = this.inFlow - cap;
                }

                public override void addEdge(BiEdge edge)
                {
                    base.addEdge(edge);
                    this.inFlow = this.inFlow - edge.capacity;
                }
        */
    }
}