using System;
using System.Collections.Generic;
using System.Linq;

namespace Bidirezionale.NodeCount.SickPropagation
{
    public class BiNodeCountSickPropagation
    {
        public static bool RepairNode(Node node, bool borderForward)
        {
            if (node is SourceNode || node is SinkNode)
                return false;
            if ((node.PreviousEdge != null && node.NextEdge != null && node.PreviousNode.SourceValid && node.NextNode.SinkValid && (node.PreviousEdge.Reversed ? node.PreviousEdge.Flow : node.PreviousEdge.Capacity) > 0 && (node.NextEdge.Reversed ? node.NextEdge.Flow : node.NextEdge.Capacity) > 0) || (node.PreviousEdge != null && node.PreviousNode.SourceValid && (node.PreviousEdge.Reversed ? node.PreviousEdge.Flow : node.PreviousEdge.Capacity) > 0 && node.NextEdge == null) || (node.NextEdge != null && node.NextNode.SinkValid && (node.NextEdge.Reversed ? node.NextEdge.Flow : node.NextEdge.Capacity) > 0 && node.PreviousEdge == null))
                return true;
            if (node.SourceSide)
            {
                foreach (var e in node.Edges)
                {
                    Node previous = e.PreviousNode;
                    Node next = e.NextNode;
                    if (previous.SourceSide != next.SourceSide)
                        continue;
                    if (node == next && e.Capacity > 0 && previous.Label == (node.Label - 1) && previous.SourceValid && previous.InFlow > 0)
                    {
                        //grafo.ChangeLabel(node, true, node.Label);
                        node.SetPreviousNode(previous);
                        node.SetPreviousEdge(e);
                        node.SetInFlow(Math.Min(e.Capacity, previous.InFlow));
                        e.SetReversed(false);
                        node.SetSourceValid(true);
                        return true;
                    }
                    if (node == previous && e.Flow > 0 && next.Label == (node.Label - 1) && next.SourceValid && next.InFlow > 0)
                    {
                        //grafo.ChangeLabel(node, true, node.Label);
                        node.SetPreviousNode(next);
                        node.SetPreviousEdge(e);
                        node.SetInFlow(Math.Min(e.Flow, next.InFlow));
                        e.SetReversed(true);
                        node.SetSourceValid(true);
                        return true;
                    }
                }
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
                        if (next == node && e.Capacity > 0 && previous.SourceValid && previous.SourceSide && previous.InFlow > 0)
                        {
                            node.SetPreviousEdge(e);
                            node.SetPreviousNode(previous);
                            node.SetInFlow(Math.Min(e.Capacity, previous.InFlow));
                            e.SetReversed(false);
                            node.SetSourceValid(true);
                            return true;
                        }
                        if (previous == node && e.Flow > 0 && next.SourceValid && next.SourceSide && next.InFlow > 0)
                        {
                            node.SetPreviousEdge(e);
                            node.SetPreviousNode(next);
                            node.SetInFlow(Math.Min(e.Flow, previous.InFlow));
                            e.SetReversed(true);
                            node.SetSourceValid(true);
                            return true;
                        }

                    }
                    else if (borderForward && next.SourceSide == previous.SourceSide)
                    {
                        if (node == previous && e.Capacity > 0 && next.SinkValid && node.Label == (next.Label + 1) && next.InFlow > 0)
                        {
                            node.SetNextEdge(e);
                            node.SetNextNode(next);
                            node.SetInFlow(Math.Min(e.Capacity, next.InFlow));
                            e.SetReversed(false);
                            node.SetSinkValid(true);
                            return true;
                        }
                        if (node == next && e.Flow > 0 && previous.SinkValid && node.Label == (previous.Label + 1) && previous.InFlow > 0)
                        {
                            node.SetNextEdge(e);
                            node.SetNextNode(previous);
                            node.SetInFlow(Math.Min(previous.InFlow, e.Flow));
                            e.SetReversed(true);
                            node.SetSinkValid(true);
                            return true;
                        }
                    }

                }
            }
            if (node.SourceSide || !borderForward)
                node.SetSourceValid(false);
            else
                node.SetSinkValid(false);
            return false;
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
                    n.SetInFlow(Math.Min(n.PreviousEdge.Reversed ? n.PreviousEdge.Flow : n.PreviousEdge.Capacity, target.InFlow));
                    return n;
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
                    n.SetInFlow(Math.Min(n.NextEdge.Reversed ? n.NextEdge.Flow : n.NextEdge.Capacity, target.InFlow));
                    return n;
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
            Queue<Node> malati = new();
            Node noCapSource = null;
            Node noCapSink = null;
            bool sourceRepaired = noCapsSource.Count == 0;
            bool sinkRepaired = noCapsSink.Count == 0;
            Queue<BiEdge> codaEdgeSource = new();
            Queue<BiEdge> codaEdgeSink = new();
            Node elementSource = null;
            Node elementSink = null;

