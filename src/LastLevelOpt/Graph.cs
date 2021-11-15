using System;
using System.Collections.Generic;
using System.Linq;

namespace LastLevelOpt
{
    public class Graph
    {
        public List<HashSet<Node>> labeledNode {get; private set;} 
        public HashSet<Node> invalidNode {get; private set;}

        //TODO capire se prima iterazione vanno inseriti in labeledNode o in invalidNode
        //considero a inserirli tutti in labeledNode
        public Graph()
        {
            this.labeledNode = new List<HashSet<Node>>();
            this.labeledNode.Add(new HashSet<Node>());
        }

        public Graph(Node node)
        {
            this.labeledNode = new List<HashSet<Node>>();
            this.labeledNode.Add(new HashSet<Node>(new Node[]{node}));
        }
        public Graph(params Node[] nodes)
        {
            this.labeledNode = new List<HashSet<Node>>();
            this.labeledNode.Add(new HashSet<Node>(nodes));

        }
        public void addNode(Node node)
        {
            this.labeledNode[0].Add(node);
        }
        public Node Source => labeledNode.First().SingleOrDefault( x => x is SourceNode);
        //TODO da capire come fare il source, cioè non deve essere una funzione
        public Node Sink {
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

        //TODO capire cosa deve fare 
        public void resetLabel(int label)
        {   
            for (int i = label; i<labeledNode.Count ; i++)
            {
            }
        }


    }
}