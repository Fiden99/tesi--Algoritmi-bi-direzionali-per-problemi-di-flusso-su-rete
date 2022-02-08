using System.Collections.Generic;
using System.Linq;

namespace Bidirezionale.NodeCount.NoOpt
{
    public class Graph
    {
        //label che divide le due parti, è la prima appartenente a quella di SinkNode
        public HashSet<Node> Nodes { get; private set; }
        public Graph()
        {
            this.Nodes = new();
        }
        public Graph(params Node[] nodes)
        {
            this.Nodes = new(nodes);
        }
        public void AddNode(Node n) => this.Nodes.Add(n);
        public Node Sink => this.Nodes.Single(x => x is SinkNode);
        public Node Source => this.Nodes.Single(x => x is SourceNode);

        public void ResetSourceSide()
        {
            foreach (Node n in this.Nodes)
                if (n.SourceSide)
                    n.Reset();
        }
        public void ResetSinkSide()
        {
            foreach (Node n in this.Nodes)
                if (!n.SourceSide)
                    n.Reset();
        }
        public void Reset()
        {
            foreach (var n in Nodes)
                n.Reset();
        }
    }
}