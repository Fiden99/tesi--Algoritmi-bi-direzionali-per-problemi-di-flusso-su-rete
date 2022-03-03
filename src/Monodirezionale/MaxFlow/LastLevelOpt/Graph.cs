using System;
using System.Collections.Generic;
using System.Linq;

namespace Monodirezionale.MaxFlow.LastLevelOpt
{
    public class Graph
    {
        public List<HashSet<Node>> LabeledNode { get; private set; }
        public HashSet<Node> InvalidNodes { get; private set; }

        public Graph()
        {
            this.LabeledNode = new List<HashSet<Node>>();
            this.LabeledNode.Add(new HashSet<Node>());
            this.InvalidNodes = new HashSet<Node>();
        }

        public Graph(Node node)
        {
            this.LabeledNode = new List<HashSet<Node>>();
            this.LabeledNode.Add(new HashSet<Node>(new Node[] { node }));
            this.InvalidNodes = new HashSet<Node>();

        }
        public Graph(int cardNode)
        {
            this.LabeledNode = new List<HashSet<Node>>();
            this.LabeledNode.Add(new (cardNode));
            this.InvalidNodes = new HashSet<Node>();

        }
        public Graph(params Node[] nodes)
        {
            this.LabeledNode = new List<HashSet<Node>>();
            this.LabeledNode.Add(new HashSet<Node>(nodes));
            this.InvalidNodes = new HashSet<Node>();

        }
        //metodo che consente in fase iniziale di inserire un nodo all'interno del grafo
        public void AddNode(Node node)
        {
            this.LabeledNode[0].Add(node);
        }
        public Node Source => LabeledNode.First().Single(x => x is SourceNode);
        public Node Sink
        {
            get
            {
                Node sink = this.LabeledNode.Last().SingleOrDefault(x => x is SinkNode);
                if (sink is null)
                    sink = this.InvalidNodes.Single(x => x is SinkNode);
                return sink;
            }
        }

        public void ResetLabel(int label)
        {
            for (int i = label; i < LabeledNode.Count; i++)
            {
                foreach (Node n in LabeledNode[i])
                {
                    if (n is not SourceNode)
                        n.SetInFlow(0);
                    n.SetPreviousNode(null);
                    //this.ChangeLabel(n, 0);
                }
            }
        }
        public static void ResetLabel(Node n)
        {
            n.SetPreviousNode(null);
            n.SetInFlow(0);
            //this.ChangeLabel(n, 0);

        }
        public void ChangeLabel(Node node, int to)
        {
            if (node.Label == to)
                return;
            if (!this.LabeledNode[node.Label].Remove(node))
                throw new ArgumentException("nodo non trovato nella label selezionata");
            while (this.LabeledNode.Count <= to)
                this.LabeledNode.Add(new HashSet<Node>());
            if (!this.LabeledNode[to].Add(node))
                throw new ArgumentException("nodo già presente nella label selezionata");
            node.SetLabel(to);
        }

        public void InvalidNode(Node node)
        {
            if (node.Valid == false)
                return;
            if (!this.LabeledNode[node.Label].Remove(node))
                return;
            InvalidNodes.Add(node);
            node.SetValid(false);
        }
        public void RepairNode(Node node, int to)
        {
            if (!this.InvalidNodes.Remove(node))
                throw new ArgumentException("impossibile riparare il nodo, già assente in InvalidNodes");
            while (this.LabeledNode.Count <= to)
                this.LabeledNode.Add(new HashSet<Node>());
            if (!this.LabeledNode[to].Add(node))
                throw new ArgumentException("impossibile aggiungere il nodo indicato, già presente nella label indicata");
            node.SetLabel(to);
            node.SetValid(true);
        }


    }
}