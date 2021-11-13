
using System;
using System.Collections.Generic;
using System.Linq;

namespace BFS
{
    public class MonoEdge
    {
        public Node nextNode{get;}
        public int flow {get; private set;}
        public int capacity {get; private set;}
        public MonoEdge(Node next, int cap)
        {
            this.flow = 0;
            this.nextNode = next;
            this.capacity = cap;
        }
        
        public void setFlow(int flow)
        {
            this.flow = flow;
        }

        public void setCapacity(int cap)
        {
            this.capacity = cap;
        }

    }

    public class Node
    {
        public int inFlow {get; protected set;}
        public uint label{get; private set;}
        public List<MonoEdge> next {get; private set;}

        public bool valid{get; private set;}

        public string name {get; private set;}
        public Node previousNode {get; private set;}

        public Node(string name)
        {
            valid = true;
            this.label = 0;
            this.next = new List<MonoEdge>();
            this.name = name;
            this.inFlow = 0;
            this.previousNode = null;
        }
        public Node(string name, params (Node,int)[] next)
        {
            this.name = name;
            this.valid = true;
            this.label = 0;
            this.next = new List<MonoEdge>(next.Select(x => new MonoEdge(x.Item1, x.Item2)));
            this.inFlow = 0;
            this.previousNode = null;


        }
        public Node(string name,params MonoEdge[] next )
        {
            valid = true;
            this.label = 0;
            this.next = new List<MonoEdge>(next);
            this.name = name;
            this.inFlow = 0;
            this.previousNode = null;
        }

        public void addNext(Node node,int cap) => this.next.Add(new MonoEdge(node, cap));

        public void addNext(MonoEdge edge) => this.next.Add(edge);

        public void addNext(IEnumerable<MonoEdge> edges) => this.next.AddRange(edges);

        public void initLabel(uint label)
        {
            this.label = label;
        }
        public void repairInvalid(uint label)
        {
            this.label = label;
            this.valid = true;
        }
        public void invalidNode()
        {
            this.valid = false;
        }
        //permette di modificare il flusso, se il flusso ha valore negativo aumento la capacità
        public void addFlow(int flow, Node n)
        {
            MonoEdge edge = this.next.SingleOrDefault(x => x.nextNode == n);
            if (edge== null)
                throw new ArgumentException("nodo non trovato");
            int f = edge.flow + flow;
            int c = edge.capacity - flow;
            //TODO capire come risolvere il problema di overflow
            if (f < 0 || c < 0 )
                throw new ArgumentException("valore di flusso non valido");
            this.inFlow += flow;
            edge.setFlow(f);
            edge.setCapacity(c);
        }
        public void setInFlow(int x)
        {
            this.inFlow = x;
        }

        #if DEBUG

        public void removeNext (MonoEdge edge)
        {
            if (!this.next.Contains(edge))
                throw new ArgumentException();
            this.next.Remove(edge);
        }
        public void setPreviousNode(Node n)
        {
            this.previousNode = n;
        }
        #endif



    }
}


