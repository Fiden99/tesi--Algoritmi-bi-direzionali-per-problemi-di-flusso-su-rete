using System;
using System.Collections.Generic;
using System.Linq;
using BFS.Abstractions;

namespace BFS.SickPropagationGraphOpt
{
    public class BfsSickPropagationGraphOpt
    {
        public static bool Repair(Graph grafo, Node node, BiEdge edge)
        {
            node.RemovePreviousLabelNode(edge.PreviousNode);
            if (node.PreviousLabelNodes.Count != 0)
            {
                Node previous = node.PreviousLabelNodes.First();
                node.SetInFlow(Math.Min(previous.Edges.First(x => x.NextNode == node).Capacity, previous.InFlow));
                node.SetPreviousNode(previous);
                node.SetValid(true);
                //grafo.ChangeLabel(e.previousNode, node.label - 1);
                return true;
                /* TODO da capire se tenere questa condizione o meno
                    if(n.edges.Single(x => x.nextNode == node).capacity <=0)
                        throw new InvalidOperationException();
                */

            }
            return false;
        }
        public static int DoBfs(Graph grafo)
        {
            Queue<Node> coda;
            /*          if (grafo.InvalidNodes.Count == 0)
                        {
            */
            grafo.Reset(0);
            coda = new Queue<Node>();
            coda.Enqueue(grafo.Source);
            /*         }
                        else
                       {
                           var min = grafo.InvalidNodes.Min(x => x.Label);
                           coda = new Queue<Node>(grafo.LabeledNodes[min]);
                           grafo.Reset(min + 1);
                       }
                        */
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
                    if (edge.Capacity == 0 && !grafo.InvalidNodes.Contains(n))
                    {
                        if (!Repair(grafo, n, edge))
                        {
                            Queue<Node> malati = new Queue<Node>();
                            malati.Enqueue(n);
                            grafo.InvalidNode(n);
                            Graph.Reset(n);
                            while (malati.Count > 0)
                            {
                                Node p = malati.Dequeue();
                                foreach (var x in p.Edges.Where(x => x.PreviousNode == p))
                                {
                                    var next = x.NextNode;
                                    if (!Repair(grafo, next, x))
                                    {
                                        //Graph.Reset(next);
                                        grafo.InvalidNode(next);
                                        malati.Enqueue(next);
                                    }
                                    else if (next is SinkNode && next.InFlow != 0)
                                        return next.InFlow;
                                    else
                                        coda.Enqueue(next);
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
                    if (element.Valid == true && edge.Capacity > 0 && n.InFlow == 0)
                    {
                        n.SetPreviousNode(element);
                        if (n.Valid == true)
                            grafo.ChangeLabel(n, element.Label + 1);
                        else
                        {
                            grafo.Repair(n, element.Label + 1);
                        }
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
                        Console.Write(" to " + x.NextNode.Name + ", f = " + x.Flow + ", c  = " + x.Capacity + ";");
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
        public static int FlowFordFulkerson(Graph grafo)
        {
            int fMax = 0;
            var s = grafo.Source;
            var t = grafo.Sink;
            while (true)
            {
                int f = BfsSickPropagationGraphOpt.DoBfs(grafo);
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
            Console.WriteLine("flusso inviato = " + fMax);
            return fMax;
        }
    }
}