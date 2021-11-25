using System;
using System.Collections.Generic;
using BFS;
using BFS.Abstractions;

namespace BFS.NoOpt
{
    public class BfsNoOpt
    {
        public static int DoBfs(Graph grafo)
        {
            grafo.ResetLabel();
            var coda = new Queue<Node>();
            coda.Enqueue(grafo.Source);
            while (coda.Count > 0)
            {
                var element = coda.Dequeue();
                foreach (MonoEdge x in element.Next)
                {
                    Node n = x.NextNode;
                    if (x.Capacity < 0)
                        throw new InvalidOperationException("capacità negativa");

                    else if (n.PreviousNode == null && x.Capacity > 0)
                    {
                        n.SetPreviousNode(element);
                        n.InitLabel(element.Label + 1);
                        n.SetInFlow(Math.Min(element.InFlow, x.Capacity));
                        if (n is SinkNode)
                            return n.InFlow;
                        else
                            coda.Enqueue(n);
                    }
                }
            }
            return 0;
        }

        static void PrintGraph(Graph grafo)
        {
            foreach (var n in grafo.Nodes)
            {
                Console.Write("node " + n.Name + ", label = " + n.Label + " ");
                foreach (var x in n.Next)
                    Console.Write("to " + x.NextNode.Name + ", f= " + x.Flow + ",c = " + x.Capacity + ", ");
                Console.WriteLine();
            }
        }

        public static int FlowFordFulkerson()
        {
            int fMax = 0;
            //aggiunti i nodi del grafo
            SinkNode t = new SinkNode("t");
            Node n6 = new Node("6", (t, 10));
            Node n5 = new Node("5", (t, 35), (n6, 10));
            Node n4 = new Node("4", (n6, 25));
            Node n3 = new Node("3", (n4, 15), (n5, 15), (n6, 10));
            Node n2 = new Node("2", (n5, 35), (n3, 10));
            Node s = new SourceNode("s", (n2, 10), (n3, 30), (n4, 30));

            Graph grafo = new Graph(s, n2, n3, n4, n5, n6, t);
            while (true)
            {
                //ricordati di cambiarlo quando testi un nuovo programma
                int f = NoOpt.BfsNoOpt.DoBfs(grafo);
                if (f == 0)
                    break;
                fMax += f;
                Node mom = t;
                while (mom != s)
                {
                    mom.PreviousNode.AddFlow(f, mom);
                    mom = mom.PreviousNode;
                }

            }

            PrintGraph(grafo);
            Console.WriteLine("flusso totale inviato = " + fMax);
            return fMax;
        }
    }
}