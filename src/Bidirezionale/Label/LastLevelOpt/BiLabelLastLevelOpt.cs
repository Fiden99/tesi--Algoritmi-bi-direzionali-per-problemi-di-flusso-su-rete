using System;
using System.Collections.Generic;
using System.Linq;

namespace Bidirezionale.Label.LastLevelOpt
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
                    if (node == next && e.Capacity > 0 && previous.Label == (node.Label - 1) && previous.Valid && previous.InFlow > 0)
                    {
                        //grafo.ChangeLabel(node, true, node.Label);
                        node.SetPreviousNode(previous);
                        node.SetPreviousEdge(e);
                        node.SetInFlow(Math.Min(e.Capacity, previous.InFlow));
                        e.SetReversed(false);
                        node.SetValid(true);
                        return true;
                    }
                    if (node == previous && e.Flow > 0 && next.Label == (node.Label - 1) && next.Valid && next.InFlow > 0)
                    {
                        //grafo.ChangeLabel(node, true, node.Label);
                        node.SetPreviousNode(next);
                        node.SetPreviousEdge(e);
                        node.SetInFlow(Math.Min(e.Flow, next.InFlow));
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
                        if (next == node && e.Capacity > 0 && previous.Valid && graph.LastNodesSourceSide.Contains(previous) && previous.InFlow > 0)
                        {
                            node.SetPreviousEdge(e);
                            node.SetPreviousNode(previous);
                            node.SetInFlow(Math.Min(e.Capacity, previous.InFlow));
                            e.SetReversed(false);
                            node.SetValid(true);
                            return true;
                        }
                        if (previous == node && e.Flow > 0 && next.Valid && graph.LastNodesSourceSide.Contains(next) && next.InFlow > 0)
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
                        if (node == previous && e.Capacity > 0 && next.Valid && node.Label == (next.Label + 1) && next.InFlow > 0)
                        {
                            node.SetNextEdge(e);
                            node.SetNextNode(next);
                            node.SetInFlow(Math.Min(e.Capacity, next.InFlow));
                            e.SetReversed(false);
                            node.SetValid(true);
                            return true;
                        }
                        if (node == next && e.Flow > 0 && previous.Valid && node.Label == (previous.Label + 1) && previous.InFlow > 0)
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
            if (target == null)
                return null;
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
        public static (int, Node) DoBfs(Graph graph, Stack<Node> noCapsSource, Stack<Node> noCapsSink)
        {
            Queue<Node> codaSource = new();
            Queue<Node> codaSink = new();
            Queue<Node> buffer = new();
            Node noCapSource = null;
            Node noCapSink = null;
            if (noCapsSource.Count > 0)
            {
                Node p = null;
                bool repaired = true;
                while (noCapsSource.Count > 0)
                {
                    noCapSource = noCapsSource.Pop();
                    GetFlow(p, noCapSource);
                    p = noCapSource;
                    if (!RepairNode(graph, noCapSource, false))
                    {// sere per confermare che ho riparato tutti i nodi
                        noCapsSource.Push(noCapSource);
                        repaired = false;
                        break;
                    }
                }
                if (repaired && noCapsSink.Count == 0)
                    foreach (var n in graph.LastNodesSinkSide.Where(x => x.Valid))
                        if (GetFlow(p, n) != null && n.InFlow != 0)
                            return (Math.Min(n.InFlow, Math.Min(n.NextEdge.Reversed ? n.NextEdge.Flow : n.NextEdge.Capacity, n.NextNode.InFlow)), n);
                if (!repaired)
                {
                    if (p is SourceNode)
                    {
                        codaSource.Enqueue(p);
                    }
                    else if (!p.SourceSide)
                    {
                        foreach (var n in graph.LastNodesSourceSide)
                            codaSource.Enqueue(n);
                    }
                    else
                    {
                        foreach (var n in graph.LabeledNodeSourceSide[p.Label - 1])
                            codaSource.Enqueue(n);
                        graph.ResetSourceSide(p.Label);
                    }
                }
            }
            if (noCapsSink.Count > 0)
            {
                bool repaired = true;
                Node p = null;
                while (noCapsSink.Count > 0)
                {
                    noCapSink = noCapsSink.Pop();
                    GetFlow(p, noCapSink);
                    p = noCapSink;
                    if (!RepairNode(graph, noCapSink, true))
                    {
                        noCapsSink.Push(p);
                        repaired = false;
                        break;
                    }
                }
                if (repaired && noCapsSource.Count == 0)
                {
                    foreach (var n in graph.LastNodesSinkSide.Where(x => x.Valid))
                    {
                        int sourceFlow = Math.Min(n.PreviousNode.InFlow, n.PreviousEdge.Reversed ? n.PreviousEdge.Flow : n.PreviousEdge.Capacity);
                        if (sourceFlow > 0 && GetFlow(p, n) != null && n.InFlow != 0)
                            return (Math.Min(n.InFlow, sourceFlow), n);
                    }
                }
                if (!repaired)
                    if (p is SinkNode)
                    {
                        codaSink.Enqueue(p);
                    }
                    else
                    {
                        foreach (var n in graph.LabeledNodeSinkSide[p.Label - 1])
                            codaSink.Enqueue(n);
                        graph.ResetSinkSide(p.Label);
                    }
            }


            while (codaSink.Count > 0 || codaSource.Count > 0)
            {
                while (codaSource.Count > 0 && noCapsSource.Count > 0)
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
                            /*if (!n.SourceSide && n is not SinkNode)
                            {
                                // TODO da valutare se è giusto
                                n.SetPreviousNode(p);
                                n.SetPreviousEdge(e);
                                e.SetReversed(false);

                                sinkRepaired = false;
                                foreach (var node in graph.LabeledNodeSinkSide[n.Label - 1])
                                    codaSink.Enqueue(node);
                                graph.ResetSinkSide(n.Label);
                                continue;
                            }*/
                            //n.SetSourceSide(true);
                            n.SetInFlow(Math.Min(p.InFlow, e.Capacity));
                            graph.ChangeLabel(n, true, p.Label + 1);
                            n.SetPreviousNode(p);
                            n.SetPreviousEdge(e);
                            e.SetReversed(false);
                            n.SetValid(true);
                            buffer.Enqueue(n);
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
                            /*                              if (!p.SourceSide && p is not SinkNode)
                                                            {
                                                                sinkRepaired = false;
                                                                foreach (var node in graph.LabeledNodeSinkSide[p.Label - 1])
                                                                codaSink.Enqueue(node);
                                                                graph.ResetSinkSide(n.Label);
                                                                continue;
                                                            }
                            */
                            p.SetInFlow(Math.Min(n.InFlow, e.Flow));
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
                                    /*
                                    if (sourceRepaired && GetFlow(noCapSource, n) != null)
                                    {//TODO se funziona, ricordarsi di inserirlo anche negli altri 3 casi
                                        int flow = Math.Min(n.InFlow, n.NextNode.InFlow);
                                        //if (!m.SourceSide)
                                        flow = Math.Min(n.PreviousNode.InFlow, Math.Min(flow, n.PreviousEdge.Reversed ? n.PreviousEdge.Flow : n.PreviousEdge.Capacity));
                                        flow = Math.Min(flow, n.NextEdge.Reversed ? n.NextEdge.Flow : n.NextEdge.Capacity);
                                        if (flow > 0)
                                            return (flow, n);
                                    }
                                    */
                                    int f = Math.Min(p.InFlow, Math.Min(n.InFlow, e.Capacity));
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
                            //if (p.SourceSide && noCapSink is not SinkNode)
                            //continue;
                            p.SetInFlow(Math.Min(e.Capacity, n.InFlow));
                            p.SetNextEdge(e);
                            p.SetNextNode(n);
                            graph.ChangeLabel(p, false, n.Label + 1);
                            e.SetReversed(false);
                            p.SetValid(true);
                            buffer.Enqueue(p);
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
                                    /*if (sourceRepaired && GetFlow(noCapSource, p) != null)
                                        return (Math.Min(p.InFlow, Math.Min(p.NextEdge.Reversed ? p.NextEdge.Flow : p.NextEdge.Capacity, p.NextNode.InFlow)), p);
                                    */
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
                            //if (n.SourceSide && n is not SinkNode)
                            //continue;
                            n.SetInFlow(Math.Min(e.Flow, p.InFlow));
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
            return (0, null);
        }

        public static int FlowFordFulkerson(Graph graph)
        {
            Node s = graph.Source;
            Node t = graph.Sink;
            Stack<Node> vuotiSource = new();
            Stack<Node> vuotiSink = new();
            int fMax = 0;
            vuotiSource.Push(s);
            vuotiSink.Push(t);
            while (true)
            {
                var (f, n) = DoBfs(graph, vuotiSource, vuotiSink);
                if (f == 0)
                    break;
                n.SetInFlow(n.InFlow + f);
                vuotiSink.Clear();
                vuotiSource.Clear();
                Node momsource = n;
                Node momsink = n;
                while (momsource != s)
                {
                    var x = momsource.PreviousEdge.AddFlow(f);
                    if (x.Item1) // cap = 0
                    {
                        if (x.Item2) // cap < 0
                        {
                            vuotiSource.Clear();
                            int flowError = GetFlow(s, n).InFlow;
                            Node mom = n;
                            while (mom != momsource)
                            {
                                mom.SetInFlow(mom.InFlow - flowError);
                                mom.PreviousEdge.AddFlow(flowError);
                                mom = mom.PreviousNode;
                            }
                            f += flowError;
                        }
                        vuotiSource.Push(momsource);
                        momsource.SetValid(false);
                    }
                    momsource.SetInFlow(momsource.InFlow - f);
                    momsource = momsource.PreviousNode;

                }
                while (momsink != t)
                {
                    var x = momsink.NextEdge.AddFlow(f);
                    if (x.Item1)
                    {
                        if (x.Item2)
                        {
                            vuotiSink.Clear();
                            int flowError = GetFlow(t, n).InFlow;
                            Node mom = n;
                            while (mom != momsink)
                            {
                                mom.SetInFlow(mom.InFlow - flowError);
                                mom.NextEdge.AddFlow(flowError);
                                mom = mom.NextNode;
                            }
                            mom = n;
                            while (mom != s)
                            {
                                mom.SetInFlow(mom.InFlow - flowError);
                                mom.PreviousEdge.AddFlow(flowError);
                                mom = mom.PreviousNode;
                            }
                            f += flowError;
                        }
                        vuotiSink.Push(momsink);
                        momsink.SetValid(false);
                    }
                    momsink.SetInFlow(momsink.InFlow - f);
                    momsink = momsink.NextNode;
                }
                fMax += f;
            }
            return fMax;

        }
    }
}
