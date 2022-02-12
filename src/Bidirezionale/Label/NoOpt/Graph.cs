using System.Collections.Generic;
using System.Linq;

namespace Bidirezionale.Label.NoOpt
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
            this.SourceNodes = new();
            this.SinkNodes = new();
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
                this.SourceNodes.Add(n);
        }

        public Node Sink => this.SinkNodes.Single(x => x is SinkNode);
        public Node Source => this.SourceNodes.Single(x => x is SourceNode);

        public void ResetSourceSide()
        {
            foreach (var n in SourceNodes)
                n.Reset();
        }
        public void ResetSinkSide()
        {
            foreach (var n in SinkNodes)
                n.Reset();
        }
        public void Reset()
        {
            this.ResetSinkSide();
            this.ResetSourceSide();
        }
        public void ChangeSide(Node n, bool sourceSide)
        {
            if (n.SourceSide && !sourceSide)
            {
                this.SourceNodes.Remove(n);
                this.SinkNodes.Add(n);
            }
            else if (!n.SourceSide && sourceSide)
            {
                this.SinkNodes.Remove(n);
                this.SourceNodes.Add(n);
            }
            n.SetSourceSide(sourceSide);
        }
    }
}