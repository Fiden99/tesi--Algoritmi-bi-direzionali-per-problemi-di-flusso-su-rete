using System.Collections.Generic;
using System.Linq;

namespace BFS.ShortestAugmentingPath
{
    public class Graph
    {
        public HashSet<Node> Nodes { get; private set; }

        public Graph()
        {
            this.Nodes = new HashSet<Node>();
        }
        public Graph(params Node[] nodes)
        {
            this.Nodes = new HashSet<Node>(nodes);
        }
        public void AddNode(Node n)
        {
            this.Nodes.Add(n);
        }
        public Node Source => this.Nodes.Single(x => x is SourceNode);
        public Node Sink => this.Nodes.Single(x => x is SinkNode);

    }
}