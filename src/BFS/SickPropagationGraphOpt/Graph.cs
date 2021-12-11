using System;
using System.Collections.Generic;
using System.Linq;

namespace BFS.SickPropagationGraphOpt
{
    public class Graph
    {
        public List<HashSet<Node>> LabeledNodes { get; private set; }
        public HashSet<Node> InvalidNodes { get; private set; }

        public Graph()
        {
            this.LabeledNodes = new List<HashSet<Node>>();
            this.LabeledNodes.Add(new HashSet<Node>());
            this.InvalidNodes = new HashSet<Node>();
        }
        public Graph(Node node)
        {
            this.LabeledNodes = new List<HashSet<Node>>();
            this.LabeledNodes.Add(new HashSet<Node>(new Node[] { node }));
            this.InvalidNodes = new HashSet<Node>();
        }
        public Graph(params Node[] nodes)
        {
            this.LabeledNodes = new List<HashSet<Node>>();
            this.LabeledNodes.Add(new HashSet<Node>(nodes));
            this.InvalidNodes = new HashSet<Node>();
        }

        public void AddNode(Node node)
        {
            this.LabeledNodes.First().Add(node);
        }
        public Node Source => LabeledNodes.First().Single(x => x is SourceNode);
        public Node Sink
        {
            get
            {
                Node sink = this.LabeledNodes.Last().SingleOrDefault(x => x is SinkNode);
                if (sink is null)
                    sink = this.InvalidNodes.Single(x => x is SinkNode);
                return sink;
            }
        }
        public static void Reset(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                Reset(node);
            }
        }
        public static void Reset(Node node)
        {
            node.SetPreviousNode(null);
            node.ResetPreviousNextLabelNodes();
            if (node is not SourceNode)
                node.SetInFlow(0);
            else
                node.SetInFlow(int.MaxValue);

        }
        public void Reset(int label)
        {
            for (int i = label; i < this.LabeledNodes.Count; i++)
            {
                Reset(this.LabeledNodes[i]);
            }
            if (label > 0)
                foreach (var node in this.LabeledNodes[label - 1])
                    node.NextLabelNodes.Clear();

        }


        public void RepairNode(Node node, int label)
        {
            if (!this.InvalidNodes.Remove(node))
                throw new ArgumentException("nodo non presente in InvalidNodes");
            while (this.LabeledNodes.Count <= label + 1)
                this.LabeledNodes.Add(new HashSet<Node>());
            if (!this.LabeledNodes[label].Add(node))
                throw new ArgumentException("nodo già presente nella label indicata");
            //non vado a resettare le label dato che dovrebbero essere già essere pulite
            foreach (var e in node.Edges)
            {
                Node next = e.NextNode;
                Node previous = e.PreviousNode;
                if (next == node && previous.Label == (label - 1) && previous.Valid == true)
                {
                    previous.AddNextLabelNode(node);
                    node.AddPreviousLabelNode(previous);
                }
                else if (previous == node && next.Label == (label + 1) && next.Valid == true)
                {
                    next.AddPreviousLabelNode(node);
                    node.AddNextLabelNode(next);
                }
                node.SetValid(true);
                node.SetLabel(label);
            }


        }

        //TODO da testare se funziona come voglio
        public void ChangeLabel(Node node, int to)
        {
            if (node.Label == to)
                return;
            while (this.LabeledNodes.Count <= to + 1)
                this.LabeledNodes.Add(new HashSet<Node>());
            if (!this.LabeledNodes[node.Label].Remove(node))
                throw new ArgumentException("nodo non presente nella label indicata");
            if (!this.LabeledNodes[to].Add(node))
                throw new ArgumentException("nodo già presente nella label indicata");
            //TODO capire se è migliorabile
            foreach (var n in node.NextLabelNodes)
                n.RemovePreviousLabelNode(node);
            foreach (var n in node.PreviousLabelNodes)
                n.RemoveNextLabelNode(node);
            node.ResetPreviousNextLabelNodes();
            //pulisce i previous e i next degli altri nodi a lui collegato
            /*  if (node.Label != 0)
                {
                    foreach (var n in this.LabeledNodes[node.Label - 1])
                    {
                        n.RemoveNextLabelNode(node);
                    }
                    foreach (var n in this.LabeledNodes[node.Label + 1])
                    {
                        n.RemovePreviousLabelNode(node);
                    }
                } */

            //aggiunge i nodi necessari
            foreach (var e in node.Edges)
            {
                if (e.NextNode == node && e.PreviousNode.Label == (to - 1))
                {
                    node.AddPreviousLabelNode(e.PreviousNode);
                    e.PreviousNode.AddNextLabelNode(node);
                }
                if (e.PreviousNode == node && e.NextNode.Label == (to + 1))
                {
                    node.AddNextLabelNode(e.NextNode);
                    e.PreviousNode.AddPreviousLabelNode(node);
                }
            }
            node.SetLabel(to);
        }
        public void InvalidNode(Node node)
        {
            if (node.Valid == false)
                return;
            if (!this.LabeledNodes[node.Label].Remove(node))
                throw new ArgumentException("nodo non presente nella label indicata");
            if (this.InvalidNodes.Contains(node))
                throw new ArgumentException("nodo già presente in InvalidNodes");
            this.InvalidNodes.Add(node);
            node.SetValid(false);
            //TODO da valutare se si deve fare il reset di NextLabel e PreviousLabel
            foreach (var n in node.PreviousLabelNodes)
            {
                n.RemoveNextLabelNode(node);
            }
            foreach (var n in node.NextLabelNodes)
            {
                n.RemovePreviousLabelNode(node);
            }
            node.ResetPreviousNextLabelNodes();
        }

    }
}