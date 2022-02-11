using System.Collections.Generic;
using System.Linq;

namespace Bidirezionale.NodePropagation.NoOpt
{
    public class Graph
    {
        //label che divide le due parti, è la prima appartenente a quella di SinkNode
        public HashSet<Node> SourceNodes { get; private set; }
        public HashSet<Node> SinkNodes { get; private set; }
        public Graph()
        {
            this.SourceNodes = new();
            this.SinkNodes = new();
        }
        public Graph(params Node[] nodes)
        {
            this.SinkNodes = new();
            this.SourceNodes = new();
            foreach (var n in nodes)
                if (n is SinkNode)
                    this.SinkNodes.Add(n);
                else
                    this.SourceNodes.Add(n);
        }
        public void AddNode(Node n)
        {
            if (n is SinkNode)
                this.SinkNodes.Add(n);
            else
                this.SinkNodes.Add(n);
        }
        public Node Sink => this.SinkNodes.Single(x => x is SinkNode);
        public Node Source => this.SourceNodes.Single(x => x is SourceNode);

        public void ResetSourceSide()
        {
            foreach (Node n in this.SourceNodes)
                if (n.SourceSide)
                    n.Reset();
        }
        public void ResetSinkSide()
        {
            foreach (Node n in this.SinkNodes)
                if (!n.SourceSide)
                    n.Reset();
        }
        public void Reset()
        {
            ResetSinkSide();
            ResetSourceSide();
        }
        public void SetSide(Node n, bool sourceSide)
        {
            if (n.SourceSide == sourceSide)
                return;
            else if (n.SourceSide && !sourceSide)
            {
                this.SourceNodes.Remove(n);
                this.SinkNodes.Add(n);
            }
            else // !n.SourceSide && sourceSide
            {
                this.SinkNodes.Remove(n);
                this.SourceNodes.Add(n);
            }
            n.SetSourceSide(sourceSide);

        }
    }
}