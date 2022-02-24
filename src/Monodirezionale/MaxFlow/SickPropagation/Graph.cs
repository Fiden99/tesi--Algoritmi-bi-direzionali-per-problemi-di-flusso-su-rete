using System;
using System.Collections.Generic;
using System.Linq;

namespace Monodirezionale.MaxFlow.SickPropagation
{
    public class Graph
    {
        public List<HashSet<Node>> LabeledNodes { get; private set; }
        //TODO da capire se deve tenere InvalidNodes
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
        //metodo che consente in fase iniziale di inserire un nodo all'interno del grafo
        public void AddNode(Node node)
        {
            this.LabeledNodes[0].Add(node);
        }
        public Node Source => LabeledNodes[0].Single(x => x is SourceNode);
        public Node Sink
        {
            get
            {
                LabeledNodes.RemoveAll(x => x.Count == 0);
                Node sink = this.LabeledNodes.Last().SingleOrDefault(x => x is SinkNode);
                if (sink is null)
                    sink = this.InvalidNodes.OfType<SinkNode>().Single();
                return sink;
            }
        }
        public static void Reset(IEnumerable<Node> values)
        {
            foreach (var n in values)
            {
                Reset(n);
            }
        }
        //reset inFlow and PreviousNode for each node with label >= designated label
        public void Reset(int label)
        {
            for (int i = label; i < LabeledNodes.Count; i++)
            {
                foreach (var x in LabeledNodes[i])
                    Reset(x);
            }
        }
        public static void Reset(Node n)
        {
            //n.SetPreviousNode(null);
            if (n is not SourceNode)
                n.SetVisited(false);
        }
        public void ChangeLabel(Node node, int to)
        {
            if (node.Label == to)
                return;
            if (!this.LabeledNodes[node.Label].Remove(node))
                throw new ArgumentException("nodo non presente nella label indicata");
            while (this.LabeledNodes.Count <= to)
                this.LabeledNodes.Add(new HashSet<Node>());
            if (!this.LabeledNodes[to].Add(node))
                throw new ArgumentException("nodo già presente nella label indicata");
            node.SetLabel(to);
        }

        public void InvalidNode(Node node)
        {

            if (node.Valid == false)
                return;
            if (!this.LabeledNodes[node.Label].Remove(node))
                throw new ArgumentException("nodo non presente nella label indicata");
            if (!this.InvalidNodes.Add(node))
                throw new ArgumentException("nodo già inserito già inserito in invalidNode");
            node.SetVisited(false);
            //node.SetPreviousNode(null);
            //node.SetPreviousEdge(null);
            node.SetValid(false);
        }

        public void RepairNode(Node node, int label)
        {
            if (node.Valid == true)
                throw new ArgumentException("nodo già valido");
            if (!this.InvalidNodes.Remove(node))
                throw new ArgumentException("impossibile trovare il nodo indicato in InvalidNodes");
            while (this.LabeledNodes.Count <= label)
                this.LabeledNodes.Add(new HashSet<Node>());
            if (!this.LabeledNodes[label].Add(node))
                throw new ArgumentException("nodo già presente nella label indicata");
            node.SetValid(true);
            node.SetLabel(label);
        }


    }
}