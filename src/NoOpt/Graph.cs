using System;
using System.Collections.Generic;
using System.Linq;

namespace BFS.NoOpt
{
    public class Graph
    {
        public HashSet<Node> Nodes { get; private set; }

        public Graph()
        {
            this.Nodes = new HashSet<Node>();
        }
        public Graph(Node node)
        {
            this.Nodes = new HashSet<Node>(new Node[] { node });
        }
        public Graph(params Node[] nodes)
        {
            this.Nodes = new HashSet<Node>(nodes);
        }

        public void AddNode(Node node)
        {
            this.Nodes.Add(node);
        }

        public Node Source => Nodes.Single(x => x is SourceNode);
        public Node Sink => Nodes.Single(x => x is SinkNode);

        public void resetLabel()
        {
            foreach (Node x in this.Nodes)
            {
                if (x.valid == false)
                    x.initLabel(0);
                x.setInFlow(0);
                x.setPreviousNode(null);
            }
            this.Nodes.Single(x => x is SourceNode).setInFlow(int.MaxValue);

        }
    }
}