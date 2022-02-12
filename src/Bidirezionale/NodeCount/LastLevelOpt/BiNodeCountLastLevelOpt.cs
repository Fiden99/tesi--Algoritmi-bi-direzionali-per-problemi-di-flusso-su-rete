using System;
using System.Collections.Generic;
using System.Linq;

namespace Bidirezionale.NodeCount.LastLevelOpt
{
    public class BiNodeCountLastLevelOpt
    {
        private static (int, Node) FirstBfs(Graph graph)
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
            int returnvalue = 0;
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
                        if (ns.InFlow != 0)
                        {
                            if (ns.SourceSide)
                                continue;
                            else if (returnvalue == 0)
                            {
                                returnvalue = Math.Min(ps.InFlow, Math.Min(ns.InFlow, es.Capacity));
                                ns.SetPreviousNode(ps);
                                ns.SetPreviousEdge(es);
                                graph.AddLast(ns);
                                returnNode = ns;
                            }
                        }
                        else
                        {
                            ns.SetInFlow(Math.Min(ps.InFlow, es.Capacity));
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
                        if (pt.InFlow != 0)
                        {
                            if (!pt.SourceSide)
                                continue;
                            else if (returnvalue == 0)
                            {
                                returnvalue = Math.Min(pt.InFlow, Math.Min(et.Capacity, nt.InFlow));
                                nt.SetPreviousEdge(et);
                                nt.SetPreviousNode(pt);
                                graph.AddLast(nt);
                                returnNode = nt;
                            }
                        }
                        else
                        {
                            //p.SetSourceSide(false);
                            pt.SetInFlow(Math.Min(et.Capacity, nt.InFlow));
                            pt.SetNextEdge(et);
                            pt.SetNextNode(nt);
                            graph.ChangeLabel(pt, false, nt.Label + 1);
                            pt.SetValid(true);
                            codaSink.Enqueue(pt);
                        }
                    }
                }
            }
            return (returnvalue, returnNode);
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
                        node.SetInFlow(Math.Min(e.Capacity, previous.InFlow));
                        node.SetValid(true);
                        e.SetReversed(false);
                        return true;
                    }
                    if (node == previous && e.Flow > 0 && next.Label == (node.Label - 1) && next.Valid)
                    {
                        //grafo.ChangeLabel(node, true, node.Label);
                        node.SetPreviousNode(next);
                        node.SetPreviousEdge(e);
                        node.SetInFlow(Math.Min(e.Flow, next.InFlow));
                        node.SetValid(true);
                        e.SetReversed(true);
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
                            node.SetInFlow(Math.Min(e.Capacity, previous.InFlow));
                            e.SetReversed(false);
                            node.SetValid(true);
                            return true;
                        }
                        if (previous == node && e.Flow > 0 && next.Valid && graph.LastNodesSourceSide.Contains(next))
                        {
                            node.SetPreviousEdge(e);
                            node.SetPreviousNode(next);
                            node.SetInFlow(Math.Min(e.Flow, previous.InFlow));
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
                            node.SetInFlow(Math.Min(e.Capacity, next.InFlow));
                            e.SetReversed(false);
                            node.SetValid(true);
                            return true;
                        }
                        if (node == next && e.Flow > 0 && previous.Valid && node.Label == (previous.Label + 1))
                        {
                            node.SetNextEdge(e);
                            node.SetNextNode(previous);
                            node.SetInFlow(Math.Min(previous.InFlow, e.Flow));
                            e.SetReversed(true);
                            node.SetValid(true);
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        public static Node GetFlow(Node target, Node n)
        {
            if (n == target)
                return n;
            if (n == null)
                return null;
            if (target.SourceSide)
            {

                if (n.Label < target.Label && n.SourceSide)
                    return null;
                if (target == n.PreviousNode)
                {
                    target.SetInFlow(Math.Min(n.PreviousEdge.Capacity, target.InFlow));
                    return target;
                }
                else if (n is not SourceNode)
                {
                    var edge = n.PreviousEdge;
                    if (!edge.Reversed)
                    {
                        var m = GetFlow(target, n.PreviousNode);
                        if (m == null)
                            return null;
                        if (n.SourceSide)
                            n.SetInFlow(Math.Min(edge.Capacity, m.InFlow));
                        else
                            n.SetInFlow(Math.Min(edge.Capacity, Math.Min(m.InFlow, Math.Min(n.NextNode.InFlow, n.NextEdge.Reversed ? n.NextEdge.Flow : n.NextEdge.Capacity))));
                        return n;
                    }
                    else
                    {
                        var m = GetFlow(target, n.PreviousNode);
                        if (m == null)
                            return null;
                        if (n.SourceSide)
                            n.SetInFlow(Math.Min(edge.Flow, m.InFlow));
                        else
                            n.SetInFlow(Math.Min(edge.Flow, Math.Min(m.InFlow, Math.Min(n.NextNode.InFlow, n.NextEdge.Reversed ? n.NextEdge.Flow : n.NextEdge.Capacity))));
                        return n;
                    }
                }

            }
            else
            {
                if (target == n.NextNode)
                {
                    target.SetInFlow(Math.Min(n.NextEdge.Capacity, target.InFlow));
                    return target;
                }
                else if (n is not SinkNode)
                {
                    var edge = n.NextEdge;
                    if (!edge.Reversed)
                    {
                        var m = GetFlow(target, n.NextNode);
                        if (m == null)
                            return null;
                        if (n.PreviousNode == null)
                            n.SetInFlow(Math.Min(edge.Capacity, m.InFlow));
                        else
                            n.SetInFlow(Math.Min(edge.Capacity, Math.Min(m.InFlow, Math.Min(n.PreviousNode.InFlow, n.PreviousEdge.Reversed ? n.PreviousEdge.Flow : n.PreviousEdge.Capacity))));
                        return n;
                    }
                    else
                    {
                        var m = GetFlow(target, n.NextNode);
                        if (m == null)
                            return null;
                        if (n.PreviousEdge == null)
                            n.SetInFlow(Math.Min(edge.Flow, m.InFlow));
                        else
                            n.SetInFlow(Math.Min(edge.Flow, Math.Min(m.InFlow, Math.Min(n.PreviousNode.InFlow, n.PreviousEdge.Reversed ? n.PreviousEdge.Flow : n.PreviousEdge.Capacity))));

                        return n;
                    }
                }
                if (n.Label < target.Label)
                    return null;
            }
            return null;
        }
        public static (int, Node) DoBfs(Graph graph, Node noCapSource, Node noCapSink)
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
                            if (n.Valid && GetFlow(noCapSource, n) != null && n.InFlow != 0)
                                return (n.InFlow, n);
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
                        foreach (var n in graph.LastNodesSinkSide.Where(x => x.Valid))
                        {
                            int sourceFlow;
                            if (sourceRepaired)
                                if (GetFlow(noCapSource, n) != null)
                                    sourceFlow = n.InFlow;
                                else
                                    continue;
                            else// vuol dire che noCapSource == null
                                sourceFlow = Math.Min(n.PreviousNode.InFlow, n.PreviousEdge.Reversed ? n.PreviousEdge.Flow : n.PreviousEdge.Capacity);
                            if (sourceFlow > 0 && GetFlow(noCapSink, n) != null && n.InFlow != 0)
                                return (Math.Min(n.InFlow, sourceFlow), n);
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
                            if (n.InFlow != 0)
                                if (n.SourceSide)
                                    continue;
                                else
                                {
                                    int f = Math.Min(n.InFlow, Math.Min(p.InFlow, e.Capacity));
                                    if (f == 0)
                                        continue;
                                    n.SetPreviousNode(element);
                                    n.SetPreviousEdge(e);
                                    //graph.AddLast(p);
                                    graph.AddLast(n);
                                    e.SetReversed(false);
                                    //n.SetInFlow(f);
                                    return (f, n);
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
                            n.SetInFlow(Math.Min(p.InFlow, e.Capacity));
                            graph.ChangeLabel(n, true, p.Label + 1);
                            n.SetPreviousNode(p);
                            n.SetPreviousEdge(e);
                            e.SetReversed(false);
                            n.SetValid(true);
                            codaSource.Enqueue(n);
                        }
                        else if (element == n && e.Flow > 0)
                        {
                            if (p.InFlow != 0)
                                if (p.SourceSide)
                                    continue;
                                else
                                {
                                    int f = Math.Min(p.InFlow, n.InFlow);
                                    f = Math.Min(f, e.Flow);
                                    if (f == 0)
                                        continue;
                                    p.SetPreviousNode(n);
                                    p.SetPreviousEdge(e);
                                    graph.AddLast(p);
                                    //graph.AddLast(n);
                                    e.SetReversed(true);
                                    //p.SetInFlow(f);
                                    return (f, p);
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
                            p.SetInFlow(Math.Min(n.InFlow, e.Flow));
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
                            if (p.InFlow != 0)
                            {
                                if (!p.SourceSide)
                                {
                                    continue;
                                }
                                else
                                {
                                    if (sourceRepaired && GetFlow(noCapSource, n) != null)
                                    {//TODO se funziona, ricordarsi di inserirlo anche negli altri 3 casi
                                        int flow = Math.Min(n.InFlow, n.NextNode.InFlow);
                                        //if (!m.SourceSide)
                                        flow = Math.Min(n.PreviousNode.InFlow, Math.Min(flow, n.PreviousEdge.Reversed ? n.PreviousEdge.Flow : n.PreviousEdge.Capacity));
                                        flow = Math.Min(flow, n.NextEdge.Reversed ? n.NextEdge.Flow : n.NextEdge.Capacity);
                                        if (flow > 0)
                                            return (flow, n);
                                    }
                                    int f = Math.Min(p.InFlow, n.InFlow);
                                    f = Math.Min(f, e.Capacity);
                                    if (f == 0)
                                        continue;
                                    n.SetPreviousEdge(e);
                                    n.SetPreviousNode(p);
                                    graph.AddLast(n);
                                    //graph.AddLast(p);
                                    e.SetReversed(false);
                                    //p.SetInFlow(f);
                                    return (f, n);
                                }
                            }
                            //p.SetSourceSide(false);
                            if (p.SourceSide && noCapSink is not SinkNode)
                                continue;
                            p.SetInFlow(Math.Min(e.Capacity, n.InFlow));
                            p.SetNextEdge(e);
                            p.SetNextNode(n);
                            graph.ChangeLabel(p, false, n.Label + 1);
                            e.SetReversed(false);
                            p.SetValid(true);
                            codaSink.Enqueue(p);
                        }
                        else if (element == p && e.Flow > 0)
                        {
                            if (n.InFlow != 0)
                                if (!n.SourceSide)
                                {
                                    continue;
                                }
                                else
                                {
                                    if (sourceRepaired && GetFlow(noCapSource, p) != null)
                                        return (Math.Min(p.InFlow, Math.Min(p.NextEdge.Reversed ? p.NextEdge.Flow : p.NextEdge.Capacity, p.NextNode.InFlow)), p);
                                    int f = Math.Min(p.InFlow, n.InFlow);
                                    f = Math.Min(f, e.Flow);
                                    if (f == 0)
                                        continue;
                                    p.SetPreviousEdge(e);
                                    p.SetPreviousNode(n);
                                    //graph.AddLast(n);
                                    graph.AddLast(p);
                                    e.SetReversed(true);
                                    //n.SetInFlow(f);
                                    return (f, p);
                                }
                            //n.SetSourceSide(false);
                            if (n.SourceSide && n is not SinkNode)
                                continue;
                            n.SetInFlow(Math.Min(e.Flow, p.InFlow));
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
            return (0, null);
        }
        private static (Node, Node, bool) SendFlow(Node n, int f)
        {
            n.SetInFlow(n.InFlow + f);
            Node vuotoSink = null;
            Node vuotoSource = null;
            Node momsource = n;
            Node momsink = n;
            while (momsource is not SourceNode)
            {
                var x = momsource.PreviousEdge.AddFlow(f);
                if (x.Item1)
                {
                    if (x.Item2)
                    {
                        vuotoSource = momsource;
                        momsource = n;
                        while (momsource != vuotoSource)
                        {
                            momsource.PreviousEdge.AddFlow(-f);
                            momsource.SetValid(true);
                            momsource.SetInFlow(momsource.InFlow + f);
                            momsource = momsource.PreviousNode;
                        }
                        return (vuotoSource, null, true);
                    }
                    vuotoSource = momsource;
                    momsource.SetValid(false);
                }
                momsource.SetInFlow(momsource.InFlow - f);
                momsource = momsource.PreviousNode;

            }
            while (momsink is not SinkNode)
            {
                var x = momsink.NextEdge.AddFlow(f);
                if (x.Item1)
                {
                    if (x.Item2)
                    {
                        vuotoSink = momsink;
                        momsink = n;
                        n.SetInFlow(n.InFlow - f);
                        while (momsink != vuotoSink)
                        {
                            momsink.NextEdge.AddFlow(-f);
                            momsink.SetValid(true);
                            momsink.SetInFlow(momsink.InFlow + f);
                            momsink = momsink.NextNode;
                        }
                        while (n is not SourceNode)
                        {
                            n.NextEdge.AddFlow(-f);
                            n.SetValid(true);
                            n.SetInFlow(n.InFlow + f);
                            n = n.NextNode;
                        }
                        return (null, vuotoSink, true);
                    }
                    vuotoSink = momsink;
                    momsink.SetValid(false);
                }
                momsink.SetInFlow(momsink.InFlow - f);
                momsink = momsink.NextNode;
            }
            return (vuotoSource, vuotoSink, false);
        }

        public static int FlowFordFulkerson(Graph graph)
        {
            var (fMax, node) = FirstBfs(graph);
            var (vuotoSource, vuotoSink, b) = SendFlow(node, fMax);
            while (true)
            {
                var (f, n) = DoBfs(graph, vuotoSource, vuotoSink);
                if (f == 0)
                    break;
                (vuotoSource, vuotoSink, b) = SendFlow(n, f);
                if (!b)
                    fMax += f;
            }
            return fMax;

        }


    }
}