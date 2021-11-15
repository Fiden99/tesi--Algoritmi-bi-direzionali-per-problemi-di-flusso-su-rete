using System;
using System.Collections.Generic;
using System.Linq;

namespace LastLevelOpt
{

    public class BiEdge
    {
        public Node nextNode{get; private set;}
        public Node previousNode{get; private set;}
        public int flow {get; private set;}
        public int capacity{get;private set;}
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
        public void setCapacity( int capacity)
        {
            this.capacity = capacity;
        }
    }

 
    public class Node
    {
        public string name {get; private set;}
        public int inFlow {get;private set;}
        public List<BiEdge> edges {get;private set;}
        public Node previousNode {get; private set;}

        public Node (string name)
        {
            this.name = name;
            this.edges = new List<BiEdge>();
            this.inFlow = 0;
            this.previousNode = null;
        }

        public void addEdge(Node node, int cap)
        {
            BiEdge edge = new BiEdge(this,node,cap);
            this.edges.Add(edge);
            node.addEdge(edge);

        }
        public void addEdge(BiEdge edge)
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
            if( f < 0 || c < 0 )
                throw new ArgumentException("valore di flusso non valido");
            this.inFlow += flow;
            edge.setCapacity(c);
            edge.setFlow(f);
        }
        


    }
}