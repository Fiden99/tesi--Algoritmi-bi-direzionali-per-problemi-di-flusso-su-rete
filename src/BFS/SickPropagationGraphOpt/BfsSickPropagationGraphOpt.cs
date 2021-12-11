using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BFS.Abstractions;

namespace BFS.SickPropagationGraphOpt
{
    public class BfsSickPropagationGraphOpt
    {
        //TODO valutare il fatto che dopo il reset non ci sono ho PreviousLabelNodes pulita
        // capire se durante il reset dobbiamo rimuovere o meno PreviousLabelNodes
        public static bool Repair(Node node)
        {
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
            if (noCap == null)
            {

                grafo.Reset(0);
                coda = new Queue<Node>();
                coda.Enqueue(grafo.Source);
            }
            else
            {
                coda = new Queue<Node>(grafo.LabeledNodes[noCap.Label - 1]);
                grafo.Reset(noCap.Label);
                foreach (Node n in coda)
                {
                    n.SetInFlow(CorrectFlow(n));
                }

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
                    {
                        malati.Enqueue(n);
                        do
                        {
                            Node x = malati.Dequeue();
                            if (!Repair(x))
                            {
                                grafo.InvalidNode(x);
                                foreach (var y in x.NextLabelNodes)
                                    malati.Enqueue(y);
                            }
                            else if (x is SinkNode && x.InFlow != 0)
                                return x.InFlow;
                            else
                                coda.Enqueue(x);

                        } while (malati.Count > 0);
                    }
                    if (element.Valid == true && edge.Capacity > 0 && n.InFlow == 0)
                    {
                        n.SetPreviousNode(element);
                        if (n.Valid == true)
                            grafo.ChangeLabel(n, element.Label + 1);
                        else
                        {
                            grafo.RepairNode(n, element.Label + 1);
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
            Node noCap = null;
            int fMax = 0;
            var s = grafo.Source;
            var t = grafo.Sink;
            while (true)
            {
                int f = BfsSickPropagationGraphOpt.DoBfs(grafo, noCap);
                if (f == 0)
                    break;
                Node mom = t;
                /*                 try
                                { */
                while (mom != s)
                {
                    if (mom.PreviousNode.AddFlow(f, mom))
                        noCap = mom;
                    mom = mom.PreviousNode;
                }

                /*                 }
                                catch (ArgumentException)
                                {

                                    PrintGraph(grafo);
                                    Console.WriteLine("flusso inviato = " + fMax);
                                    return fMax;
                                } */
                fMax += f;
            }
            PrintGraph(grafo);
            Console.WriteLine("flusso inviato = " + fMax);
            return fMax;
        }
    }
}