using System;
using System.Collections.Generic;
using System.Linq;

namespace Bidirezionale.NodeCount.LastLevelOptEdgeFlow
{
    public class BiNodeCountLastLevelOpt
    {
        private static Node FirstBfs(Graph graph)
        {
            var s = graph.Source;
            var t = graph.Sink;
            var codaSource = new Queue<Node>();
            codaSource.Enqueue(s);
            var codaSink = new Queue<Node>();
            codaSink.Enqueue(t);
            var codaEdgeSource = new Queue<BiEdge>();
            var codaEdgeSink = new Queue<BiEdge>();
            Node elementSource = null;
            Node elementSink = null;
            Node returnNode = null;
            while (codaSink.Count > 0 || codaSource.Count > 0)
            {
                if ((codaSource.Count > 0 && codaEdgeSource.Count == 0) || (codaSink.Count == 0 && codaEdgeSink.Count == 0))
                {
                    elementSource = codaSource.Dequeue();
                    foreach (var e in elementSource.Edges.Where(x => x.PreviousNode == elementSource))
                        codaEdgeSource.Enqueue(e);
                }
                if ((codaSink.Count > 0 && codaEdgeSink.Count == 0) || (codaSource.Count == 0 && codaEdgeSource.Count == 0))
                {
                    elementSink = codaSink.Dequeue();
                    foreach (var e in elementSink.Edges.Where(x => x.NextNode == elementSink))
                        codaEdgeSink.Enqueue(e);
                }
                while (codaEdgeSink.Count > 0 && codaEdgeSource.Count > 0)
                {
                    var es = codaEdgeSource.Dequeue();
                    Node ps = es.PreviousNode;
                    Node ns = es.NextNode;
#if DEBUG
                    if (es.Capacity < 0 || es.Flow < 0)
                        throw new InvalidOperationException("capacità o flusso negativi");
#endif
                    if (elementSource == ps && es.Capacity > 0)
                    {
                        if (ns.Visited)
                        {
                            if (ns.SourceSide)
                                continue;
                            else if (returnNode is null)
                            {
                                ns.SetPreviousNode(ps);
                                ns.SetPreviousEdge(es);
                                graph.AddLast(ns);
                                returnNode = ns;
                            }
                        }
                        else
                        {
                            ns.SetVisited(true);
                            graph.ChangeLabel(ns, true, ps.Label + 1);
                            ns.SetPreviousEdge(es);
                            ns.SetPreviousNode(ps);
                            codaSource.Enqueue(ns);
                        }
                    }
                    var et = codaEdgeSink.Dequeue();
                    var nt = et.NextNode;
                    var pt = et.PreviousNode;
#if DEBUG
                    if (et.Capacity < 0 || et.Flow < 0)
                        throw new InvalidOperationException("capacità negativa");
#endif
                    if (elementSink == nt && et.Capacity > 0)
                    {
                        if (pt.Visited)
                        {
                            if (!pt.SourceSide)
                                continue;
                            else if (returnNode is null)
                            {
                                nt.SetPreviousEdge(et);
                                nt.SetPreviousNode(pt);
                                graph.AddLast(nt);
                                returnNode = nt;
                            }
                        }
                        else
                        {
                            //p.SetSourceSide(false);
                            pt.SetVisited(true);
                            pt.SetNextEdge(et);
                            pt.SetNextNode(nt);
                            graph.ChangeLabel(pt, false, nt.Label + 1);
                            pt.SetValid(true);
                            codaSink.Enqueue(pt);
                        }
                    }
                }
            }
            return returnNode;
        }

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

