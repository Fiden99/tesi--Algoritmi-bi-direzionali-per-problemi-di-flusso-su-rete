using System;
using System.Collections.Generic;
using System.Linq;
namespace Monodirezionale.MaxFlow.LastLevelOpt
//TODO valutare se conviene comunque tenere il set InvalidNodes
{
    public class BfsLastLevelOpt
    {
        private static bool Repair(Graph grafo, Node node)
        {
            foreach (var e in node.Edges)
            {
                Node previous = e.PreviousNode;
                Node next = e.NextNode;
                if (node == next && e.Capacity > 0 && previous.Label == (node.Label - 1) && previous.Valid == true)
                {
                    grafo.ChangeLabel(node, node.Label);
                    node.SetPreviousNode(previous);
                    node.SetInFlow(Math.Min(e.Capacity, previous.InFlow));
                    e.SetReversed(false);
                    return true;
                }
                if (node == previous && e.Flow > 0 && next.Label == (node.Label - 1) && next.Valid == true)
                {
                    grafo.ChangeLabel(node, node.Label);
                    node.SetPreviousNode(next);
                    node.SetInFlow(Math.Min(e.Flow, next.InFlow));
                    e.SetReversed(true);
                    return true;
                }
            }
            grafo.InvalidNode(node);
            return false;
        }

        //TODO da capire se CorrectFlow deve arrivare fino a s oppure si può fermare prima

        public static int doBfs(Graph grafo, Node noCap)
        {

            Queue<Node> coda;
            if (noCap == null)
            {

                //grafo.ResetLabel(0);
                coda = new Queue<Node>();
                coda.Enqueue(grafo.Source);
            }
            else
            {
                Node t = grafo.Sink;
                if (Repair(grafo, noCap) && t.PreviousNode.InFlow != 0 && t.PreviousEdge.Capacity > 0)
                {
                    return Math.Min(t.InFlow, noCap.InFlow);
                }
                coda = new Queue<Node>(grafo.LabeledNode[noCap.Label - 1]);
                grafo.ResetLabel(noCap.Label);
            }
            while (coda.Count > 0)
            {
                var element = coda.Dequeue();
                foreach (BiEdge edge in element.Edges)
                {
                    Node n = edge.NextNode;
                    Node p = edge.PreviousNode;
                    if (edge.Capacity < 0)
                        throw new InvalidOperationException();
                    //bfs normale
                    if (element.Valid == true)
                    {
                        if (p == element && edge.Capacity > 0 && (n.InFlow == 0 || n.Valid == false))
                        {
                            if (n.Valid == true)
                                grafo.ChangeLabel(n, p.Label + 1);
                            else
                                grafo.RepairNode(n, p.Label + 1);
                            n.SetInFlow(Math.Min(p.InFlow, edge.Capacity));
                            n.SetPreviousNode(p);
                            n.SetPreviousEdge(edge);
                            edge.SetReversed(false);
                            if (n is SinkNode)
                                return n.InFlow;
                            coda.Enqueue(n);

                        }
                        else if (n == element && edge.Flow > 0 && (p.InFlow == 0 || p.Valid == false))
                        {
                            if (p.Valid == true)
                                grafo.ChangeLabel(p, n.Label + 1);
                            else
                                grafo.RepairNode(p, n.Label + 1);

                            p.SetInFlow(Math.Min(n.InFlow, edge.Flow));
                            p.SetPreviousNode(n);
                            p.SetPreviousEdge(edge);
                            edge.SetReversed(true);
                            if (p is SinkNode)
                                return p.InFlow;
                            coda.Enqueue(p);
                        }
                    }
                }
            }
            return 0;
        }

        public static void PrintGraph(Graph grafo)
        {
            foreach (var set in grafo.LabeledNode)
            {
                foreach (var node in set)
                {
                    Console.Write("node " + node.Name + " label = " + node.Label);
                    foreach (var x in node.Edges.Where(x => x.PreviousNode == node))
                        Console.Write(" to " + x.NextNode.Name + ", f = " + x.Flow + ", c  = " + x.Capacity + ";");
                    Console.WriteLine();
                }
            }
            foreach (var node in grafo.InvalidNodes)
            {
                Console.Write("node " + node.Name);
                foreach (var x in node.Edges.Where(x => x.PreviousNode == node))
                    Console.Write(" to " + x.NextNode.Name + ", f = " + x.Flow + ", c  = " + x.Capacity + ";");
                Console.WriteLine();

            }
        }
        public static int FlowFordFulkerson(Graph grafo)
        {
            Node vuoto = null;
            //int fMax = 0;
            var s = grafo.Source;
            var t = grafo.Sink;
            while (true)
            {
                int f;
                f = doBfs(grafo, vuoto);
                if (f == 0)
                    break;
                Node mom = t;
                while (mom != s)
                {
                    var x = mom.PreviousNode.AddFlow(f, mom.PreviousEdge);
                    if (x.Item1)
                        if (x.Item2)
                        {
                            vuoto = mom;
                            mom = t;
                            while (mom != vuoto)
                            {
                                mom.PreviousNode.AddFlow(-f, mom.PreviousEdge);
                                mom.SetValid(true);
                                mom.SetInFlow(mom.InFlow + f);
                                mom = mom.PreviousNode;
                            }
                            break;
                        }
                        else
                        {
                            vuoto = mom;
                        }
                    mom = mom.PreviousNode;

                }
                //fMax += f;
            }
            //PrintGraph(grafo);
            //Console.WriteLine("flusso inviato = " + (int.MaxValue - s.InFlow));
            return int.MaxValue - s.InFlow;
        }
    }
}