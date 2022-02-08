using System;
using System.Collections.Generic;

namespace Bidirezionale.NodeCount.NoOpt
{

    public class BiEdge
    {
        public Node NextNode { get; private set; }
        public Node PreviousNode { get; private set; }
        public int Flow { get; private set; }
        public int Capacity { get; private set; }
        public bool Reversed { get; private set; }
        public BiEdge(Node from, Node to, int capacity)
        {
            this.PreviousNode = from;
            this.NextNode = to;
            this.Capacity = capacity;
            this.Flow = 0;
            this.Reversed = false;

        }
        public void SetFlow(int flow)
        {
            this.Flow = flow;
        }
        public void SetCapacity(int capacity)
        {
            this.Capacity = capacity;
        }
        public void SetReversed(bool reversed)
        {
            this.Reversed = reversed;
        }
        public bool AddFlow(int flow)
        {
            int f, c;
            if (this.Reversed == false)
            {
                f = this.Flow + flow;
                c = this.Capacity - flow;
            }
            else
            {
                f = this.Flow - flow;
                c = this.Capacity + flow;
            }
#if DEBUG
            if (f < 0 || c < 0)
                throw new ArgumentException("valore di flusso non valido");
#endif
            this.SetCapacity(c);
            this.SetFlow(f);
            return c == 0;
        }
    }


    public class Node
    {
        public bool Visited { get; protected set; }
        public int Label { get; protected set; }
        public List<BiEdge> Edges { get; private set; }
        public string Name { get; private set; }
        public Node PreviousNode { get; private set; }
        public Node NextNode { get; private set; }
        public BiEdge NextEdge { get; private set; }
        public BiEdge PreviousEdge { get; private set; }
        public bool SourceSide { get; protected set; }

        public Node(string name)
        {
            this.Name = name;
            this.Visited = false;
            this.Label = 0;
            this.Edges = new();
            this.PreviousNode = null;
            this.NextNode = null;
            this.PreviousEdge = null;
            this.NextEdge = null;
            this.SourceSide = true;
        }
        public void AddEdge(Node node, int cap)
        {
            var e = new BiEdge(this, node, cap);
            this.Edges.Add(e);
            node.AddEdge(e);
        }
        public void AddEdge(BiEdge edge) => this.Edges.Add(edge);
        public void AddEdge(params (Node, int)[] edges)
        {
            foreach (var x in edges)
                this.AddEdge(x.Item1, x.Item2);
        }
        public void SetLabel(int label) => this.Label = label;
        public void SetVisited(bool visited) => this.Visited = visited;
        public void SetPreviousNode(Node n) => this.PreviousNode = n;
        public void SetNextNode(Node n) => this.NextNode = n;
        public void SetPreviousEdge(BiEdge e) => this.PreviousEdge = e;
        public void SetNextEdge(BiEdge e) => this.NextEdge = e;
        public void SetSourceSide(bool sourceSide) => this.SourceSide = sourceSide;

        public static bool AddFlow(Node n, int flow)
        {
            if (n.NextEdge is not null && n.PreviousEdge is not null)
            {
                bool x = n.NextEdge.AddFlow(flow);
                return x & n.PreviousEdge.AddFlow(flow);
            }
            else
                return ((n.NextEdge is not null) ? n.NextEdge : n.PreviousEdge).AddFlow(flow);
        }
        public virtual void Reset()
        {
            this.SetVisited(false);
            this.SetPreviousEdge(null);
            this.SetPreviousNode(null);
            this.SetNextEdge(null);
            this.SetNextNode(null);
        }
    }
}