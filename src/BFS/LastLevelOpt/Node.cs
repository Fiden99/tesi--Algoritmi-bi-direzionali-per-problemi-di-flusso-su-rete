using System;
using System.Collections.Generic;
using System.Linq;

namespace BFS.LastLevelOpt
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
    }


    public class Node
    {
        public bool Valid { get; private set; }
        public int Label { get; private set; }
        public string Name { get; private set; }
        public int InFlow { get; protected set; }
        public List<BiEdge> Edges { get; private set; }
        public Node PreviousNode { get; private set; }

        public Node(string name)
        {
            this.Name = name;
            this.Edges = new List<BiEdge>();
            this.InFlow = 0;
            this.PreviousNode = null;
            this.Label = 0;
            this.Valid = true;
        }

        public void AddEdge(Node node, int cap)
        {
            BiEdge edge = new BiEdge(this, node, cap);
            this.Edges.Add(edge);
            node.AddEdge(edge);
        }
        public void AddEdge(params (Node, int)[] edges)
        {
            foreach (var x in edges)
                AddEdge(x.Item1, x.Item2);
        }
        public void AddEdge(BiEdge edge)
        {
            this.Edges.Add(edge);
        }
        public void AddEdge(IEnumerable<BiEdge> edges)
        {
            this.Edges.AddRange(edges);
        }

        public void SetInFlow(int f)
        {
            this.InFlow = f;
        }

        public void SetPreviousNode(Node n)
        {
            this.PreviousNode = n;
        }

        public bool AddFlow(int flow, Node n)
        {
            //TODO ricordarsi di controllare se edge è reversed o meno
            BiEdge edge = this.Edges.Single(x => x.NextNode == n || x.PreviousNode == n);
            int f, c;
            if (edge.Reversed == false)
            {
                f = edge.Flow + flow;
                c = edge.Capacity - flow;
            }
            else
            {
                f = edge.Flow - flow;
                c = edge.Capacity + flow;
            }
            if (f < 0 || c < 0)
                throw new ArgumentException("valore di flusso non valido");
            edge.SetCapacity(c);
            edge.SetFlow(f);
            this.SetInFlow(this.InFlow - flow);
            return c == 0;

        }
        public void SetLabel(int label)
        {
            this.Label = label;
        }
        public void SetValid(bool valid)
        {
            this.Valid = valid;
        }

    }
}