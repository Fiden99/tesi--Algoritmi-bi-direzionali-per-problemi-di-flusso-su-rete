using System;
using System.Collections.Generic;
using System.Linq;

namespace BFS.SickPropagationGraphOpt
{
    public class Graph
    {
        public List<HashSet<Node>> labeledNodes { get; private set; }
        public HashSet<Node> invalidNodes { get; private set; }

        public Graph()
        {
            this.labeledNodes = new List<HashSet<Node>>();
            this.labeledNodes.Add(new HashSet<Node>());
            this.invalidNodes = new HashSet<Node>();
        }
        public Graph(Node node)
        {
            this.labeledNodes = new List<HashSet<Node>>();
            this.labeledNodes.Add(new HashSet<Node>(new Node[] { node }));
            this.invalidNodes = new HashSet<Node>();
        }
        public Graph(params Node[] nodes)
        {
            this.labeledNodes = new List<HashSet<Node>>();
            this.labeledNodes.Add(new HashSet<Node>(nodes));
            this.invalidNodes = new HashSet<Node>();
        }

        public void AddNode(Node node)
        {
            this.labeledNodes.First().Add(node);
        }
        public Node Source => labeledNodes.First().Single(x => x is SourceNode);
        public Node Sink
        {
            get
            {
                Node sink = this.labeledNodes.Last().SingleOrDefault(x => x is SinkNode);
                if (sink is null)
                    sink = this.invalidNodes.Single(x => x is SinkNode);
                return sink;
            }
        }
        public void Reset(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                Reset(node);
            }
        }
        public void Reset(Node node)
        {
            node.SetPreviousNode(null);
            if (node is not SourceNode)
                node.SetInFlow(0);
            else
                //TODO da capire se va bene solo int.MaxValue o devo sottrarre le capacità
                node.SetInFlow(int.MaxValue);
            //TODO da capire come vanno trattato previousLabelNodes

        }
        public void Reset(int label)
        {
            for (int i = label; i < this.labeledNodes.Count; i++)
            {
                Reset(this.labeledNodes[i]);
            }
        }

        //TODO da testare se funziona come voglio
        public void ChangeLabel(Node node, int to)
        {
            while (this.labeledNodes.Count <= to)
                this.labeledNodes.Add(new HashSet<Node>());
            if (node.label == to)
                return;
            if (!this.labeledNodes[node.label].Remove(node))
                throw new ArgumentException();
            if (!this.labeledNodes[to].Add(node))
                throw new ArgumentException();
            //TODO capire se è migliorabile
            node.ResetPreviousNextLabelNodes();
            //pulisce i previous e i next degli altri nodi a lui collegato
            if (node.label != 0)
            {
                foreach (var n in this.labeledNodes[node.label - 1])
                {
                    n.RemoveNextLabelNode(node);
                }
                foreach (var n in this.labeledNodes[node.label + 1])
                {
                    n.RemovePreviousLabelNode(node);
                }
            }
            //aggiunge i nodi necessari
            foreach (var e in node.edges)
            {
                if (e.nextNode == node && e.previousNode.label == (to - 1))
                {
                    node.AddPreviousLabelNode(e.previousNode);
                    e.previousNode.AddNextLabelNode(node);
                }
                if (e.previousNode == node && e.nextNode.label == (to + 1))
                {
                    node.AddNextLabelNode(e.nextNode);
                    e.previousNode.AddPreviousLabelNode(node);
                }
            }
            node.SetLabel(to);
        }
        public void InvalidNode(Node node)
        {
            if (node.valid == false)
                return;
            if (!this.labeledNodes[node.label].Remove(node))
                throw new ArgumentException();
            if (this.invalidNodes.Contains(node))
                throw new ArgumentException();
            this.invalidNodes.Add(node);
            node.SetValid(false);
            node.ResetPreviousNextLabelNodes();
            //TODO da valutare se si deve fare il reset di NextLabel e PreviousLabel
        }

    }
}