using System;
using System.Collections.Generic;
using System.Linq;

namespace LastLevelOpt

{
    public class Program
    {

        public static void StampaGrafo(Graph grafo)
        {
            foreach (var set in grafo.labeledNode.Append(grafo.invalidNode))
            {
                foreach (var node in set)
                {
                    Console.Write("node " + node.name + " label = " + node.label);
                    foreach (var x in node.edges.Where(x => x.previousNode == node))
                        Console.Write(" to " + x.nextNode.name + ", f = " + x.flow + ", c  = " + x.capacity + ";");
                    Console.WriteLine();
                }
            }
        }
        static void Main(string[] args)
        {
            int fMax = 0;
            SinkNode t = new SinkNode("t");
            Node n6 = new Node("6");
            n6.addEdge(t, 10);
            Node n5 = new Node("5");
            n5.addEdge((t, 35), (n6, 10));
            Node n4 = new Node("4");
            n4.addEdge(n6, 25);
            Node n3 = new Node("3");
            n3.addEdge((n4, 15), (n5, 15), (n6, 10));
            Node n2 = new Node("2");
            n2.addEdge((n5, 35), (n3, 10));
            SourceNode s = new SourceNode("s");
            s.addEdge((n2, 10), (n3, 30), (n4, 30));
            Graph grafo = new Graph(s, n2, n3, n4, n5, n6, t);
            while (true)
            {
                int f = LastLevelOpt.doBfs(grafo);
                if (f == 0)
                    break;
                fMax += f;
                Node mom = t;
                while (mom != s)
                {
                    mom.previousNode.addFlow(f, mom);
                    mom = mom.previousNode;
                }


            }

            StampaGrafo(grafo);
            Console.WriteLine("flusso inviato = " + fMax);
        }
    }
}