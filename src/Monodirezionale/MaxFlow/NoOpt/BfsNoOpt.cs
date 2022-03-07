using System;
using System.Collections.Generic;


namespace Monodirezionale.MaxFlow.NoOpt
{
    public class BfsNoOpt
    {
        public static void DoBfs(Graph grafo)
        {
            grafo.ResetLabel();
            var coda = new Queue<Node>();
            coda.Enqueue(grafo.Source);
            while (coda.Count > 0)
            {
                var element = coda.Dequeue();
                foreach (MonoEdge e in element.Next)
                {
                    Node n = e.NextNode;
#if DEBUG
                    if (e.Capacity < 0)
                        throw new InvalidOperationException("capacità negativa");
#endif
                    if (n.InFlow == 0 && ((e.Capacity > 0 && e is not ReversedMonoEdge) || (e.Flow > 0 && e is ReversedMonoEdge)))
                    {
                        n.SetPreviousNode(element);
                        n.InitLabel(element.Label + 1);
                        n.SetInFlow(Math.Min(element.InFlow, (e is ReversedMonoEdge) ? e.Flow : e.Capacity));
                        if (n is SinkNode)
                            return;
                        else
                            coda.Enqueue(n);
                    }
                }
            }
            return;
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
            //int fMax = 0;
            var s = grafo.Source;
            var t = grafo.Sink;

            //aggiunti i nodi del grafo
            while (true)
            {
                //ricordati di cambiarlo quando testi un nuovo programma
                DoBfs(grafo);
                if (t.InFlow == 0)
                    break;
                //fMax += f;
                Node mom = t;
                while (mom != s)
                {
                    mom.PreviousNode.AddFlow(t.InFlow, mom);
                    mom = mom.PreviousNode;
                }
            }
            //PrintGraph(grafo);
            return int.MaxValue - s.InFlow;
        }
    }
}