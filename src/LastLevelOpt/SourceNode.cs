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
            base.addEdge(node, cap);
            this.inFlow = this.inFlow - cap;
        }

        public override void addEdge(BiEdge edge)
        {
            base.addEdge(edge);
            this.inFlow = this.inFlow - edge.capacity;
        }
    }
}