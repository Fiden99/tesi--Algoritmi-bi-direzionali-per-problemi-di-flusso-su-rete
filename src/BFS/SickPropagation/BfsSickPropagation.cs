
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Security.Cryptography;
using BFS.LastLevelOpt;

namespace BFS.SickPropagation
{
    public class BfsSickPropagation
    {
        private static int SickPropagation(Graph grafo, Node node, Queue<Node> coda)
        {
            Queue<Node> malati = new Queue<Node>();
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
                    CorrectFlow(m);
                    return m.InFlow;//RecoverFlow(m);
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
            foreach (var e in node.Edges)
            {
                Node previous = e.PreviousNode;
                Node next = e.NextNode;
                if (e.NextNode == node && e.Capacity > 0)
                {
                    if (previous.Label == (node.Label - 1) && previous.Valid == true && previous.InFlow != 0)
                    {
                        node.SetInFlow(Math.Min(previous.InFlow, e.Capacity));
                        node.SetPreviousNode(previous);
                        node.SetPreviousEdge(e);
                        e.SetReversed(false);
                        return true;
                    }
                }
                else if (previous == node && e.Flow > 0)
                {
                    if (next.Label == (node.Label - 1) && next.Valid == true && next.InFlow != 0)
                    {
                        node.SetInFlow(Math.Min(next.InFlow, e.Flow));
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

                coda = new Queue<Node>();
                int v = SickPropagation(grafo, noCap, coda);
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
                //if (element.Valid == false)
                //continue;
                foreach (var edge in element.Edges)
                {
                    var n = edge.NextNode;
                    var p = edge.PreviousNode;
                    if (n.InFlow != 0 && p.InFlow != 0)
                        continue;
                    if (edge.Capacity < 0)
                        throw new InvalidOperationException();
                    if (p == element && edge.Capacity > 0 && (n.Label >= p.Label || noCap == null || n.Valid == false))
                    {
                        n.SetPreviousNode(p);
                        n.SetPreviousEdge(edge);
                        if (n.Valid == true)
                            grafo.ChangeLabel(n, p.Label + 1);
                        else
                            grafo.RepairNode(n, p.Label + 1);
                        n.SetInFlow(Math.Min(p.InFlow, edge.Capacity));
                        edge.SetReversed(false);
                        if (n is SinkNode)
                        {
                            CorrectFlow(n);
                            return n.InFlow;
                        }
                        else
                            coda.Enqueue(n);
                    }
                    else if (n == element && edge.Flow > 0 && (p.Label >= n.Label || noCap == null || p.Valid == false))
                    {
                        p.SetPreviousNode(n);
                        p.SetPreviousEdge(edge);
                        if (p.Valid == true)
                            grafo.ChangeLabel(p, n.Label + 1);
                        else
                            grafo.RepairNode(p, n.Label + 1);
                        p.SetInFlow(Math.Min(n.InFlow, edge.Flow));
                        edge.SetReversed(true);
                        if (p is SinkNode)
                        {
                            //RecoverFlow(p);
                            CorrectFlow(p);
                            return p.InFlow;

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
            Node vuoto = null;
            //int fMax = 0;
            Node t = graph.Sink;
            Node s = graph.Source;
            while (true)
            {
                int f = BfsSickPropagation.DoBfs(graph, vuoto);
                if (f == 0)
                    break;
                Node mom = t;
                while (mom != s)
                {
                    if (mom.PreviousNode.AddFlow(f, mom.PreviousEdge))
                        vuoto = mom;
                    mom = mom.PreviousNode;
                }
                //fMax += f;
            }
            PrintGraph(graph);
            return int.MaxValue - s.InFlow;

        }
    }
}