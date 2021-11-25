using System;
using System.Collections.Generic;
using System.Linq;

namespace BFS.LastLevelOpt
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
                Node sink = this.InvalidNodes.SingleOrDefault(x => x is SinkNode);
                if (sink is null)
                    sink = this.LabeledNode.Last().SingleOrDefault(x => x is SinkNode);
                if (sink is null)
                    throw new InvalidOperationException();
                return sink;
            }
        }

        public void ResetLabel(int label)
        {
            for (int i = label; i < LabeledNode.Count; i++)
            {
                foreach (var n in LabeledNode[i])
                {//TODO da capire se posso tenere inFlow di sourceNode int.MaxValue o devo eliminare togliere per forza la capacità degli archi che partono da lui

                    if (n is SourceNode)
                        n.SetInFlow(int.MaxValue);
                    else

                        n.SetInFlow(0);
                    n.SetPreviousNode(null);
                }
            }
        }
        public static void ResetLabel(Node n)
        {
            n.SetPreviousNode(null);
            n.SetInFlow(0);

        }
        public void ChangeLabel(Node node, int to)
        {
            if (node.Label == to)
                return;
            if (!this.LabeledNode[node.Label].Remove(node))
                throw new ArgumentException();
            while (this.LabeledNode.Count <= to)
                this.LabeledNode.Add(new HashSet<Node>());
            if (!this.LabeledNode[to].Add(node))
                throw new ArgumentException();
            node.SetLabel(to);
        }

        public void ChangeLabel(Node node, int from, int to)
        {

            if (node.Label != from && !this.LabeledNode[from].Remove(node))
                throw new ArgumentException();
            this.LabeledNode[from].Remove(node);
            while (this.LabeledNode.Count <= to)
                this.LabeledNode.Add(new HashSet<Node>());
            if (!this.LabeledNode[to].Add(node))
                throw new ArgumentException();
            node.SetLabel(to);

        }
        public void InvalidNode(Node node)
        {
            bool removed = false;
            foreach (var set in this.LabeledNode)
            {
                if (set.Remove(node))
                {
                    removed = true;
                    break;
                }
            }
            if (!removed)
                throw new ArgumentException();
            if (!InvalidNodes.Add(node))
                throw new ArgumentException();
            node.SetValid(false);

        }
        public void InvalidNode(Node node, int from)
        {
            if (!this.LabeledNode[from].Remove(node))
                throw new ArgumentException();
            if (!this.InvalidNodes.Add(node))
                throw new ArgumentException();
            node.SetValid(false);
        }
        public void RepairNode(Node node, int to)
        {

            if (!this.InvalidNodes.Remove(node))
                throw new ArgumentException();
            while (this.LabeledNode.Count <= to)
                this.LabeledNode.Add(new HashSet<Node>());
            if (!this.LabeledNode[to].Add(node))
                throw new ArgumentException();
            node.SetLabel(to);
        }


    }
}