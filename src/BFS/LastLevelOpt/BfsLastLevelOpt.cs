using System;
using System.Collections.Generic;
using System.Linq;
namespace BFS.LastLevelOpt
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
                    grafo.RepairNode(node, node.Label);
                    node.SetPreviousNode(previous);
                    node.SetInFlow(Math.Min(e.Capacity, previous.InFlow));
                    e.SetReversed(false);
                    return true;
                }
                if (node == previous && e.Flow > 0 && next.Label == (node.Label - 1) && next.Valid == true)
                {
                    grafo.RepairNode(node, node.Label);
                    node.SetPreviousNode(next);
                    node.SetInFlow(Math.Min(e.Flow, next.InFlow));
                    e.SetReversed(true);
                    return true;
                }
            }
            return false;
        }

        public static int CorrectFlow(Node node)
        {
            if (node.PreviousNode != null && node.InFlow > node.PreviousNode.InFlow)
                node.SetInFlow(CorrectFlow(node.PreviousNode));
            return node.InFlow;
        }

        public static int DoBfs(Graph grafo, Node noCap)
        {

            Queue<Node> coda;
            if (noCap == null)
            {

                grafo.ResetLabel(0);
                coda = new Queue<Node>();
                coda.Enqueue(grafo.Source);

                /*                 if (grafo.InvalidNodes.Count == 0)
                                {
                                    grafo.ResetLabel(0);
                                    coda = new Queue<Node>();
                                    coda.Enqueue(grafo.Source);
                                }
                                else
                                {

                                    int x = grafo.InvalidNodes.Min(x => x.Label);
                                    coda = new Queue<Node>(grafo.LabeledNode[x - 1]);
                                    //grafo.ResetLabel(x);
                                    grafo.ResetLabel(x);

                                    //coda = new Queue<Node>(grafo.LabeledNode[startLabel - 1]);
                                    //grafo.ResetLabel(startLabel);
                                }
                 */
            }
            else
            {
                Node t = grafo.Sink;
                grafo.InvalidNode(noCap);
                if (Repair(grafo, noCap) && t.PreviousNode.InFlow != 0 && t.Edges.Single(x => x.PreviousNode == t.PreviousNode).Capacity > 0)
                {
                    return Math.Min(t.InFlow, noCap.InFlow);
                }
                coda = new Queue<Node>(grafo.LabeledNode[noCap.Label - 1]);
                grafo.ResetLabel(noCap.Label);
                foreach (Node n in coda)
                    n.SetInFlow(CorrectFlow(n));

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
                    //BFS normale
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
                f = DoBfs(grafo, vuoto);
                if (f == 0)
                    break;
                Node mom = t;
                while (mom != s)
                {

                    if (mom.PreviousNode.AddFlow(f, mom))
                        vuoto = mom;
                    mom = mom.PreviousNode;

                }
                //fMax += f;
            }
            PrintGraph(grafo);
            Console.WriteLine("flusso inviato = " + (int.MaxValue - s.InFlow));
            return int.MaxValue - s.InFlow;
        }
    }
}