            if (noCapsSource.Count > 0)
            {
                Node p = null;
                bool repaired = true;
                Node momNoCap = null;
                while (noCapsSource.Count > 0)
                {
                    noCapSource = noCapsSource.Pop();
                    GetFlow(p, noCapSource);
                    p = noCapSource;
                    if (!RepairNode(noCapSource, false))
                    {// serve per confermare che ho riparato tutti i nodi
                        malati.Enqueue(noCapSource);
                        repaired = false;
                        momNoCap ??= noCapSource;
                    }
                }
                noCapSource = momNoCap;
                if (repaired && sinkRepaired)
                    foreach (var n in graph.LastNodesSinkSide.Where(x => x.SourceValid && x.SinkValid))
                        if (GetFlow(p, n) != null && n.InFlow != 0)
                            return (Math.Min(n.InFlow, Math.Min(n.NextEdge.Reversed ? n.NextEdge.Flow : n.NextEdge.Capacity, n.NextNode.InFlow)), n);
                //parte di sickPropagation
                Node malato = null;
                while (malati.Count > 0)
                {
                    var m = SourceSickPropagation(graph, malati.Dequeue(), codaSource);
                    malato ??= m; // malato = malato == null ? m : malato
                }
                if (malato != null)//&& (malato.NextEdge.Reversed ? malato.NextEdge.Flow : malato.NextEdge.Capacity) > 0 && malato.NextNode.InFlow > 0)
                    return (Math.Min(Math.Min(malato.NextEdge.Reversed ? malato.NextEdge.Flow : malato.NextEdge.Capacity, malato.NextNode.InFlow), malato.InFlow), malato);
                if (sinkRepaired)
                    foreach (var n in graph.LastNodesSinkSide.Where(x => (x.SourceValid && x.NextEdge.Reversed ? x.NextEdge.Flow : x.NextEdge.Capacity) > 0 && x.NextNode.InFlow > 0))
                    {
                        //TODO capire come si può migliorare
                        //TODO qui c'è un problema
                        int mom = n.InFlow;
                        int f = GetFlow(graph.Source, n).InFlow;
                        if (f != 0)
                            return (Math.Min(f, Math.Min(n.NextNode.InFlow, n.NextEdge.Reversed ? n.NextEdge.Flow : n.NextEdge.Capacity)), n);
                        n.SetInFlow(mom);
                    }
                sourceRepaired = repaired;
                // fine parte di sickpropagation
                if (!repaired && codaSource.Count == 0)
                {
                    if (noCapSource is SourceNode)
                    {
                        codaSource.Enqueue(p);
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
                Node p = null;
                Node momNoCap = null;
                while (noCapsSink.Count > 0)
                {
                    noCapSink = noCapsSink.Pop();
                    GetFlow(p, noCapSink);
                    p = noCapSink;
                    if (!RepairNode(noCapSink, true))
                    {
                        malati.Enqueue(p);
                        repaired = false;
                        momNoCap ??= noCapSink;
                    }
                }
                noCapSink = momNoCap;
                if (repaired && sourceRepaired)
                {
                    foreach (var n in graph.LastNodesSinkSide.Where(x => x.SinkValid && x.SourceValid))
                    {
                        int sourceFlow = Math.Min(n.PreviousNode.InFlow, n.PreviousEdge.Reversed ? n.PreviousEdge.Flow : n.PreviousEdge.Capacity);
                        if (sourceFlow > 0 && GetFlow(p, n) != null && n.InFlow != 0)
                            return (Math.Min(n.InFlow, sourceFlow), n);
                    }
                }
                Node malato = null;
                while (malati.Count > 0)
                    malato ??= SickPropagationSink(malati.Dequeue(), codaSink);
                if (malato != null)
                    return (Math.Min(Math.Min(malato.InFlow, malato.PreviousEdge.Reversed ? malato.PreviousEdge.Flow : malato.PreviousEdge.Capacity), malato.PreviousNode.InFlow), malato);
                foreach (var n in graph.LastNodesSinkSide.Where(x => x.SinkValid && (x.PreviousEdge.Reversed ? x.PreviousEdge.Flow : x.PreviousEdge.Capacity) > 0 && x.PreviousNode.InFlow > 0))
                {
                    int mom = n.InFlow;
                    int f = GetFlow(graph.Sink, n).InFlow;
                    if (f != 0)
                        return (Math.Min(Math.Min(f, n.PreviousEdge.Reversed ? n.PreviousEdge.Flow : n.PreviousEdge.Capacity), n.PreviousNode.InFlow), n);
                    n.SetInFlow(mom);
                }
                sinkRepaired = repaired;
                if (!repaired && codaSink.Count == 0)
                    if (noCapSink is SinkNode)
                    {
                        codaSink.Enqueue(p);
                    }
                    else
                    {
                        foreach (var n in graph.LabeledNodeSinkSide[noCapSink.Label - 1])
                            codaSink.Enqueue(n);
                        graph.ResetSinkSide(noCapSink.Label);
                    }
            }
            // ricerca normale del flusso
            while (codaSink.Count > 0 || codaSource.Count > 0)
            {
                if (codaSource.Count > 0 && (codaEdgeSource.Count == 0 || (codaEdgeSink.Count == 0 && codaSink.Count == 0 && !sinkRepaired)))
                {
                    elementSource = codaSource.Dequeue();
                    if (!elementSource.SourceSide || !elementSource.SourceValid || elementSource.InFlow == 0)
                        continue;
                    foreach (var e in elementSource.Edges.Where(x => (x.PreviousNode == elementSource && x.Capacity > 0 && (x.NextNode.InFlow == 0 || !x.NextNode.SourceSide)) || (x.NextNode == elementSource && x.Flow > 0 && (x.PreviousNode.InFlow == 0 || !x.PreviousNode.SourceSide))))
                        codaEdgeSource.Enqueue(e);
                }
                if (codaSink.Count > 0 && (codaEdgeSink.Count == 0 || (codaEdgeSource.Count == 0 && codaSource.Count == 0 && !sourceRepaired)))
                {
                    elementSink = codaSink.Dequeue();
                    if (elementSink.SourceSide || !elementSink.SinkValid || elementSink.InFlow == 0)
                        continue;
                    foreach (var e in elementSink.Edges.Where(x => (x.NextNode == elementSink && x.Capacity > 0 && (x.PreviousNode.InFlow == 0 || x.PreviousNode.SourceSide)) || (x.PreviousNode == elementSink && x.Flow > 0 && (x.NextNode.InFlow == 0 || x.NextNode.SourceSide))))
                        codaEdgeSink.Enqueue(e);
                }
                while ((codaEdgeSource.Count > 0 || sourceRepaired) && (codaEdgeSink.Count > 0 || sinkRepaired))
                {
                    if (codaEdgeSource.Count > 0)
                    {
                        var e = codaEdgeSource.Dequeue();
                        Node p = e.PreviousNode;
                        Node n = e.NextNode;
#if DEBUG
                        if (e.Capacity < 0 || e.Flow < 0)
                            throw new InvalidOperationException("capacità negativa");
#endif
                        if (elementSource == p && e.Capacity > 0)
                        {
                            if (n.SourceSide && n.InFlow == 0)
                            {
                                n.SetInFlow(Math.Min(p.InFlow, e.Capacity));
                                graph.ChangeLabel(n, true, p.Label + 1);
                                n.SetPreviousNode(p);
                                n.SetPreviousEdge(e);
                                e.SetReversed(false);
                                n.SetSourceValid(true);
                                codaSource.Enqueue(n);
                            }
                            else if (n.InFlow != 0 && !n.SourceSide && n.SinkValid)
                            {
                                int f = Math.Min(n.InFlow, Math.Min(p.InFlow, e.Capacity));
                                if (f == 0)
                                    continue;
                                n.SetSourceValid(true);
                                n.SetPreviousNode(p);
                                n.SetPreviousEdge(e);
                                //graph.AddLast(p);
                                graph.AddLast(n);
                                e.SetReversed(false);
                                //n.SetInFlow(f);
                                return (f, n);
                            }
                            else if (n.InFlow == 0 && !n.SourceSide && sinkRepaired)
                            {
                                //TODO capire se è il metodo giusto, o ci sono dei metodi migliori
                                foreach (var edge in n.Edges)
                                    if (edge.PreviousNode == n && edge.NextNode.InFlow > 0 && !edge.NextNode.SourceSide && edge.Capacity > 0)
                                    {
                                        graph.ChangeLabel(n, false, edge.NextNode.Label + 1);
                                        n.SetInFlow(Math.Min(edge.Capacity, edge.NextNode.InFlow));
                                        n.SetNextEdge(edge);
                                        n.SetNextNode(edge.NextNode);
                                        edge.SetReversed(false);
                                        n.SetSinkValid(true);
                                        break;
                                    }
                                    else if (edge.NextNode == n && edge.PreviousNode.InFlow > 0 && !edge.PreviousNode.SourceSide && edge.Flow > 0)
                                    {
                                        graph.ChangeLabel(n, false, edge.PreviousNode.Label + 1);
                                        n.SetInFlow(Math.Min(edge.Flow, edge.PreviousNode.InFlow));
                                        n.SetNextEdge(edge);
                                        n.SetNextNode(edge.PreviousNode);
                                        edge.SetReversed(true);
                                        n.SetSinkValid(true);
                                        break;
                                    }
                                if (n.InFlow > 0)
                                {
                                    int f = Math.Min(n.InFlow, Math.Min(p.InFlow, e.Reversed ? e.Flow : e.Capacity));
                                    if (f == 0)
                                        continue;
                                    n.SetSourceValid(true);
                                    n.SetPreviousNode(p);
                                    n.SetPreviousEdge(e);
                                    //graph.AddLast(p);
                                    graph.AddLast(n);
                                    e.SetReversed(false);
                                    n.SetInFlow(f);
                                    return (f, n);
                                }
                                Node mom = n;
                                Node malato = null;
                                while (mom is not SinkNode)
                                {
                                    if ((mom.NextEdge.Reversed ? mom.NextEdge.Flow : mom.NextEdge.Capacity) == 0)
                                        malato = mom;
                                    mom = mom.NextNode;
                                }
                                sinkRepaired = false;
                                foreach (var node in graph.LabeledNodeSinkSide[malato.Label - 1])
                                    codaSink.Enqueue(node);
                                graph.ResetSinkSide(malato.Label);
                            }
                        }
                        else if (elementSource == n && e.Flow > 0)
                        {
                            if (p.SourceSide && p.InFlow == 0)
                            {
                                p.SetInFlow(Math.Min(n.InFlow, e.Flow));
                                graph.ChangeLabel(p, true, n.Label + 1);
                                p.SetPreviousEdge(e);
                                p.SetPreviousNode(n);
                                e.SetReversed(true);
                                p.SetSourceValid(true);
                                codaSource.Enqueue(p);
                            }
                            else if (p.InFlow != 0 && !p.SourceSide && p.SinkValid)
                            {
                                int f = Math.Min(p.InFlow, n.InFlow);
                                f = Math.Min(f, e.Flow);
                                if (f == 0)
                                    continue;
                                p.SetSourceValid(true);
                                p.SetPreviousNode(n);
                                p.SetPreviousEdge(e);
                                graph.AddLast(p);
                                //graph.AddLast(n);
                                e.SetReversed(true);
                                //p.SetInFlow(f);
                                return (f, p);
                            }
                            else if (p.InFlow == 0 && !p.SourceSide && sinkRepaired)
                            {
                                //TODO capire se è giusto e se ci sono dei metodi migliori
                                foreach (var edge in p.Edges)
                                    if (edge.PreviousNode == p && edge.NextNode.InFlow > 0 && !edge.NextNode.SourceSide && edge.Capacity > 0)
                                    {
                                        graph.ChangeLabel(p, false, edge.NextNode.Label + 1);
                                        p.SetInFlow(Math.Min(edge.Capacity, edge.NextNode.InFlow));
                                        p.SetNextEdge(edge);
                                        p.SetNextNode(edge.NextNode);
                                        edge.SetReversed(false);
                                        p.SetSinkValid(true);
                                        break;
                                    }
                                    else if (edge.NextNode == p && edge.PreviousNode.InFlow > 0 && !edge.PreviousNode.SourceSide && edge.Flow > 0)
                                    {
                                        graph.ChangeLabel(p, false, edge.PreviousNode.Label + 1);
                                        p.SetInFlow(Math.Min(edge.Flow, edge.PreviousNode.InFlow));
                                        p.SetNextEdge(edge);
                                        p.SetNextNode(edge.PreviousNode);
                                        edge.SetReversed(true);
                                        p.SetSinkValid(true);
                                        break;
                                    }
                                if (p.InFlow > 0)
                                {
                                    int f = Math.Min(p.InFlow, n.InFlow);
                                    f = Math.Min(f, e.Reversed ? e.Flow : e.Capacity);
                                    p.SetPreviousNode(n);
                                    p.SetPreviousEdge(e);
                                    graph.AddLast(p);
                                    //graph.AddLast(n);
                                    e.SetReversed(true);
                                    p.SetInFlow(f);
                                    return (f, p);
                                }
                                Node mom = p;
                                Node malato = null;
                                while (mom is not SinkNode)
                                {
                                    if ((mom.NextEdge.Reversed ? mom.NextEdge.Flow : mom.NextEdge.Capacity) == 0)
                                        malato = mom;
                                    mom = mom.NextNode;
                                }
                                sinkRepaired = false;
                                foreach (var node in graph.LabeledNodeSinkSide[malato.Label - 1])
                                    codaSink.Enqueue(node);
                                graph.ResetSinkSide(malato.Label);

                            }
                        }
                    }
                    if (codaEdgeSink.Count > 0)
                    {
                        var e = codaEdgeSink.Dequeue();
                        var p = e.PreviousNode;
                        var n = e.NextNode;
#if DEBUG
                        if (e.Capacity < 0 || e.Flow < 0)
                            throw new InvalidOperationException("capacità negativa");
#endif
                        if (elementSink == n && e.Capacity > 0)
                        {
                            if (p.InFlow != 0)
                            {
                                if (!p.SourceSide || !p.SourceValid)
                                {
                                    continue;
                                }
                                else
                                {
                                    int f = Math.Min(p.InFlow, n.InFlow);
                                    f = Math.Min(f, e.Capacity);
                                    if (f == 0)
                                        continue;
                                    n.SetPreviousEdge(e);
                                    n.SetPreviousNode(p);
                                    graph.AddLast(n);
                                    //graph.AddLast(p);
                                    e.SetReversed(false);
                                    n.SetSinkValid(true);
                                    //p.SetInFlow(f);
                                    return (f, n);
                                }
                            }
                            //p.SetSourceSide(false);
                            p.SetInFlow(Math.Min(e.Capacity, n.InFlow));
                            p.SetNextEdge(e);
                            p.SetNextNode(n);
                            graph.ChangeLabel(p, false, n.Label + 1);
                            e.SetReversed(false);
                            p.SetSinkValid(true);
                            codaSink.Enqueue(p);
                        }
                        else if (elementSink == p && e.Flow > 0)
                        {
                            if (n.InFlow != 0)
                                if (!n.SourceSide || !p.SourceValid)
                                {
                                    continue;
                                }
                                else
                                {
                                    int f = Math.Min(p.InFlow, n.InFlow);
                                    f = Math.Min(f, e.Flow);
                                    if (f == 0)
                                        continue;
                                    p.SetPreviousEdge(e);
                                    p.SetPreviousNode(n);
                                    //graph.AddLast(n);
                                    graph.AddLast(p);
                                    e.SetReversed(true);
                                    p.SetSinkValid(true);
                                    //n.SetInFlow(f);
                                    return (f, p);
                                }
                            //n.SetSourceSide(false);
                            n.SetInFlow(Math.Min(e.Flow, p.InFlow));
                            n.SetNextEdge(e);
                            n.SetNextNode(p);
                            graph.ChangeLabel(n, false, p.Label + 1);
                            e.SetReversed(true);
                            n.SetSinkValid(true);
                            codaSink.Enqueue(n);
                        }
                    }
                }
            }
            return (0, null);
        }

        private static Node SickPropagationSink(Node node, Queue<Node> codaSink)
        {
            Queue<Node> malati = new();
            malati.Enqueue(node);
            while (malati.Count > 0)
            {
                var m = malati.Dequeue();
                if (!m.SourceSide)
                {
                    if (!RepairNode(m, true))
                    {
                        foreach (var e in m.Edges)
                            if (e.NextNode == m && e == e.PreviousNode.NextEdge)
                                malati.Enqueue(e.PreviousNode);
                            else if (e.PreviousNode == m && e == e.NextNode.NextEdge)
                                malati.Enqueue(e.NextNode);
                    }
                    else if (m.PreviousEdge != null && (m.PreviousEdge.Reversed ? m.PreviousEdge.Flow : m.PreviousEdge.Capacity) > 0 && m.PreviousNode.InFlow > 0)
                        return m;
                    else
                        codaSink.Enqueue(m);
                }
            }
            return null;
        }

        private static Node SourceSickPropagation(Graph graph, Node node, Queue<Node> codaSource)
        {
            Queue<Node> malati = new();
            malati.Enqueue(node);
            while (malati.Count > 0)
            {
                Node m = malati.Dequeue();
                if (m.SourceSide || graph.LastNodesSinkSide.Contains(m))
                    if (!RepairNode(m, false))
                    {
                        foreach (var e in m.Edges)
                            if (e.PreviousNode == m && e == e.NextNode.PreviousEdge)
                                malati.Enqueue(e.NextNode);
                            else if (e.NextNode == m && e == e.PreviousNode.PreviousEdge)
                                malati.Enqueue(e.PreviousNode);
                    }
                    else if (m.NextEdge != null && (m.NextEdge.Reversed ? m.NextEdge.Flow : m.NextEdge.Capacity) > 0 && m.NextNode.InFlow > 0)
                        return m;
                    else if (m.SourceSide)
                        codaSource.Enqueue(m);
            }
            return null;

        }

        public static int FlowFordFulkerson(Graph graph)
        {
            Node s = graph.Source;
            Node t = graph.Sink;
            Stack<Node> vuotiSource = new();
            Stack<Node> vuotiSink = new();
            vuotiSource.Push(s);
            vuotiSink.Push(t);
            int fMax = 0;
            while (true)
            {
                var (f, n) = DoBfs(graph, vuotiSource, vuotiSink);
                if (f == 0)
                    break;
                n.SetInFlow(n.InFlow + f);
                vuotiSource.Clear();
                vuotiSink.Clear();
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
                        momsource.SetSourceValid(false);
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
                        momsink.SetSinkValid(false);
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
