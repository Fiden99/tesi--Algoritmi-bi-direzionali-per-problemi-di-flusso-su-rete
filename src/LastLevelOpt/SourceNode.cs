using System.Linq;

namespace BFS.LastLevelOpt
{
    public class SourceNode : Node
    {
        public SourceNode(string name) : base(name)
        {
            this.inFlow = int.MaxValue;
        }

        public override void addEdge(Node node, int cap)
        {
            BiEdge edge = new BiEdge(this, node, cap);
            this.edges.Add(edge);
            node.addEdge(edge);
            this.inFlow = this.inFlow - cap;
        }

        public override void addEdge(BiEdge edge)
        {
            this.edges.Add(edge);
            this.inFlow = this.inFlow - edge.capacity;
        }
    }
}