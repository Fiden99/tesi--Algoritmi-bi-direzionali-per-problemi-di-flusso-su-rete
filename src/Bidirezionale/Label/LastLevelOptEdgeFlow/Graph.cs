using System;
using System.Collections.Generic;
using System.Linq;

namespace Bidirezionale.Label.LastLevelOptEdgeFlow
{
    public class Graph
    {
        public List<HashSet<Node>> LabeledNodeSourceSide { get; private set; }
        public List<HashSet<Node>> LabeledNodeSinkSide { get; private set; }
        public HashSet<Node> LastNodesSinkSide { get; private set; }
        public HashSet<Node> LastNodesSourceSide { get; private set; }


        public Graph()
        {
            this.LabeledNodeSourceSide = new();
            this.LabeledNodeSourceSide.Add(new());
            this.LastNodesSourceSide = new();
            this.LabeledNodeSinkSide = new();
            this.LabeledNodeSinkSide.Add(new());
            this.LastNodesSinkSide = new();
        }
        public Graph(params Node[] nodes)
        {
            this.LabeledNodeSourceSide = new();
            this.LabeledNodeSourceSide.Add(new());
            this.LastNodesSourceSide = new();

            this.LabeledNodeSinkSide = new();
            this.LabeledNodeSinkSide.Add(new());
            this.LastNodesSinkSide = new();

            foreach (var n in nodes)
                if (n.SourceSide)
                    this.LabeledNodeSourceSide[0].Add(n);
                else
                    this.LabeledNodeSinkSide[0].Add(n);
        }
        public void AddNode(Node n)
        {
            if (n.SourceSide)
                this.LabeledNodeSourceSide[0].Add(n);
            else
                this.LabeledNodeSinkSide[0].Add(n);
        }
        public void AddNode(params Node[] nodes)
        {
            foreach (var n in nodes)
                AddNode(n);
        }
        public Node Sink => this.LabeledNodeSinkSide[0].Single(x => x is SinkNode);
        public Node Source => this.LabeledNodeSourceSide[0].Single(x => x is SourceNode);
        public static void Reset(Node node)
        {
            /*if (node is not SourceNode && node is not SinkNode)
                node.SetInFlow(0);
                        node.SetPreviousEdge(null);
                        node.SetPreviousNode(null);
                        node.SetNextEdge(null);
                        node.SetNextNode(null); */
            node.Reset();
        }
        public void RemoveLastNode(Node n)
        {
#if DEBUG
            if (n.SourceSide)
            {
                if (!this.LastNodesSourceSide.Contains(n))
                    throw new ArgumentException("nodo non contenuto in lastSourceNode");
            }
            else
            {
                if (!this.LastNodesSinkSide.Contains(n))
                    throw new ArgumentException("nodo non contenuto in lastSinkNode");
            }
#endif
            if (n.SourceSide)
                this.LastNodesSourceSide.Remove(n);
            else
                this.LastNodesSinkSide.Remove(n);
        }
        public void ResetSourceSide(int label)
        {
            for (; label < this.LabeledNodeSourceSide.Count; label++)
                foreach (var n in this.LabeledNodeSourceSide[label])
                    Reset(n);
        }
        public void ResetSinkSide(int label)
        {
            for (; label < this.LabeledNodeSinkSide.Count; label++)
                foreach (var n in this.LabeledNodeSinkSide[label])
                    Reset(n);
        }
        public void ChangeLabel(Node n, bool sourceSide, int label)
        {
            if (n.Label == label && n.SourceSide == sourceSide)
                return;
#if DEBUG
            if (n.SourceSide)
                if (!this.LabeledNodeSourceSide[n.Label].Contains(n))
                    throw new ArgumentException("nodo non contenuto dove dichiarato");
            if (!n.SourceSide)
                if (!this.LabeledNodeSinkSide[n.Label].Contains(n))
                    throw new ArgumentException("nodo non contenuto dove dichiarato");
            if (sourceSide)
            {
                while (this.LabeledNodeSourceSide.Count <= label)
                    this.LabeledNodeSourceSide.Add(new());
                if (this.LabeledNodeSourceSide[label].Contains(n))
                    throw new ArgumentException("nodo già presente dove dichiarato");
            }
            if (!sourceSide)
            {
                while (this.LabeledNodeSinkSide.Count <= label)
                    this.LabeledNodeSinkSide.Add(new());
                if (this.LabeledNodeSinkSide[label].Contains(n))
                    throw new ArgumentException("nodo già presente dove dichiarato");
            }
#endif

            if (n.SourceSide)
                this.LabeledNodeSourceSide[n.Label].Remove(n);
            else
                this.LabeledNodeSinkSide[n.Label].Remove(n);
            if (sourceSide)
            {
                while (this.LabeledNodeSourceSide.Count <= label)
                    this.LabeledNodeSourceSide.Add(new());
                this.LabeledNodeSourceSide[label].Add(n);
            }
            else
            {
                while (this.LabeledNodeSinkSide.Count <= label)
                    this.LabeledNodeSinkSide.Add(new());
                this.LabeledNodeSinkSide[label].Add(n);
            }
            n.SetSourceSide(sourceSide);
            n.SetLabel(label);
        }
        public void AddLastSource(Node n)
        {
            this.LastNodesSourceSide.Add(n);
        }

        public void AddLastSink(Node n)
        {
            this.LastNodesSinkSide.Add(n);
        }
        public void AddLast(Node n)
        {
            if (!n.SourceSide)
            {
                if (LastNodesSinkSide.Add(n))
                {
                    foreach (var e in n.Edges)
                    {
                        if (e.PreviousNode.SourceSide)
                            LastNodesSourceSide.Add(e.PreviousNode);
                        else if (e.NextNode.SourceSide)
                            LastNodesSourceSide.Add(e.NextNode);
                    }
                }
            }
        }
    }
}