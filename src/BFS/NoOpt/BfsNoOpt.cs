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

        public static int FlowFordFulkerson(Graph grafo)
        {
            int fMax = 0;
            var s = grafo.Source;
            var t = grafo.Sink;

            //aggiunti i nodi del grafo
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
            return fMax;
        }
    }
}