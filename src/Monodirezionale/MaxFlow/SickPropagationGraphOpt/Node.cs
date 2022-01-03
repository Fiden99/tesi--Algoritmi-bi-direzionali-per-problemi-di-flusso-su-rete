using System;
using System.Collections.Generic;
using System.Linq;

namespace Monodirezionale.MaxFlow.SickPropagationGraphOpt
{
    public class BiEdge
    {
        public Node PreviousNode { get; private set; }
        public Node NextNode { get; private set; }
        public int Flow { get; private set; }
        public int Capacity { get; private set; }
        public bool Reversed { get; private set; }

        public BiEdge(Node previous, Node next, int capacity)
        {
            this.PreviousNode = previous;
            this.NextNode = next;
            this.Capacity = capacity;
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
            if (f < 0 || c < 0)
                throw new ArgumentException("flusso inviato non valido");
            this.Flow = f;
            this.Capacity = c;
            return c == 0;
        }


    }
    public class Node
    {
        public string Name { get; protected set; }
        public bool Valid { get; private set; }
        public int Label { get; private set; }
        public int InFlow { get; protected set; }
        public List<BiEdge> Edges { get; private set; }
        public Node PreviousNode { get; private set; }
        public List<Node> PreviousLabelNodes { get; private set; }
        public List<Node> NextLabelNodes { get; private set; }
        public BiEdge PreviousEdge { get; private set; }

        public Node(string name)
        {
            this.Name = name;
            this.Valid = true;
            this.Label = 0;
            this.Edges = new List<BiEdge>();
            this.PreviousLabelNodes = new List<Node>();
            this.PreviousNode = null;
            this.PreviousEdge = null;
            this.NextLabelNodes = new List<Node>();
        }
        public void AddEdge(BiEdge edge)
        {
            this.Edges.Add(edge);
        }
        public void AddEdge(Node node, int capacity)
        {
            BiEdge edge = new BiEdge(this, node, capacity);
            this.Edges.Add(edge);
            node.AddEdge(edge);
        }
        public void AddEdge(params (Node, int)[] edges)
        {
            foreach (var e in edges)
                this.AddEdge(e.Item1, e.Item2);
        }
        public void SetInFlow(int inFlow)
        {
            this.InFlow = inFlow;
        }
        public void SetPreviousNode(Node node)
        {
            this.PreviousNode = node;
        }
        public void SetPreviousEdge(BiEdge e)
        {
            this.PreviousEdge = e;
        }
        public void SetPrevious(BiEdge e)
        {
            this.PreviousEdge = e;
            if (this == e.PreviousNode)
                this.PreviousNode = e.NextNode;
            else
                this.PreviousNode = e.PreviousNode;
        }
        public bool AddFlow(int flow, Node node)
        {
            //TODO da valutare se il nodo deve essere solo next o va bene anche previous
            //TODO da capire se in caso di previous node si debba aggiungere la capacità e non il flusso

            BiEdge edge = this.Edges.Single(x => x.NextNode == node || x.PreviousNode == node);
            this.SetInFlow(this.InFlow - flow);
            return edge.AddFlow(flow);

        }
        public bool AddFlow(int flow, BiEdge edge)
        {
            //TODO da valutare se fare un controllo se BiEdge appartiene o meno a this.edges
#if DEBUG
            if (!this.Edges.Contains(edge))
                throw new ArgumentException("arco richiesto non contenuto in " + this.Name);
#endif
            this.SetInFlow(this.InFlow - flow);
            return edge.AddFlow(flow);
        }
        public void SetLabel(int label)
        {
            this.Label = label;
        }
        public void SetValid(bool valid)
        {
            this.Valid = valid;
        }
        public void AddPreviousLabelNode(Node node)
        {
            //TODO migliorare il codice in maniera tale che non debba controllare se è presente il nodo o meno
            if (!this.PreviousLabelNodes.Contains(node))
                // throw new ArgumentException("nodo " + node.Name + " già contenuto in previousLabelNodes di " + this.Name);
                this.PreviousLabelNodes.Add(node);
        }
        public void RemovePreviousLabelNode(Node node)
        {
            this.PreviousLabelNodes.Remove(node);
        }

        public void AddNextLabelNode(Node node)
        {
            //TODO migliorare il codice in maniera tale che non debba controllare se è presente il nodo o meno
            if (!this.NextLabelNodes.Contains(node))
                //throw new ArgumentException("nodo " + node.Name + " già contenuto in nextLabelNodes di " + this.Name);
                this.NextLabelNodes.Add(node);
        }
        public void RemoveNextLabelNode(Node node)
        {
            this.NextLabelNodes.Remove(node);
        }
        public void ResetPreviousNextLabelNodes()
        {
            this.PreviousLabelNodes.Clear();
            this.NextLabelNodes.Clear();
        }

    }
}