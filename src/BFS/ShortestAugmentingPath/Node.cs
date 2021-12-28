using System;
using System.Collections.Generic;
using System.Linq;

namespace BFS.ShortestAugmentingPath
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
    }
    public class Node
    {
        public int Distance { get; protected set; }
        public List<BiEdge> Edges { get; private set; }
        public string Name { get; private set; }
        public Node PreviousNode { get; private set; }
        public BiEdge PreviousEdge { get; private set; }
        public Node(string name)
        {
            this.Name = name;
            this.Edges = new List<BiEdge>();
            this.Distance = -1;
            this.PreviousEdge = null;
            this.PreviousNode = null;
        }

        public void AddEdge(Node nextNode, int cap)
        {
            BiEdge edge = new BiEdge(this, nextNode, cap);
            this.Edges.Add(edge);
            nextNode.AddEdge(edge);
        }
        public void AddEdge(BiEdge edge)
        {
            this.Edges.Add(edge);
        }
        public void AddEdge(params (Node, int)[] edges)
        {
            foreach (var x in edges)
                AddEdge(x.Item1, x.Item2);
        }
        public void SetPrevious(BiEdge edge)
        {
            this.PreviousEdge = edge;
            if (edge.PreviousNode == this)
                this.PreviousNode = edge.NextNode;
            else
                this.PreviousNode = edge.PreviousNode;
        }
        public void SetDistance(int d)
        {
            this.Distance = d;
        }
        public void AddFlow(int flow, Node n)
        {
            BiEdge edge = this.Edges.Single(x => x.NextNode == n || x.PreviousNode == n);
            AddFlow(flow, edge);
        }
        public void AddFlow(int flow, BiEdge edge)
        {
            int f, c;
            if (edge.Reversed)
            {
                f = edge.Flow - flow;
                c = edge.Capacity + flow;
            }
            else
            {
                f = edge.Flow + flow;
                c = edge.Capacity - flow;
            }
#if DEBUG
            if (c < 0 || f < 0)
                throw new ArgumentException("valore di flusso non valido");
#endif
            edge.SetCapacity(c);
            edge.SetFlow(f);

        }


    }

}