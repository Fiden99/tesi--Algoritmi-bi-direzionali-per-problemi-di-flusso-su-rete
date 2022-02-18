using System;
using System.Collections.Generic;
using System.Linq;

namespace Bidirezionale.Label.LastLevelOptEdgeFlow
{
    public class BiLabelLastLevelOpt
    {
        public static bool RepairNode(Graph graph, Node node, bool borderForward)
        {
            if (node is SourceNode || node is SinkNode)
                return false;
            if (node.SourceSide)
            {
                foreach (var e in node.Edges)
                {
                    Node previous = e.PreviousNode;
                    Node next = e.NextNode;
                    if (previous.SourceSide != next.SourceSide)
                        continue;
                    //TODO controllare se è corretto il calcolo della label
                    if (node == next && e.Capacity > 0 && previous.Label == (node.Label - 1) && previous.Valid)
                    {
                        //grafo.ChangeLabel(node, true, node.Label);
                        node.SetPreviousNode(previous);
                        node.SetPreviousEdge(e);
                        node.SetVisited(true);
                        e.SetReversed(false);
                        node.SetValid(true);
                        return true;
                    }
                    if (node == previous && e.Flow > 0 && next.Label == (node.Label - 1) && next.Valid)
                    {
                        //grafo.ChangeLabel(node, true, node.Label);
                        node.SetPreviousNode(next);
                        node.SetPreviousEdge(e);
                        node.SetVisited(true);
                        e.SetReversed(true);
                        node.SetValid(true);
                        return true;
                    }
                }
                return false;
            }
            else
            {
                foreach (var e in node.Edges)
                {
                    Node previous = e.PreviousNode;
                    Node next = e.NextNode;
                    if (previous.SourceSide != next.SourceSide && !borderForward)
                    {
                        //TODO da capire se ca bene che sia contenuto in lastNodesSourceSide o se devo considerarare altro
                        //TODO fare debugging per essere sicuro di non aver invertito next e previous
                        if (next == node && e.Capacity > 0 && previous.Valid && graph.LastNodesSourceSide.Contains(previous))
                        {
                            node.SetPreviousEdge(e);
                            node.SetPreviousNode(previous);
                            node.SetVisited(true);
                            e.SetReversed(false);
                            node.SetValid(true);
                            return true;
                        }
                        if (previous == node && e.Flow > 0 && next.Valid && graph.LastNodesSourceSide.Contains(next))
                        {
                            node.SetPreviousEdge(e);
                            node.SetPreviousNode(next);
                            node.SetVisited(true);
                            e.SetReversed(true);
                            node.SetValid(true);
                            return true;
                        }
                    }
                    else if (borderForward)
                    {
                        if (node == previous && e.Capacity > 0 && next.Valid && node.Label == (next.Label + 1))
                        {
                            node.SetNextEdge(e);
                            node.SetNextNode(next);
                            node.SetVisited(true);
                            e.SetReversed(false);
                            node.SetValid(true);
                            return true;
                        }
                        if (node == next && e.Flow > 0 && previous.Valid && node.Label == (previous.Label + 1))
                        {
                            node.SetNextEdge(e);
                            node.SetNextNode(previous);
                            node.SetVisited(true);
                            e.SetReversed(true);
                            node.SetValid(true);
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public static bool Reached(Node target, Node n)
        {
            if (target == n)
                return true;
            if (target.SourceSide)
            {
                while (target.Label <= n.Label || !n.SourceSide)
                {
                    if (!n.Valid)
                        return false;
                    n = n.PreviousNode;
                    if (n == target)
                        return true;
                }
            }
            else
            {
                while (target.Label <= n.Label)
                {
                    if (!n.Valid)
                        return false;
                    n = n.NextNode;
                    if (n == target)
                        return true;
                }
            }
            return false;
        }
        public static Node DoBfs(Graph graph, Stack<Node> noCapsSource, Stack<Node> noCapsSink)
        {
            Queue<Node> codaSource = new();
            Queue<Node> codaSink = new();
            Queue<Node> buffer = new();
            Node noCapSource = null;
            Node noCapSink = null;
            if (noCapsSource.Count > 0)
            {
                bool repaired = true;
                while (noCapsSource.Count > 0)
                {
                    noCapSource = noCapsSource.Pop();
                    if (!RepairNode(graph, noCapSource, false))
                    {// sere per confermare che ho riparato tutti i nodi
                        noCapsSource.Push(noCapSource);
                        repaired = false;
                        break;
                    }
                }
                if (repaired && noCapsSink.Count == 0)
                    foreach (var n in graph.LastNodesSinkSide.Where(x => x.Valid))
                        if (Reached(noCapSource, n))
                            return n;
                if (!repaired)
                {
                    if (noCapSource is SourceNode)
                    {
                        codaSource.Enqueue(noCapSource);
                    }
                    else if (!noCapSource.SourceSide)
                    {
                        foreach (var n in graph.LastNodesSourceSide)
                            codaSource.Enqueue(n);
                    }
                    else
                    {
                        foreach (var n in graph.LabeledNodeSourceSide[noCapSource.Label - 1])
                            codaSource.Enqueue(n);
                        graph.ResetSourceSide(noCapSource.Label);
                    }
                }
            }
            if (noCapsSink.Count > 0)
            {
                bool repaired = true;
                while (noCapsSink.Count > 0)
                {
                    noCapSink = noCapsSink.Pop();
                    if (!RepairNode(graph, noCapSink, true))
                    {
                        noCapsSink.Push(noCapSink);
                        repaired = false;
                        break;
                    }
                }
                if (repaired && noCapsSource.Count == 0)
                {
                    foreach (var n in graph.LastNodesSinkSide.Where(x => x.Valid))
                    {
                        if (Reached(noCapSink, n))
                            return n;
                    }
                }
                if (!repaired)
                    if (noCapSink is SinkNode)
                    {
                        codaSink.Enqueue(noCapSink);
                    }
                    else
                    {
                        foreach (var n in graph.LabeledNodeSinkSide[noCapSink.Label - 1])
                            codaSink.Enqueue(n);
                        graph.ResetSinkSide(noCapSink.Label);
                    }
            }
            while (codaSink.Count > 0 || codaSource.Count > 0)
            {
                while (codaSource.Count > 0 && noCapsSource.Count > 0)
                {
                    var element = codaSource.Dequeue();
                    if (!element.SourceSide || !element.Visited)
                        continue;
                    if (element.Valid)
                        foreach (var e in element.Edges)
                        {
                            Node p = e.PreviousNode;
                            Node n = e.NextNode;
#if DEBUG
                            if (e.Capacity < 0 || e.Flow < 0)
                                throw new InvalidOperationException("capacità negativa");
#endif
                            if (element == p && e.Capacity > 0)
                            {
                                if (n.Visited)
                                    if (n.SourceSide)
                                        continue;
                                    else
                                    {
                                        n.SetVisited(true);
                                        n.SetPreviousNode(element);
                                        n.SetPreviousEdge(e);
                                        //graph.AddLast(p);
                                        graph.AddLast(n);
                                        e.SetReversed(false);
                                        //n.SetInFlow(f);
                                        return n;
                                    }
                                //n.SetSourceSide(true);
                                n.SetVisited(true);
                                graph.ChangeLabel(n, true, p.Label + 1);
                                n.SetPreviousNode(p);
                                n.SetPreviousEdge(e);
                                e.SetReversed(false);
                                n.SetValid(true);
                                buffer.Enqueue(n);
                            }
                            else if (element == n && e.Flow > 0)
                            {
                                if (p.Visited)
                                    if (p.SourceSide)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        p.SetVisited(true);
                                        p.SetPreviousNode(n);
                                        p.SetPreviousEdge(e);
                                        graph.AddLast(p);
                                        //graph.AddLast(n);
                                        e.SetReversed(true);
                                        //p.SetInFlow(f);
                                        return p;
                                    }
                                //p.SetSourceSide(true);
                                p.SetVisited(true);
                                graph.ChangeLabel(p, true, n.Label + 1);
                                p.SetPreviousEdge(e);
                                p.SetPreviousNode(n);
                                e.SetReversed(true);
                                p.SetValid(true);
                                buffer.Enqueue(p);
                            }
                        }
                }
                var mom = codaSource;
                codaSource = buffer;
                buffer = mom;
                while (codaSink.Count > 0 && noCapsSink.Count > 0)
                {
                    var element = codaSink.Dequeue();
                    if (element.SourceSide || !element.Visited)
                        continue;
                    if (element.Valid)
                        foreach (var e in element.Edges)
                        {
                            var p = e.PreviousNode;
                            var n = e.NextNode;
#if DEBUG
                            if (e.Capacity < 0 || e.Flow < 0)
                                throw new InvalidOperationException("capacità negativa");
#endif
                            if (element == n && e.Capacity > 0)
                            {
                                if (p.Visited)
                                {
                                    if (!p.SourceSide)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        n.SetVisited(true);
                                        n.SetPreviousEdge(e);
                                        n.SetPreviousNode(p);
                                        graph.AddLast(n);
                                        //graph.AddLast(p);
                                        e.SetReversed(false);
                                        //p.SetInFlow(f);
                                        return n;
                                    }
                                }
                                //p.SetSourceSide(false);
                                p.SetVisited(true);
                                p.SetNextEdge(e);
                                p.SetNextNode(n);
                                graph.ChangeLabel(p, false, n.Label + 1);
                                e.SetReversed(false);
                                p.SetValid(true);
                                buffer.Enqueue(p);
                            }
                            else if (element == p && e.Flow > 0)
                            {
                                if (n.Visited)
                                    if (!n.SourceSide)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        //TODO capire come fare in caso getflow ritorni null                                        
                                        p.SetVisited(true);
                                        p.SetPreviousEdge(e);
                                        p.SetPreviousNode(n);
                                        //graph.AddLast(n);
                                        graph.AddLast(p);
                                        e.SetReversed(true);
                                        //n.SetInFlow(f);
                                        return p;
                                    }
                                //n.SetSourceSide(false);
                                n.SetVisited(true);
                                n.SetNextEdge(e);
                                n.SetNextNode(p);
                                graph.ChangeLabel(n, false, p.Label + 1);
                                e.SetReversed(true);
                                n.SetValid(true);
                                buffer.Enqueue(n);
                            }
                        }

                }
                mom = codaSink;
                codaSink = buffer;
                buffer = mom;
            }
            return null;
        }

        public static int GetFlow(Node n)
        {
            if (n == null)
                return 0;
            int source = int.MaxValue;
            int sink = int.MaxValue;
#if DEBUG
            if (n.PreviousEdge == null || n.NextEdge == null)
                throw new ArgumentException("il nodo selezionato deve avere sia previous sia next");
#endif
            Node x = n;
            while (n is not SourceNode)
            {
                source = Math.Min(source, n.PreviousEdge.Reversed ? n.PreviousEdge.Flow : n.PreviousEdge.Capacity);
                n = n.PreviousNode;
            }
            while (x is not SinkNode)
            {
                sink = Math.Min(sink, x.NextEdge.Reversed ? x.NextEdge.Flow : x.NextEdge.Capacity);
                x = x.NextNode;
            }
            return Math.Min(source, sink);
        }

        public static int FlowFordFulkerson(Graph graph)
        {
            Node s = graph.Source;
            Node t = graph.Sink;
            Stack<Node> vuotiSource = new();
            vuotiSource.Push(s);
            Stack<Node> vuotiSink = new();
            vuotiSink.Push(t);
            int fMax = 0;
            while (true)
            {
                var n = DoBfs(graph, vuotiSource, vuotiSink);
                if (n == null)
                    return fMax;
                int f = GetFlow(n);
                if (f == 0)
                    return fMax;
                fMax += f;
                vuotiSink.Clear();
                vuotiSource.Clear();
                Node mom = n;
                while (n != s)
                {
                    if (n.PreviousEdge.AddFlow(f))
                    {
                        vuotiSource.Push(n);
                        n.SetValid(false);
                    }
                    n = n.PreviousNode;

                }
                while (mom != t)
                {
                    if (mom.NextEdge.AddFlow(f))
                    {
                        vuotiSink.Push(mom);
                        mom.SetValid(false);
                    }
                    mom = mom.NextNode;

                }
            }


        }
    }
}