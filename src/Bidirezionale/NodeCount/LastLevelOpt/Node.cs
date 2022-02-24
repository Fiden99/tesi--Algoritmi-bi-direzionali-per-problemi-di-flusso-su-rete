using System.Collections.Generic;
using System.ComponentModel;

namespace Bidirezionale.NodeCount.LastLevelOpt
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
        public (bool, bool) AddFlow(int flow)
        {
            bool invalid = false;
            int f, c;
            if (!this.Reversed)
            {
                f = this.Flow + flow;
                c = this.Capacity - flow;
            }
            else
            {
                f = this.Flow - flow;
                c = this.Capacity + flow;
            }
            if (f < 0 || c < 0)
                invalid = true;
            this.SetCapacity(c);
            this.SetFlow(f);
            return (c == 0, invalid);
        }
    }


    public class Node
    {
        public int InFlow { get; protected set; }
        public int Label { get; protected set; }
        public List<BiEdge> Edges { get; private set; }
        public string Name { get; private set; }
        public Node PreviousNode { get; private set; }
        public Node NextNode { get; private set; }
        public BiEdge NextEdge { get; private set; }
        public BiEdge PreviousEdge { get; private set; }
        public bool SourceSide { get; protected set; }
        public bool Valid { get; private set; }
        public Node(string name)
        {
            this.Name = name;
            this.InFlow = 0;
            this.Label = 0;
            this.Edges = new();
            this.PreviousNode = null;
            this.NextNode = null;
            this.PreviousEdge = null;
            this.NextEdge = null;
            this.SourceSide = true;
            this.Valid = true;
        }
        public Node(string name, bool sourceSide)
        {
            this.Name = name;
            this.InFlow = 0;
            this.Label = 0;
            this.Edges = new();
            this.PreviousNode = null;
            this.NextNode = null;
            this.PreviousEdge = null;
            this.NextEdge = null;
            this.SourceSide = sourceSide;
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
        public void SetInFlow(int f) => this.InFlow = f;
        public void SetPreviousNode(Node n) => this.PreviousNode = n;
        public void SetNextNode(Node n) => this.NextNode = n;
        public void SetPreviousEdge(BiEdge e) => this.PreviousEdge = e;
        public void SetNextEdge(BiEdge e) => this.NextEdge = e;

        public static (bool, bool) AddFlow(Node n, int flow)
        {
            n.SetInFlow(n.InFlow - flow);
            if (n.NextEdge is not null && n.PreviousEdge is not null)
            {

                var x = n.NextEdge.AddFlow(flow);
                var y = n.PreviousEdge.AddFlow(flow);

                return (x.Item1 & y.Item1, y.Item2 | x.Item2);
            }
            else
                return ((n.NextEdge is not null) ? n.NextEdge : n.PreviousEdge).AddFlow(flow);
        }
        public virtual void Reset()
        {
            this.SetInFlow(0);
            this.SetPreviousEdge(null);
            this.SetPreviousNode(null);
            this.SetNextEdge(null);
            this.SetNextNode(null);
        }
        public void SetSourceSide(bool sourceside) => this.SourceSide = sourceside;
        public void SetValid(bool valid) => this.Valid = valid;
    }
}