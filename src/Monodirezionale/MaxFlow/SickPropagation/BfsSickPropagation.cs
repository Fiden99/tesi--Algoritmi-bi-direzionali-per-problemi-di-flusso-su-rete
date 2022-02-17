
using System;
using System.Collections.Generic;
using System.Linq;

namespace Monodirezionale.MaxFlow.SickPropagation
{
    public class BfsSickPropagation
    {
        private static int SickPropagation(Graph grafo, Node node, Queue<Node> coda)
        {
            Queue<Node> malati = new();
            malati.Enqueue(node);
            while (malati.Count > 0)
            {
                Node m = malati.Dequeue();
                if (!Repair(grafo, m))
                    foreach (var e in m.Edges)
                    {
                        Node n = e.NextNode;
                        Node p = e.PreviousNode;
                        //TODO capire se devo tenere 
                        //e.NextNode == m.PreviousNode
                        //oppure 
                        //e.NextNode.Label == m.Label + 1
                        if (p == m && m == n.PreviousNode)
                            malati.Enqueue(n);
                        else if (n == m && m == p.PreviousNode)
                            malati.Enqueue(p);
                    }
                else if (m is SinkNode)
                {
                    var n = GetFlow(m);
                    return n;//RecoverFlow(m);
                }
                else
                {
                    coda.Enqueue(m);
                }
            }
            return 0;
        }
        private static bool Repair(Graph grafo, Node node)
        {
            if (node.PreviousEdge != null && node.PreviousNode.Valid && (node.PreviousEdge.Reversed ? node.PreviousEdge.Flow : node.PreviousEdge.Capacity) > 0)
                return true;
            foreach (var e in node.Edges)
            {
                Node previous = e.PreviousNode;
                Node next = e.NextNode;
                if (e.NextNode == node && e.Capacity > 0)
                {
                    if (previous.Label == (node.Label - 1) && previous.Valid == true && previous.Visited == true)
                    {
                        node.SetVisited(true);
                        node.SetPreviousNode(previous);
                        node.SetPreviousEdge(e);
                        e.SetReversed(false);
                        return true;
                    }
                }
                else if (previous == node && e.Flow > 0)
                {
                    if (next.Label == (node.Label - 1) && next.Valid == true && next.Visited == true)
                    {
                        node.SetVisited(true);
                        node.SetPreviousNode(next);
                        node.SetPreviousEdge(e);
                        e.SetReversed(true);
                        return true;
                    }
                }
            }
            grafo.InvalidNode(node);
            return false;

        }
        public static int GetFlow(Node n)
        {
            if (n is not SourceNode)
            {
                var edge = n.PreviousEdge;
                if (!edge.Reversed)
                {
                    return Math.Min(edge.Capacity, GetFlow(n.PreviousNode));
                }
                else
                {
                    return Math.Min(edge.Flow, GetFlow(n.PreviousNode));
                }
            }
            return int.MaxValue;
        }
        /*
                //TODO capire se si può migliorare fermandosi alla prima volta in cui v è uguale alla capacità
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
        */
        public static int DoMaxFlow(Graph grafo, Stack<Node> noCaps)
        {
            Queue<Node> coda = new();
            Queue<Node> malati = new();
            bool fromSource = false;
            //if (grafo.InvalidNodes.Count == 0)
            if (noCaps.Count == 0)
            {
                //grafo.Reset(0);
                coda.Enqueue(grafo.Source);
                fromSource = true;
            }
            else
            {
                Node t = grafo.Sink;
                bool repaired = true;
                while (noCaps.Count > 0)
                {
                    var noCap = noCaps.Pop();
                    if (!Repair(grafo, noCap))
                    {
                        malati.Enqueue(noCap);
                        repaired = false;
                        //break;
                    }
                }
                if (repaired)
                    return GetFlow(t);
                bool first = true;
                Node x = null;
                while (malati.Count > 0)
                {
                    var noCap = malati.Dequeue();
                    int v = SickPropagation(grafo, noCap, coda);
                    if (v != 0)
                        return v;
                    if (first)
                        x = noCap;
                    first = false;
                }
                if (t.Valid)
                    return GetFlow(t);
                grafo.Reset(x.Label);
                if (coda.Count == 0)
                    foreach (var n in grafo.LabeledNodes[x.Label - 1])
                        coda.Enqueue(n);
                //foreach (var n in coda)
                //RecoverFlow(n);
            }
            while (coda.Count > 0)
            {
                var element = coda.Dequeue();
                //if (element.Valid == false)
                //continue;
                foreach (var edge in element.Edges)
                {
#if DEBUG
                    if (edge.Capacity < 0)
                        throw new InvalidOperationException();
#endif
                    var n = edge.NextNode;
                    var p = edge.PreviousNode;
                    if ((n.Visited && p.Visited) || !element.Valid)
                        continue;
                    else if (p == element && edge.Capacity > 0 && (n.Label >= p.Label || fromSource || n.Valid == false))
                    {
                        n.SetPreviousNode(p);
                        n.SetPreviousEdge(edge);
                        if (n.Valid == true)
                            grafo.ChangeLabel(n, p.Label + 1);
                        else
                            grafo.RepairNode(n, p.Label + 1);
                        n.SetVisited(true);
                        edge.SetReversed(false);
                        if (n is SinkNode)
                        {
                            return GetFlow(n);
                        }
                        else
                            coda.Enqueue(n);
                    }
                    else if (n == element && edge.Flow > 0 && (p.Label >= n.Label || fromSource || p.Valid == false))
                    {
                        p.SetPreviousNode(n);
                        p.SetPreviousEdge(edge);
                        if (p.Valid == true)
                            grafo.ChangeLabel(p, n.Label + 1);
                        else
                            grafo.RepairNode(p, n.Label + 1);
                        p.SetVisited(true);
                        edge.SetReversed(true);
                        if (p is SinkNode)
                        {
                            return GetFlow(p);
                        }
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
            Stack<Node> vuoti = new();
            int fMax = 0;
            Node t = graph.Sink;
            Node s = graph.Source;
            while (true)
            {
                int f = DoMaxFlow(graph, vuoti);
                if (f == 0)
                    break;
                Node mom = t;
                //vuoto.Clear();
                while (mom != s)
                {
                    if (mom.PreviousNode.AddFlow(f, mom.PreviousEdge))
                        vuoti.Push(mom);
                    mom = mom.PreviousNode;
                }

                fMax += f;
            }
            //PrintGraph(graph);
            return fMax;

        }
    }
}