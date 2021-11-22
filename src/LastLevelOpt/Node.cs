using System;
using System.Collections.Generic;
using System.Linq;

namespace BFS.LastLevelOpt
{

    public class BiEdge
    {
        public Node nextNode { get; private set; }
        public Node previousNode { get; private set; }
        public int flow { get; private set; }
        public int capacity { get; private set; }
        public BiEdge(Node from, Node to, int capacity)
        {
            this.previousNode = from;
            this.nextNode = to;
            this.capacity = capacity;
            this.flow = 0;
        }
        public void setFlow(int flow)
        {
            this.flow = flow;
        }
        public void setCapacity(int capacity)
        {
            this.capacity = capacity;
        }
    }


    public class Node : IEquatable<Node>
    {
        public bool valid { get; private set; }
        public int label { get; private set; }
        public string name { get; private set; }
        public int inFlow { get; protected set; }
        public List<BiEdge> edges { get; private set; }
        public Node previousNode { get; private set; }

        public Node(string name)
        {
            this.name = name;
            this.edges = new List<BiEdge>();
            this.inFlow = 0;
            this.previousNode = null;
            this.label = 0;
            this.valid = true;
        }

        public virtual void addEdge(Node node, int cap)
        {
            BiEdge edge = new BiEdge(this, node, cap);
            this.edges.Add(edge);
            node.addEdge(edge);

        }
        public void addEdge(params (Node, int)[] edges)
        {
            foreach (var x in edges)
                addEdge(x.Item1, x.Item2);
        }
        public virtual void addEdge(BiEdge edge)
        {
            this.edges.Add(edge);
        }
        public void addEdge(IEnumerable<BiEdge> edges)
        {
            this.edges.AddRange(edges);
        }

        public void setInFlow(int f)
        {
            this.inFlow = f;
        }

        public void setPreviousNode(Node n)
        {
            this.previousNode = n;
        }

        public void addFlow(int flow, Node n)
        {
            BiEdge edge = this.edges.SingleOrDefault(x => x.nextNode == n || x.previousNode == n);
            if (edge is null)
                throw new ArgumentException("nodo non trovato");
            int f = edge.flow + flow;
            int c = edge.capacity - flow;
            if (f < 0 || c < 0)
                throw new ArgumentException("valore di flusso non valido");
            this.inFlow += flow;
            edge.setCapacity(c);
            edge.setFlow(f);
        }
        public void SetLabel(int label)
        {
            this.label = label;
        }
        public void setValid(bool valid)
        {
            this.valid = valid;
        }

        public bool Equals(Node other)
        {
            return name == other.name;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
    }
}