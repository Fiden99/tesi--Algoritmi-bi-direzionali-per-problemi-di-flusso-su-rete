using System;
using System.Collections.Generic;
using System.Linq;

namespace Monodirezionale.MaxFlow.SickPropagationGraphOpt
{
    public class BfsSickPropagationGraphOpt
    {
        private static int SickPropagation(Graph grafo, Node n, Node element, Queue<Node> coda)
        {
            Queue<(Node, Node)> malati = new Queue<(Node, Node)>();
            malati.Enqueue((n, element));
            while (malati.Count > 0)
            {
                var x = malati.Dequeue();
                Node actual = x.Item1;
                Node previous = x.Item2;
                if (!Repair(grafo, actual, previous))
                {
                    foreach (var y in actual.NextLabelNodes)
                        malati.Enqueue((y, actual));
                }
                else if (actual is SinkNode)//&& actual.InFlow != 0)
                    return CorrectFlow(actual);
                else
                    coda.Enqueue(actual);
            }
            return 0;
        }

        //TODO valutare il fatto che dopo il reset non ci sono ho PreviousLabelNodes pulita
        // capire se durante il reset dobbiamo rimuovere o meno PreviousLabelNodes
        public static bool Repair(Graph grafo, Node node, Node removedNode)
        {
            node.RemovePreviousLabelNode(removedNode);
            if (node.PreviousLabelNodes.Count != 0)
            {
                Node previous = node.PreviousLabelNodes.First();
                BiEdge e = previous.Edges.First(x => x.NextNode == node);
                node.SetInFlow(Math.Min(e.Capacity, previous.InFlow));
                node.SetPreviousNode(previous);
                node.SetPreviousEdge(e);
                node.SetValid(true);
                //grafo.ChangeLabel(e.previousNode, node.label - 1);
                return true;
                /* TODO da capire se tenere questa condizione o meno
                    if(n.edges.Single(x => x.nextNode == node).capacity <=0)
                        throw new InvalidOperationException();
                */

            }
            grafo.InvalidNode(node);
            return false;
        }
        public static int CorrectFlow(Node node)
        {
            int v;
            if (node is not SourceNode)
            {
                var p = node.PreviousNode;
                var e = node.Edges.Single(x => x.NextNode == p || p == x.PreviousNode);
                if (e.Reversed)
                {
                    v = Math.Min(e.Flow, CorrectFlow(p));
                    if (v == e.Flow && p.InFlow == v)
                        return v;
                }
                else
                {
                    v = Math.Min(e.Capacity, CorrectFlow(p));
                    if (v == e.Capacity && v == p.InFlow)
                        return v;
                }
                node.SetInFlow(v);
            }
            else
                v = node.InFlow;
            return v;
        }

        public static int RecoverFlow(Node node)
        {
            if (node.InFlow != 0)
                return node.InFlow;
            var e = node.Edges.Single(x => x.PreviousNode == node.PreviousNode || x.NextNode == node.PreviousNode);
            if (node.PreviousNode != null && node.InFlow == 0)
                if (e.Capacity > 0 && e.NextNode == node)
                    node.SetInFlow(Math.Min(RecoverFlow(node.PreviousNode), e.Capacity));
                else if (e.Flow > 0 && e.PreviousNode == node)
                    node.SetInFlow(Math.Min(RecoverFlow(node.PreviousNode), e.Flow));
            return node.InFlow;
        }

        public static int DoBfs(Graph grafo, Node noCap)
        {
            Queue<Node> coda;
            Queue<(Node, Node)> malati = new Queue<(Node, Node)>();
            if (noCap == null)
            {

                //grafo.Reset(0);
                coda = new Queue<Node>();
                coda.Enqueue(grafo.Source);
            }
            else
            {
                Node t = grafo.Sink;
                if (Repair(grafo, noCap, noCap.PreviousNode) && t.PreviousNode.InFlow != 0 && t.Edges.Single(x => x.PreviousNode == t.PreviousNode).Capacity > 0)
                {
                    return Math.Min(t.InFlow, noCap.InFlow);
                }
                coda = new Queue<Node>();
                int v = SickPropagation(grafo, noCap, noCap.PreviousNode, coda);
                if (v != 0)
                    return v;
                grafo.Reset(noCap.Label);
                if (coda.Count == 0)
                    foreach (var n in grafo.LabeledNodes[noCap.Label - 1])
                        coda.Enqueue(n);
                foreach (var n in coda)
                    RecoverFlow(n);
            }
            while (coda.Count > 0)
            {
                var element = coda.Dequeue();
                //TODO capire se è possibile che element sia invalido o meno
                //if (element.Valid == false)
                //continue;
                foreach (var edge in element.Edges)
                {
                    var p = edge.PreviousNode;
                    var n = edge.NextNode;
                    if (n.InFlow != 0 && p.InFlow != 0)
                        continue;
                    if (edge.Capacity < 0)
                        throw new InvalidOperationException();
                    if (p == element && edge.Capacity > 0 && (n.Label >= p.Label || n.Label == 0 || n.Valid == false))
                    {
                        n.SetPreviousNode(element);
                        n.SetPreviousEdge(edge);
                        if (n.Valid == true)
                            grafo.ChangeLabel(n, p.Label + 1);
                        else
                            grafo.RepairNode(n, p.Label + 1);
                        n.SetInFlow(Math.Min(p.InFlow, edge.Capacity));
                        edge.SetReversed(false);
                        if (n is SinkNode && n.Valid == true)//&& n.InFlow != 0)
                            return n.InFlow;
                        else
                            coda.Enqueue(n);
                    }
                    if (n == element && edge.Flow > 0 && (p.Label >= n.Label || p.Label == 0 || p.Valid == false))
                    {
                        p.SetPreviousNode(element);
                        p.SetPreviousEdge(edge);
                        if (p.Valid == true)
                            grafo.ChangeLabel(p, n.Label + 1);
                        else
                            grafo.RepairNode(p, n.Label + 1);
                        p.SetInFlow(Math.Min(n.InFlow, edge.Flow));
                        edge.SetReversed(true);
                        if (p is SinkNode && p.Valid == true)
                            return p.InFlow;
                        else
                            coda.Enqueue(p);
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
                    foreach (var x in node.Edges.Where(x => x.PreviousNode == node || x.NextNode == node))
                        Console.Write(" to " + x.NextNode.Name + ", f = " + x.Flow + ", c  = " + x.Capacity + ";");
                    Console.WriteLine();
                }
            }
            if (grafo.InvalidNodes.Count > 0)
                Console.WriteLine("nodi malati : ");
            foreach (var node in grafo.InvalidNodes)
            {
                Console.Write("node " + node.Name);
                foreach (var x in node.Edges.Where(x => x.PreviousNode == node || x.NextNode == node))
                    Console.Write(" to " + x.NextNode.Name + ", f = " + x.Flow + ", c  = " + x.Capacity + ";");
                Console.WriteLine();
            }
        }
        public static int FlowFordFulkerson(Graph grafo)
        {
            Node noCap = null;
            //int fMax = 0;
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
                    if (mom.PreviousNode.AddFlow(f, mom.PreviousEdge))
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
                //fMax += f;
            }
            PrintGraph(grafo);
            Console.WriteLine("flusso inviato = " + (int.MaxValue - s.InFlow));
            return (int.MaxValue - s.InFlow);
        }
    }
}