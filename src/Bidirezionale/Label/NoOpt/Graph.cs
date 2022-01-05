using System.Collections.Generic;
using System.Linq;

namespace Bidirezionale.Label.NoOpt
{
    public class Graph
    {
        //label che divide le due parti, è la prima appartenente a quella di SinkNode
        public int MeanLabel { get; private set; }
        public HashSet<Node> Nodes { get; private set; }
        public Graph()
        {
            this.Nodes = new();
        }
        public Graph(params Node[] nodes)
        {
            this.Nodes = new(nodes);
            this.MeanLabel = 0;
        }
        public void AddNode(Node n) => this.Nodes.Add(n);
        public void SetMeanLabel(int l) => this.MeanLabel = l;
        public Node Sink => this.Nodes.Single(x => x is SinkNode);
        public Node Source => this.Nodes.Single(x => x is SourceNode);
        public void Reset(Node node)
        {
            if (node is not SourceNode && node is not SinkNode)
                node.SetInFlow(0);
            node.SetPreviousEdge(null);
            node.SetPreviousNode(null);
            node.SetNextEdge(null);
            node.SetNextNode(null);
        }

        public void ResetSourceSide()
        {
            foreach (Node n in this.Nodes)
                if (n.Label < this.MeanLabel)
                    Reset(n);
        }
        public void ResetSinkSide()
        {
            foreach (Node n in this.Nodes)
                if (n.Label >= this.MeanLabel)
                    Reset(n);
        }
        public void Reset()
        {
            foreach (var n in Nodes)
                Reset(n);
        }
    }
}