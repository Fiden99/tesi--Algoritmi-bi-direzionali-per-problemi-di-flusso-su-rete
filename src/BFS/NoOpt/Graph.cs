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

        public void ResetLabel()
        {
            foreach (Node x in this.Nodes)
            {
                if (x.Valid == false)
                    x.InitLabel(0);
                x.SetInFlow(0);
                x.SetPreviousNode(null);
            }
            this.Nodes.Single(x => x is SourceNode).SetInFlow(int.MaxValue);

        }
    }
}