
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
            foreach (var e in node.Edges.Where(x => x.NextNode == node && x.Capacity > 0))
            {
                Node previous = e.PreviousNode;
                //TODO riguardare questa parte
                if (previous.Label == (node.Label - 1) && previous.Valid == true)
                {
                    node.SetInFlow(Math.Min(previous.InFlow, e.Capacity));
                    node.SetPreviousNode(previous);
                    grafo.ChangeLabel(previous, node.Label - 1);
                    return true;
                }
            }
            grafo.InvalidNode(node);
            return false;
        }
        public static int DoBfs(Graph grafo)
        {
            Queue<Node> coda;
            if (grafo.InvalidNodes.Count == 0)
            {
                grafo.Reset(0);
                coda = new Queue<Node>();
                coda.Enqueue(grafo.Source);
            }
            else
            {
                var min = grafo.InvalidNodes.Min(x => x.Label);
                coda = new Queue<Node>(grafo.LabeledNodes[min - 1]);
                grafo.Reset(min);

            }
            while (coda.Count > 0)
            {
                var element = coda.Dequeue();
                foreach (var edge in element.Edges.Where(x => x.PreviousNode == element))
                {
                    var n = edge.NextNode;
                    if (n.InFlow != 0)
                        continue;
                    if (edge.Capacity < 0)
                        throw new InvalidOperationException();
                    if (edge.Capacity == 0 && n.Valid == true)
                    {//capire se si può fare in maniera molto migliore
                        if (!Repair(grafo, n))
                        {
                            Queue<Node> malati = new Queue<Node>();
                            malati.Enqueue(n);
                            while (malati.Count > 0)
                            {
                                Node z = malati.Dequeue();
                                foreach (var x in z.Edges.Where(x => x.PreviousNode == z).Select(x => x.NextNode))
                                {
                                    if (!Repair(grafo, x))
                                    {
                                        //Graph.Reset(x);
                                        malati.Enqueue(x);
                                    }
                                    else if (x is SinkNode && x.InFlow != 0)
                                        return x.InFlow;
                                    else
                                        coda.Enqueue(x);
                                }
                            }
                        }
                        else
                        {
                            if (n is SinkNode && n.InFlow != 0)
                                return n.InFlow;
                            else
                                coda.Enqueue(n);
                        }

                    }
                    //if (element.Valid == true && edge.Capacity > 0 && (n.Label == 0 || n.Label > element.Label))
                    if (element.Valid == true && edge.Capacity > 0 && n.InFlow == 0)
                    {
                        n.SetPreviousNode(element);
                        if (n.Valid == true)
                            grafo.ChangeLabel(n, element.Label + 1);
                        else
                            grafo.RepairNode(n, element.Label + 1);
                        n.SetInFlow(Math.Min(element.InFlow, edge.Capacity));
                        if (n is SinkNode && n.Valid == true && n.InFlow != 0)
                            return n.InFlow;
                        else
                            coda.Enqueue(n);
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
            int fMax = 0;
            Node t = graph.Sink;
            Node s = graph.Source;
            while (true)
            {
                int f = BfsSickPropagation.DoBfs(graph);
                if (f == 0)
                    break;
                Node mom = t;
                try
                {
                    while (mom != s)
                    {
                        mom.PreviousNode.AddFlow(f, mom);
                        mom = mom.PreviousNode;
                    }
                }
                catch (ArgumentException)
                {
                    PrintGraph(graph);
                    return fMax;
                }
                fMax += f;
            }
            PrintGraph(graph);
            return fMax;

        }
    }
}