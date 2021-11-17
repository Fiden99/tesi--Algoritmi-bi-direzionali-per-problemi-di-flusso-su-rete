using System;
using System.Collections.Generic;
using System.Linq;

namespace BFS.LastLevelOpt
{
    public class Graph
    {
        public List<HashSet<Node>> labeledNode { get; private set; }
        public HashSet<Node> invalidNode { get; private set; }

        public Graph()
        {
            this.labeledNode = new List<HashSet<Node>>();
            this.labeledNode.Add(new HashSet<Node>());
            this.invalidNode = new HashSet<Node>();
        }

        public Graph(Node node)
        {
            this.labeledNode = new List<HashSet<Node>>();
            this.labeledNode.Add(new HashSet<Node>(new Node[] { node }));
            this.invalidNode = new HashSet<Node>();

        }
        public Graph(params Node[] nodes)
        {
            this.labeledNode = new List<HashSet<Node>>();
            this.labeledNode.Add(new HashSet<Node>(nodes));
            this.invalidNode = new HashSet<Node>();

        }
        public void AddNode(Node node)
        {
            this.labeledNode[0].Add(node);
        }
        public Node Source => labeledNode.First().SingleOrDefault(x => x is SourceNode);
        public Node Sink
        {
            get
            {
                Node sink = this.invalidNode.SingleOrDefault(x => x is SinkNode);
                if (sink is null)
                    sink = this.labeledNode.Last().SingleOrDefault(x => x is SinkNode);
                if (sink is null)
                    throw new InvalidOperationException();
                return sink;
            }
        }

        public void ResetLabel(int label)
        {
            foreach (var set in labeledNode)
            {
                foreach (var n in set)
                {

                    if (n is SourceNode)
                        n.setInFlow(int.MaxValue - n.edges.Sum(x => x.capacity));
                    else
                    {
                        //n.SetLabel(0);
                        n.setPreviousNode(null);
                        n.setInFlow(0);
                    }
                }
            }
        }
        public void ResetLabel(Node n)
        {
            //n.SetLabel(0);
            n.setPreviousNode(null);
            n.setInFlow(0);

        }
        /*        public void ChangeLabel(Node node, int to)
                {
                    bool removed = false;
                    foreach(var set in this.labeledNode)
                    {
                        if( set.Remove(node))
                        {
                            removed = true;
                            break;
                        }
                    }
                    if (!removed)
                        throw new ArgumentException();
                    while(this.labeledNode.Count <= to)
                        this.labeledNode.Add(new HashSet<Node>());
                    if(!this.labeledNode[to].Add(node))
                        throw new ArgumentException();
                    node.SetLabel(to);

                }
        */
        public void ChangeLabel(Node node, int to)
        {
            if (node.label == to)
                return;
            if (!this.labeledNode[node.label].Remove(node))
                throw new ArgumentException();
            while (this.labeledNode.Count <= to)
                this.labeledNode.Add(new HashSet<Node>());
            if (!this.labeledNode[to].Add(node))
                throw new ArgumentException();
            node.SetLabel(to);
        }
        public void ChangeLabel(Node node, int from, int to)
        {

            if (node.label != from && !this.labeledNode[from].Remove(node))
                throw new ArgumentException();
            // capire come mai non mi viene rimossa dentro l'if
            this.labeledNode[from].Remove(node);
            while (this.labeledNode.Count <= to)
                this.labeledNode.Add(new HashSet<Node>());
            if (!this.labeledNode[to].Add(node))
                throw new ArgumentException();
            node.SetLabel(to);

        }
        public void InvalidNode(Node node)
        {
            bool removed = false;
            foreach (var set in this.labeledNode)
            {
                if (set.Remove(node))
                {
                    removed = true;
                    break;
                }
            }
            if (!removed)
                throw new ArgumentException();
            if (!invalidNode.Add(node))
                throw new ArgumentException();
            node.setValid(false);

        }
        public void InvalidNode(Node node, int from)
        {
            if (!this.labeledNode[from].Remove(node))
                throw new ArgumentException();
            if (!this.invalidNode.Add(node))
                throw new ArgumentException();
            node.setValid(false);
        }
        public void RepairNode(Node node, int to)
        {

            if (!this.invalidNode.Remove(node))
                throw new ArgumentException();
            while (this.labeledNode.Count <= to)
                this.labeledNode.Add(new HashSet<Node>());
            if (!this.labeledNode[to].Add(node))
                throw new ArgumentException();
            node.SetLabel(to);
        }


    }
}