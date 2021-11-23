using System;
using System.Collections.Generic;
using System.Linq;

namespace BFS.SickPropagationGraphOpt
{
    public class BiEdge
    {
        public Node previousNode { get; private set; }
        public Node nextNode { get; private set; }
        public int flow { get; private set; }
        public int capacity { get; private set; }

        public BiEdge(Node previous, Node next, int capacity)
        {
            this.previousNode = previous;
            this.nextNode = next;
            this.capacity = capacity;
            this.flow = 0;
        }
        public void SetFlow(int flow)
        {
            this.flow = flow;
        }
        public void SetCapacity(int cap)
        {
            this.capacity = cap;
        }
        public void AddFlow(int flow)
        {
            int f = this.flow + flow;
            int c = this.capacity - flow;
            if (f < 0 || c < 0)
                throw new ArgumentException();
            this.flow = f;
            this.capacity = c;
        }


    }
    public class Node
    {
        public string name { get; protected set; }
        public bool valid { get; private set; }
        public int label { get; private set; }
        public int inFlow { get; protected set; }
        public List<BiEdge> edges { get; private set; }
        public Node previousNode { get; private set; }
        public List<Node> previousLabelNodes { get; private set; }
        public List<Node> nextLabelNodes { get; private set; }

        public Node(string name)
        {
            this.name = name;
            this.valid = true;
            this.label = 0;
            this.edges = new List<BiEdge>();
            this.previousLabelNodes = new List<Node>();
            this.previousNode = null;
            this.nextLabelNodes = new List<Node>();
        }
        public void AddEdge(BiEdge edge)
        {
            this.edges.Add(edge);
        }
        public void AddEdge(Node node, int capacity)
        {
            BiEdge edge = new BiEdge(this, node, capacity);
            this.edges.Add(edge);
            node.AddEdge(edge);
        }
        public void AddEdge(params (Node, int)[] edges)
        {
            foreach (var e in edges)
                this.AddEdge(e.Item1, e.Item2);
        }
        public void SetInFlow(int inFlow)
        {
            this.inFlow = inFlow;
        }
        public void SetPreviousNode(Node node)
        {
            this.previousNode = node;
        }
        public void AddFlow(int flow, Node node)
        {
            //TODO da valutare se il nodo deve essere solo next o va bene anche previous
            //TODO da capire se in caso di previous node si debba aggiungere la capacità e non il flusso

            BiEdge edge = this.edges.Single(x => x.nextNode == node);
            int f = edge.flow + flow;
            int c = edge.capacity - flow;
            if (c < 0 || f < 0)
                throw new ArgumentException();
            edge.SetCapacity(c);
            edge.SetFlow(f);
            this.inFlow += f;

        }
        public void AddFlow(int flow, BiEdge edge)
        {
            //TODO da valutare se fare un controllo se BiEdge appartiene o meno a this.edges
            if (!this.edges.Contains(edge))
                throw new ArgumentException();
            edge.AddFlow(flow);
            this.inFlow += flow;
        }
        public void SetLabel(int label)
        {
            this.label = label;
        }
        public void SetValid(bool valid)
        {
            this.valid = valid;
        }
        public void AddPreviousLabelNode(Node node)
        {
            //TODO da valutare se tenere if eccezione
            if (this.previousLabelNodes.Contains(node))
                throw new ArgumentException();
            this.previousLabelNodes.Add(node);
        }
        public void RemovePreviousLabelNode(Node node)
        {
            this.previousLabelNodes.Remove(node);
        }

        public void AddNextLabelNode(Node node)
        {
            //TODO da valutare se tenere if eccezione
            if (this.nextLabelNodes.Contains(node))
                throw new ArgumentException();
            this.nextLabelNodes.Add(node);
        }
        public void RemoveNextLabelNode(Node node)
        {
            this.nextLabelNodes.Remove(node);
        }
        public void ResetPreviousNextLabelNodes()
        {
            this.previousLabelNodes.Clear();
            this.nextLabelNodes.Clear();
        }

    }
}