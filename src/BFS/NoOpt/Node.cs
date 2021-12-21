
using System;
using System.Collections.Generic;
using System.Linq;

namespace BFS.NoOpt
{
    public class MonoEdge
    {
        public Node NextNode { get; }
        public int Flow { get; private set; }
        public int Capacity { get; private set; }
        public MonoEdge(Node next, int cap)
        {
            this.Flow = 0;
            this.NextNode = next;
            this.Capacity = cap;
        }

        public void SetFlow(int flow)
        {
            this.Flow = flow;
        }

        public void SetCapacity(int cap)
        {
            this.Capacity = cap;
        }

    }

    public class ReversedMonoEdge : MonoEdge
    {
        public ReversedMonoEdge(Node n, int cap) : base(n, cap) { }
    }

    public class Node
    {
        public int InFlow { get; protected set; }
        public uint Label { get; private set; }
        public List<MonoEdge> Next { get; private set; }

        public bool Valid { get; private set; }

        public string Name { get; private set; }
        public Node PreviousNode { get; private set; }

        public Node(string name)
        {
            Valid = true;
            this.Label = 0;
            this.Next = new List<MonoEdge>();
            this.Name = name;
            this.InFlow = 0;
            this.PreviousNode = null;
        }
        /*        public Node(string name, params (Node, int)[] next)
        {
            this.Name = name;
            this.Valid = true;
            this.Label = 0;
            this.Next = new List<MonoEdge>(next.Select(x => new MonoEdge(x.Item1, x.Item2)));
            this.InFlow = 0;
            this.PreviousNode = null;


        }
        public Node(string name, params MonoEdge[] next)
        {
            Valid = true;
            this.Label = 0;
            this.Next = new List<MonoEdge>(next);
            this.Name = name;
            this.InFlow = 0;
            this.PreviousNode = null;
        }
        */

        public void AddNext(Node node, int cap)
        {
            this.Next.Add(new MonoEdge(node, cap));
            node.AddNext(new ReversedMonoEdge(this, cap));
        }
        public void AddNext(params (Node, int)[] nodes)
        {
            foreach (var x in nodes)
                this.AddNext(x.Item1, x.Item2);
        }

        public void AddNext(MonoEdge edge) => this.Next.Add(edge);

        //public void AddNext(IEnumerable<MonoEdge> edges) => this.Next.AddRange(edges);

        public void InitLabel(uint label)
        {
            this.Label = label;
        }
        public void RepairInValid(uint label)
        {
            this.Label = label;
            this.Valid = true;
        }
        public void InValidNode()
        {
            this.Valid = false;
        }
        //permette di modificare il flusso, se il flusso ha valore negativo aumento la capacità
        public void AddFlow(int flow, Node n)
        {
            MonoEdge edge = this.Next.Single(x => x.NextNode == n);
            MonoEdge rEdge = n.Next.Single(x => x.NextNode == this);
            int f, c;
            if (edge is ReversedMonoEdge)
            {
                f = edge.Flow - flow;
                c = edge.Capacity + flow;

            }
            else
            {
                f = edge.Flow + flow;
                c = edge.Capacity - flow;
            }
            if (f < 0 || c < 0)
                throw new ArgumentException("valore di flusso non Valido");
            edge.SetFlow(f);
            edge.SetCapacity(c);
            rEdge.SetCapacity(c);
            rEdge.SetFlow(f);
            this.SetInFlow(this.InFlow - flow);
        }
        public void SetInFlow(int x)
        {
            this.InFlow = x;
        }

        public void SetPreviousNode(Node n)
        {
            this.PreviousNode = n;
        }

    }
}


