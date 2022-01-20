using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Bidirezionale.ShortestAugmentingPath
{
    public class Graph
    {
        public HashSet<Node> Nodes { get; private set; }
        public Graph() => this.Nodes = new();
        public Graph(params Node[] nodes) => this.Nodes = new(nodes);
        public void AddNode(Node n) => this.Nodes.Add(n);
        public void AddNode(params Node[] nodes)
        {
            foreach (var n in nodes)
                this.Nodes.Add(n);
        }
        public Node Source => this.Nodes.Single(x => x is SourceNode);
        public Node Sink => this.Nodes.Single(x => x is SinkNode);
    }
}