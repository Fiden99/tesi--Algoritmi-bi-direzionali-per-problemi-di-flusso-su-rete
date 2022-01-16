using System;
using System.Collections.Generic;
using System.Linq;

namespace Bidirezionale.NodePropagation.LastLevelOpt
{
    public class BiNodePropagationLastLevelOpt
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
                    if (node == next && e.Capacity > 0 && previous.Label == (node.Label - 1) && previous.Valid)
                    {
                        //grafo.ChangeLabel(node, true, node.Label);
                        node.SetPreviousNode(previous);
                        node.SetPreviousEdge(e);
                        node.SetInFlow(Math.Min(e.Capacity, previous.InFlow));
                        e.SetReversed(false);
                        return true;
                    }
                    if (node == previous && e.Flow > 0 && next.Label == (node.Label - 1) && next.Valid)
                    {
                        //grafo.ChangeLabel(node, true, node.Label);
                        node.SetPreviousNode(next);
                        node.SetPreviousEdge(e);
                        node.SetInFlow(Math.Min(e.Flow, next.InFlow));
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
                        foreach (var n in graph.LastNodesSinkSide.Where(x => x.Valid))
                        {
                            //TODO capire se devo fare un controllo di inflow tra n, il predecessore e il successore ( con nodi)
                            if (GetFlow(noCapSource, n) != null && n.InFlow != 0)
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
                            if (sourceRepaired && GetFlow(noCapSource, n) != null)
                                sourceFlow = n.InFlow;
                            else
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
                    if (!element.SourceSide)
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
                                if (n.InFlow != 0)
                                    if (n.SourceSide)
                                        continue;
                                    else
                                    {
                                        int f = Math.Min(n.InFlow, p.InFlow);
                                        f = Math.Min(f, e.Capacity);
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
                                    {
                                        continue;
                                    }
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
                    if (element.SourceSide)
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
                                if (p.InFlow != 0)
                                {
                                    if (!p.SourceSide)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        if (sourceRepaired)
                                        {
                                            Node m = GetFlow(noCapSource, n);
                                            if (m != null && m.InFlow != 0)
                                            {//TODO se funziona, ricordarsi di inserirlo anche negli altri 3 casi
                                                int flow = Math.Min(m.InFlow, n.NextNode.InFlow);
                                                if (!m.SourceSide)
                                                    flow = Math.Min(m.PreviousNode.InFlow, Math.Min(flow, m.PreviousEdge.Reversed ? m.PreviousEdge.Flow : m.PreviousEdge.Capacity));
                                                if (n.NextEdge.Reversed)
                                                    return (Math.Min(flow, n.NextEdge.Flow), n);
                                                else
                                                    return (Math.Min(flow, n.NextEdge.Capacity), n);
                                            }
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
                                        int f = Math.Min(p.InFlow, n.InFlow);
                                        f = Math.Min(f, e.Flow);
                                        if (sourceRepaired)
                                        {
                                            Node m = GetFlow(noCapSource, p);
                                            if (m != null)
                                                return (Math.Min(m.InFlow, Math.Min(p.NextEdge.Flow, p.NextNode.InFlow)), p);
                                            else
                                                return (Math.Min(m.InFlow, Math.Min(p.NextEdge.Capacity, p.NextNode.InFlow)), p);
                                        }
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

        public static int FlowFordFulkerson(Graph graph)
        {
            Node s = graph.Source;
            Node t = graph.Sink;
            Node vuotoSource = s;
            Node vuotoSink = t;
            int fMax = 0;
            while (true)
            {
                bool removedFlow = false;
                var (f, n) = DoBfs(graph, vuotoSource, vuotoSink);
                if (f == 0)
                    break;
                fMax += f;
                n.SetInFlow(n.InFlow + f);
                vuotoSink = null;
                vuotoSource = null;
                Node momsource = n;
                Node momsink = n;
                while (momsource != s)
                {
                    try
                    {
                        if (momsource.PreviousEdge.AddFlow(f))
                        {
                            vuotoSource = momsource;
                            momsource.SetValid(false);
                        }
                        momsource.SetInFlow(momsource.InFlow - f);
                        momsource = momsource.PreviousNode;
                    }
                    catch (ArgumentException)
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
                        fMax -= f;
                        removedFlow = true;
                        break;
                    }
                }
                while (momsink != t)
                {
                    try
                    {
                        if (momsink.NextEdge.AddFlow(f))
                        {
                            vuotoSink = momsink;
                            momsink.SetValid(false);
                        }
                        momsink.SetInFlow(momsink.InFlow - f);
                        momsink = momsink.NextNode;
                    }
                    catch (ArgumentException)
                    {
                        vuotoSink = momsink;
                        momsink = n;
                        while (momsink != vuotoSink)
                        {
                            momsink.NextEdge.AddFlow(-f);
                            momsink.SetValid(true);
                            momsink.SetInFlow(momsink.InFlow + f);
                            momsink = momsink.NextNode;
                        }
                        if (!removedFlow)
                        {
                            while (momsource != s)
                            {
                                momsource.PreviousEdge.AddFlow(-f);
                                momsource.SetValid(true);
                                momsource.SetInFlow(momsource.InFlow + f);
                                momsource = momsource.PreviousNode;
                            }
                            fMax -= f;
                        }
                        break;
                    }
                }
            }
            return fMax;

        }
    }
}