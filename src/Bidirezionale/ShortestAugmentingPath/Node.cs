using System;
using System.Collections.Generic;

namespace Bidirezionale.ShortestAugmentingPath
{
    public class BiEdge
    {
        public Node NextNode { get; private set; }
        public Node PreviousNode { get; private set; }
        public int Flow { get; private set; }
        public int Capacity { get; private set; }
        public bool Reversed { get; private set; }
        public BiEdge(Node previous, Node next, int cap)
        {
            this.NextNode = next;
            this.PreviousNode = previous;
            this.Capacity = cap;
            this.Flow = 0;
            this.Reversed = false;
        }
        public void SetFlow(int flow)
        {
            this.Flow = flow;
        }
        public void SetCapacity(int cap)
        {
            this.Capacity = cap;
        }
        public void SetReversed(bool rev)
        {
            this.Reversed = rev;
        }

        public bool AddFlow(int flow)
        {
            int f = this.Reversed ? (this.Flow - flow) : (this.Flow + flow);
            int c = this.Reversed ? (this.Capacity + flow) : (this.Capacity - flow);
            if (f < 0 || c < 0)
                throw new ArgumentException("valore di flusso non valido");
            this.Capacity = c;
            this.Flow = f;
            return c == 0;
        }
    }

    public class Node
    {
        public int SourceDistance { get; protected set; }
        public int SinkDistance { get; protected set; }
        public string Name { get; private set; }
        public Node PreviousNode { get; private set; }
        public BiEdge PreviousEdge { get; private set; }
        public Node NextNode { get; private set; }
        public BiEdge NextEdge { get; private set; }
        public List<BiEdge> Edges { get; private set; }
        public Node(string name)
        {
            this.SourceDistance = -1;
            this.SinkDistance = -1;
            this.Name = name;
            this.PreviousNode = null;
            this.PreviousEdge = null;
            this.NextNode = null;
            this.NextEdge = null;
            this.Edges = new();
        }

        public void AddEdge(Node nextNode, int cap)
        {
            BiEdge edge = new(this, nextNode, cap);
            this.Edges.Add(edge);
            nextNode.AddEdge(edge);
        }
        public void AddEdge(BiEdge edge) => this.Edges.Add(edge);
        public void AddEdge(params (Node, int)[] edges)
        {
            foreach (var x in edges)
                this.AddEdge(x.Item1, x.Item2);
        }
        public void SetPreviousEdge(BiEdge e) => this.PreviousEdge = e;
        public void SetPreviousNode(Node n) => this.PreviousNode = n;
        public void SetPrevious(BiEdge e)
        {
            if (e.PreviousNode == this)
                this.PreviousNode = e.NextNode;
            else
                this.PreviousNode = e.PreviousNode;
            this.PreviousEdge = e;
        }

        public void SetNextEdge(BiEdge e) => this.NextEdge = e;
        public void SetNextNode(Node n) => this.NextNode = n;
        public void SetNext(BiEdge e)
        {
            this.NextEdge = e;
            this.NextNode = e.NextNode == this ? e.PreviousNode : e.NextNode;
        }
        public void SetSourceDistance(int d) => this.SourceDistance = d;
        public void SetSinkDistance(int d) => this.SinkDistance = d;

        public void AddFlow(int f)
        {
            if (this.NextEdge != null)
                this.NextEdge.AddFlow(f);
            if (this.PreviousEdge != null)
                this.PreviousEdge.AddFlow(f);
        }
        public void Reset()
        {
            this.PreviousEdge = null;
            this.PreviousNode = null;
            this.NextEdge = null;
            this.NextNode = null;
        }
    }
}