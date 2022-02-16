using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

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
                if (node == next && e.Capacity > 0 && previous.Label == (node.Label - 1) && previous.Valid == true && previous.InFlow > 0)
                {
                    node.SetPreviousNode(previous);
                    node.SetPreviousEdge(e);
                    node.SetInFlow(Math.Min(e.Capacity, previous.InFlow));
                    e.SetReversed(false);
                    node.SetValid(true);
                    return true;
                }
                if (node == previous && e.Flow > 0 && next.Label == (node.Label - 1) && next.Valid == true && next.InFlow > 0)
                {
                    node.SetPreviousNode(next);
                    node.SetPreviousEdge(e);
                    node.SetInFlow(Math.Min(e.Flow, next.InFlow));
                    e.SetReversed(true);
                    node.SetValid(true);
                    return true;
                }
            }
            grafo.InvalidNode(node);
            return false;
        }

        //TODO da capire se CorrectFlow deve arrivare fino a s oppure si può fermare prima


        public static int doBfs(Graph grafo, Stack<Node> malati)
        {
            Queue<Node> coda;
            if (malati.Count == 0)
            {

                //grafo.ResetLabel(0);
                coda = new Queue<Node>();
                coda.Enqueue(grafo.Source);
                grafo.ResetLabel(0);
            }
            else
            {
                Node n = null;
                Node p = null;
                bool repaired = true;
                while (malati.Count > 0)
                {
                    n = malati.Pop();
                    UpdateInFlow(n, p);
                    if (!Repair(grafo, n))
                    {
                        repaired = false;
                        break;
                    }
                    p = n;
                }
                if (repaired)
                {
                    int f = UpdateInFlow(grafo.Sink, n);
                    //if (f != 0)
                    return f;
                }
                coda = new Queue<Node>(grafo.LabeledNode[n.Label - 1]);
                grafo.ResetLabel(n.Label);
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

        private static int UpdateInFlow(Node n, Node p)
        {
            if (p == null)
                return 0;
            if (n == p)
                return p.InFlow;
            n.SetInFlow(Math.Min(UpdateInFlow(n.PreviousNode, p), n.PreviousEdge.Reversed ? n.PreviousEdge.Flow : n.PreviousEdge.Capacity));
            return n.InFlow;
        }
        /*
                private static int UpdateInFlow(Node sink)
                {
                    if (!sink.Valid)
                        return 0;
                    if (sink is SourceNode)
                        return int.MaxValue;
                    sink.SetInFlow(Math.Min(UpdateInFlow(sink.PreviousNode), sink.PreviousEdge.Reversed ? sink.PreviousEdge.Flow : sink.PreviousEdge.Capacity));
                    return sink.InFlow;
                }
        */


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
            //int fMax = 0;
            var s = grafo.Source;
            var t = grafo.Sink;
            Stack<Node> malati = new();
            while (true)
            {
                int f;
                f = doBfs(grafo, malati);
                if (f == 0)
                    break;
                Node mom = t;
                malati.Clear();
                while (mom != s)
                {
                    if (mom.PreviousNode.AddFlow(f, mom.PreviousEdge))
                        malati.Push(mom);
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