        public static Node DoBfs(Graph graph, Node noCapSource, Node noCapSink)
        {
            Queue<Node> codaSource = new();
            Queue<Node> codaSink = new();
            bool sourceRepaired = false;
            bool sinkRepaired = false;
            if (noCapSource != null)
            {
                if (RepairNode(graph, noCapSource, false))
                {
                    if (noCapSink == null)
                        foreach (var n in graph.LastNodesSinkSide)
                        {
                            //TODO capire se devo fare un controllo di inflow tra n, il predecessore e il successore ( con nodi)
                            if (ValidPath(noCapSource, n))
                                return n;
                        }
                    else
                        sourceRepaired = true;
                }
                if (noCapSource is SourceNode)
                {
                    codaSource = new();
                    codaSource.Enqueue(noCapSource);
                }
                else if (!noCapSource.SourceSide)
                {
                    codaSource = new(graph.LastNodesSourceSide);
                }
                else
                {
                    codaSource = new(graph.LabeledNodeSourceSide[noCapSource.Label - 1]);
                    graph.ResetSourceSide(noCapSource.Label);
                }
            }
            if (noCapSink != null)
            {
                if (RepairNode(graph, noCapSink, true))
                {
                    sinkRepaired = true;
                    if (noCapSource == null || sourceRepaired)
                    {
                        foreach (var n in graph.LastNodesSinkSide)
                        {
                            if (sourceRepaired)
                                if (!ValidPath(noCapSource, n))
                                    continue;
                            if (ValidPath(noCapSink, n))
                                return n;
                        }
                    }
                }
                if (noCapSink is SinkNode)
                {
                    codaSink = new();
                    codaSink.Enqueue(noCapSink);
                }
                else
                {
                    codaSink = new(graph.LabeledNodeSinkSide[noCapSink.Label - 1]);
                    graph.ResetSinkSide(noCapSink.Label);
                }
            }

#if DEBUG
            if (noCapSink == null && noCapSource == null)
                throw new InvalidOperationException("non ho nessun arco senza capacità residua");
#endif
            while (codaSink.Count > 0 || codaSource.Count > 0)
            {
                if (codaSource.Count > 0 && (noCapSource != null || !sourceRepaired))
                {
                    var element = codaSource.Dequeue();
                    if (!element.SourceSide || !element.Valid)
                        continue;
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
                                    n.SetPreviousNode(element);
                                    n.SetPreviousEdge(e);
                                    //graph.AddLast(p);
                                    graph.AddLast(n);
                                    e.SetReversed(false);
                                    //n.SetInFlow(f);
                                    return n;
                                }
                            //nel caso abbiamo trovato un nodo sourside, ma non sono riuscito a procedere, dico di iniziare nuovamente a visionare sinkSide
                            if (!n.SourceSide && n is not SinkNode)
                            {
                                sinkRepaired = false;
                                foreach (var node in graph.LabeledNodeSinkSide[n.Label - 1])
                                    codaSink.Enqueue(node);
                                graph.ResetSinkSide(n.Label);
                                continue;
                            }
                            //n.SetSourceSide(true);
                            n.SetVisited(true);
                            graph.ChangeLabel(n, true, p.Label + 1);
                            n.SetPreviousNode(p);
                            n.SetPreviousEdge(e);
                            e.SetReversed(false);
                            n.SetValid(true);
                            codaSource.Enqueue(n);
                        }
                        else if (element == n && e.Flow > 0)
                        {
                            if (p.Visited)
                                if (p.SourceSide)
                                    continue;
                                else
                                {
                                    p.SetPreviousNode(n);
                                    p.SetPreviousEdge(e);
                                    graph.AddLast(p);
                                    //graph.AddLast(n);
                                    e.SetReversed(true);
                                    //p.SetInFlow(f);
                                    return p;
                                }
                            //p.SetSourceSide(true);
                            if (!p.SourceSide && p is not SinkNode)
                            {
                                sinkRepaired = false;
                                foreach (var node in graph.LabeledNodeSinkSide[p.Label - 1])
                                    codaSink.Enqueue(node);
                                graph.ResetSinkSide(n.Label);
                                continue;
                            }
                            p.SetVisited(true);
                            graph.ChangeLabel(p, true, n.Label + 1);
                            p.SetPreviousEdge(e);
                            p.SetPreviousNode(n);
                            e.SetReversed(true);
                            p.SetValid(true);
                            codaSource.Enqueue(p);
                        }
                    }
                }
                if (codaSink.Count > 0 && (noCapSink != null || !sinkRepaired))
                {
                    var element = codaSink.Dequeue();
                    if (element.SourceSide || !element.Valid)
                        continue;
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
                            if (p.SourceSide && noCapSink is not SinkNode)
                                continue;
                            p.SetVisited(true);
                            p.SetNextEdge(e);
                            p.SetNextNode(n);
                            graph.ChangeLabel(p, false, n.Label + 1);
                            e.SetReversed(false);
                            p.SetValid(true);
                            codaSink.Enqueue(p);
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
                                    p.SetPreviousEdge(e);
                                    p.SetPreviousNode(n);
                                    //graph.AddLast(n);
                                    graph.AddLast(p);
                                    e.SetReversed(true);
                                    //n.SetInFlow(f);
                                    return p;
                                }
                            //n.SetSourceSide(false);
                            if (n.SourceSide && n is not SinkNode)
                                continue;
                            n.SetValid(true);
                            n.SetNextEdge(e);
                            n.SetNextNode(p);
                            graph.ChangeLabel(n, false, p.Label + 1);
                            e.SetReversed(true);
                            n.SetValid(true);
                            codaSink.Enqueue(n);
                        }
                    }

                }
            }
            return null;
        }

        public static bool ValidPath(Node target, Node n)
        {
            if (target.SourceSide)
            {
                while (target.Label < n.Label || !n.SourceSide)
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
                while (target.Label < n.Label)
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

        private static (Node, Node) SendFlow(Node n, int f)
        {
            Node vuotoSource = null;
            Node vuotoSink = null;
            Node mom = n;
            while (mom is not SourceNode)
            {
                if (mom.PreviousEdge.AddFlow(f))
                {
                    vuotoSource = mom;
                    mom.SetValid(false);
                }
                mom = mom.PreviousNode;
            }
            while (n is not SinkNode)
            {
                if (n.NextEdge.AddFlow(f))
                {
                    vuotoSink = n;
                    n.SetValid(false);
                }
                n = n.NextNode;
            }
            return (vuotoSource, vuotoSink);
        }

        public static int FlowFordFulkerson(Graph graph)
        {
            var node = FirstBfs(graph);
            int fMax = GetFlow(node);
            var (vuotoSource, vuotoSink) = SendFlow(node, fMax);
            while (true)
            {
                var n = DoBfs(graph, vuotoSource, vuotoSink);
                if (n == null)
                    break;
                int f = GetFlow(n);
                if (f == 0)
                    break;
                fMax += f;
                (vuotoSource, vuotoSink) = SendFlow(n, f);
            }
            return fMax;

        }

        private static int GetFlow(Node n)
        {
            int f = int.MaxValue;
            Node mom = n;
            while (n is not SourceNode)
            {
                f = Math.Min(f, n.PreviousEdge.Reversed ? n.PreviousEdge.Flow : n.PreviousEdge.Capacity);
                n = n.PreviousNode;
            }
            while (mom is not SinkNode)
            {
                f = Math.Min(f, mom.NextEdge.Reversed ? mom.NextEdge.Flow : mom.NextEdge.Capacity);
                mom = mom.NextNode;
            }
            return f;
        }
    }
}