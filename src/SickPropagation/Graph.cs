using System.Collections.Generic;
using System.Linq;
using System;
using BFS.LastLevelOpt;

namespace BFS.SickPropagation
{
    public class Graph
    {
        public List<HashSet<Node>> labeledNodes { get; private set; }
        //TODO da capire se si può tenere invalidNodes come set o meno
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
        //metodo che consente in fase iniziale di inserire un nodo all'interno del grafo
        public void AddNode(Node node)
        {
            this.labeledNodes[0].Add(node);
        }
        public Node Source => labeledNodes[0].Single(x => x is SourceNode);
        public Node Sink
        {
            get
            {
                Node sink = this.labeledNodes.Last().SingleOrDefault(x => x is SinkNode);
                if (sink is null)
                    sink = this.invalidNodes.OfType<SinkNode>().SingleOrDefault();
                if (sink is null)
                    throw new InvalidOperationException();
                return sink;
            }
        }
        public void Reset(IEnumerable<Node> values)
        {
            foreach (var n in values)
            {
                n.setPreviousNode(null);
                n.setInFlow(0);
            }
        }
        //reset inFlow and PreviousNode for each node with label >= designated label
        public void Reset(int label)
        {
            for (int i = label; i < labeledNodes.Count; i++)
            {
                foreach (var x in labeledNodes[i])
                    Reset(x);
            }
        }
        public void Reset(Node n)
        {
            n.setPreviousNode(null);
            if (n is SourceNode)
                n.setInFlow(int.MaxValue - n.edges.Sum(x => x.capacity));
            else
                n.setInFlow(0);
        }
        public void ChangeLabel(Node node, int to)
        {
            if (node.label == to)
                return;
            if (!this.labeledNodes[node.label].Remove(node))
                throw new ArgumentException();
            while (this.labeledNodes.Count <= to)
                this.labeledNodes.Add(new HashSet<Node>());
            if (!this.labeledNodes[to].Add(node))
                throw new ArgumentException();
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
            node.setValid(false);
        }


    }
}