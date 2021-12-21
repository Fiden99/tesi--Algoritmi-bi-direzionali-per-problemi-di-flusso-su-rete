
using System;
using System.Collections.Generic;
using System.Linq;
using BFS.LastLevelOpt;

namespace BFS.SickPropagation
{
    public class BfsSickPropagation
    {
        private static bool Repair(Graph grafo, Node node)
        {
            foreach (var e in node.Edges)
            {
                if (e.NextNode == node && e.Capacity > 0)
                {
                    Node previous = e.PreviousNode;
                    if (previous.Label == (node.Label - 1) && previous.Valid == true && previous.InFlow != 0)
                    {
                        node.SetInFlow(Math.Min(previous.InFlow, e.Capacity));
                        node.SetPreviousNode(previous);
                        e.SetReversed(false);
                        return true;
                    }
                    else if (e.PreviousNode == node && e.Flow > 0)
                    {
                        Node next = e.NextNode;
                        if (next.Label == (node.Label - 1) && next.Valid == true && next.InFlow != 0)
                        {
                            node.SetInFlow(Math.Min(next.InFlow, e.Flow));
                            node.SetPreviousNode(next);
                            e.SetReversed(true);
                            return true;
                        }
                    }
                }
            }
            grafo.InvalidNode(node);
            return false;
        }

        //TODO capire se CorrectFlow deve tornare indietro fino a s oppure si può fermare prima
        public static int CorrectFlow(Node node)
        {
            if (node.PreviousNode != null && node.InFlow > node.PreviousNode.InFlow)
                node.SetInFlow(CorrectFlow(node.PreviousNode));
            return node.InFlow;
        }

        public static int DoBfs(Graph grafo, Node noCap)
        {
            Queue<Node> coda;
            Queue<Node> malati = new Queue<Node>();
            //if (grafo.InvalidNodes.Count == 0)
            if (noCap is null)
            {
                //grafo.Reset(0);
                coda = new Queue<Node>();
                coda.Enqueue(grafo.Source);
            }
            else
            {
                Node t = grafo.Sink;
                if (Repair(grafo, noCap) && t.PreviousNode.InFlow != 0 && t.Edges.Single(x => x.PreviousNode == t.PreviousNode).Capacity > 0)
                {
                    return Math.Min(t.InFlow, noCap.InFlow);
                }

                coda = new Queue<Node>(grafo.LabeledNodes[noCap.Label - 1]);
                foreach (Node n in coda)
                    n.SetInFlow(CorrectFlow(n));
                grafo.Reset(noCap.Label);
                malati.Enqueue(noCap);
                while (malati.Count > 0)
                {
                    Node m = malati.Dequeue();
                    if (!Repair(grafo, m))
                        foreach (var e in m.Edges)
                        {
                            //TODO capire se devo tenere 
                            //e.NextNode == m.PreviousNode
                            //oppure 
                            //e.NextNode.Label == m.Label + 1
                            if (e.PreviousNode == m && e.NextNode == m.PreviousNode)
                                malati.Enqueue(e.NextNode);
                            else if (e.NextNode == m && e.PreviousNode == m.PreviousNode)
                                malati.Enqueue(e.PreviousNode);
                        }
                    else if (m is SinkNode)
                        return m.InFlow;
                    else
                        coda.Enqueue(m);
                }

            }
            while (coda.Count > 0)
            {
                var element = coda.Dequeue();
                foreach (var edge in element.Edges)
                {
                    var n = edge.NextNode;
                    var p = edge.PreviousNode;
                    if (n.InFlow != 0 && p.InFlow != 0)
                        continue;
                    if (edge.Capacity < 0)
                        throw new InvalidOperationException();
                    if (element.Valid == true)
                    {
                        if (p == element && edge.Capacity > 0 && n.InFlow == 0)
                        {
                            n.SetPreviousNode(p);
                            if (n.Valid == true)
                                grafo.ChangeLabel(n, p.Label + 1);
                            else
                                grafo.RepairNode(n, p.Label + 1);
                            n.SetInFlow(Math.Min(p.InFlow, edge.Capacity));
                            edge.SetReversed(false);
                            if (n is SinkNode)
                                return n.InFlow;
                            else
                                coda.Enqueue(n);
                        }
                        else if (n == element && edge.Flow > 0 && p.InFlow == 0)
                        {
                            p.SetPreviousNode(n);
                            if (p.Valid == true)
                                grafo.ChangeLabel(p, n.Label + 1);
                            else
                                grafo.RepairNode(p, n.Label + 1);
                            p.SetInFlow(Math.Min(n.InFlow, edge.Flow));
                            edge.SetReversed(true);
                            if (p is SinkNode)
                                return p.InFlow;
                            else
                                coda.Enqueue(p);
                        }
                    }
                }
            }
            return 0;
        }

        public static void PrintGraph(Graph grafo)
        {
            foreach (var set in grafo.LabeledNodes)
            {
                foreach (var node in set)
                {
                    Console.Write("node " + node.Name + " label = " + node.Label);
                    foreach (var x in node.Edges.Where(x => x.PreviousNode == node))
                        Console.Write(" to " + x.NextNode.Name + ", f = " + x.Flow + ", c = " + x.Capacity + ";");
                    Console.WriteLine();
                }
            }
            if (grafo.InvalidNodes.Count > 0)
                Console.WriteLine("nodi malati : ");
            foreach (var node in grafo.InvalidNodes)
            {
                Console.Write("node " + node.Name);
                foreach (var x in node.Edges.Where(x => x.PreviousNode == node))
                    Console.Write(" to " + x.NextNode.Name + ", f = " + x.Flow + ", c  = " + x.Capacity + ";");
                Console.WriteLine();
            }
        }
        public static int FlowFordFulkerson(Graph graph)
        {
            Node vuoto = null;
            int fMax = 0;
            Node t = graph.Sink;
            Node s = graph.Source;
            while (true)
            {
                int f = BfsSickPropagation.DoBfs(graph, vuoto);
                if (f == 0)
                    break;
                Node mom = t;
                /*                 try
                                { */
                while (mom != s)
                {
                    if (mom.PreviousNode.AddFlow(f, mom))
                        vuoto = mom;
                    mom = mom.PreviousNode;
                }
                /*                 }
                                catch (ArgumentException)
                                {
                                    PrintGraph(graph);
                                    return fMax;
                                } */
                fMax += f;
            }
            PrintGraph(graph);
            return fMax;

        }
    }
}