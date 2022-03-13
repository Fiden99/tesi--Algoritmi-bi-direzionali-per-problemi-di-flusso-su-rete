using System;
using System.Collections.Generic;
using System.Linq;

namespace Bidirezionale.NodePropagation.SickPropagation
{
    public class BiNodePropagationSickPropagation
    {
        public static bool RepairNode(Node node, bool onlySinkExploration)
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
                    if (node == next && e.Capacity > 0 && previous.Label == (node.Label - 1) && previous.SourceValid && previous.Visited && previous.PreviousNode != node)
                    {
                        //grafo.ChangeLabel(node, true, node.Label);
                        node.SetPreviousNode(previous);
                        node.SetPreviousEdge(e);
                        node.SetVisited(true);
                        e.SetReversed(false);
                        node.SetSourceValid(true);
                        return true;
                    }
                    if (node == previous && e.Flow > 0 && next.Label == (node.Label - 1) && next.SourceValid && next.Visited && next.PreviousNode != node)
                    {
                        //grafo.ChangeLabel(node, true, node.Label);
                        node.SetPreviousNode(next);
                        node.SetPreviousEdge(e);
                        node.SetVisited(true);
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
                    if (previous.SourceSide != next.SourceSide && !onlySinkExploration)
                    {
                        //TODO da capire se ca bene che sia contenuto in lastNodesSourceSide o se devo considerarare altro
                        //TODO fare debugging per essere sicuro di non aver invertito next e previous
                        if (next == node && e.Capacity > 0 && previous.SourceValid && previous.SourceSide && previous.Visited)
                        {
                            node.SetPreviousEdge(e);
                            node.SetPreviousNode(previous);
                            node.SetVisited(true);
                            e.SetReversed(false);
                            node.SetSourceValid(true);
                            return true;
                        }
                        if (previous == node && e.Flow > 0 && next.SourceValid && next.SourceSide && next.Visited)
                        {
                            node.SetPreviousEdge(e);
                            node.SetPreviousNode(next);
                            node.SetVisited(true);
                            e.SetReversed(true);
                            node.SetSourceValid(true);
                            return true;
                        }

                    }
                    else if (onlySinkExploration && next.SourceSide == previous.SourceSide)
                    {
                        if (node == previous && e.Capacity > 0 && next.SinkValid && node.Label == (next.Label + 1) && next.Visited && next.NextNode != node)
                        {
                            node.SetNextEdge(e);
                            node.SetNextNode(next);
                            node.SetVisited(true);
                            e.SetReversed(false);
                            node.SetSinkValid(true);
                            return true;
                        }
                        if (node == next && e.Flow > 0 && previous.SinkValid && node.Label == (previous.Label + 1) && previous.Visited && previous.NextNode != node)
                        {
                            node.SetNextEdge(e);
                            node.SetNextNode(previous);
                            node.SetVisited(true);
                            e.SetReversed(true);
                            node.SetSinkValid(true);
                            return true;
                        }
                    }

                }
            }
            if (node.SourceSide || !onlySinkExploration)
                node.SetSourceValid(false);
            else
                node.SetSinkValid(false);
            return false;
        }
        public static bool Reachable(Node target, Node n)
        {
            while (n.Label >= target.Label || (target.SourceSide && !n.SourceSide))
            {
                if (n is null)
                    break;
                if (target == n)
                    return true;
                if (target.SourceSide && n.SourceValid && n.Visited)
                    n = n.PreviousNode;
                else if (!target.SourceSide && n.SinkValid && n.Visited)
                    n = n.NextNode;
                else
                    break;
            }
            return false;
        }
        public static Node DoBfs(Graph graph, Stack<Node> noCapsSource, Stack<Node> noCapsSink)
        {
            bool needsink = false;
            Queue<Node> codaSource = new();
            Queue<Node> codaSink = new();
            Queue<Node> malati = new();
            Node noCapSource = null;
            Node noCapSink = null;
            bool sourceRepaired = noCapsSource.Count == 0;
            bool sinkRepaired = noCapsSink.Count == 0;
            if (noCapsSource.Count > 0)
            {
                bool repaired = true;
                Node momNoCap = null;
                while (noCapsSource.Count > 0)
                {
                    noCapSource = noCapsSource.Pop();
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
                        if (Reachable(graph.Source, n) && Reachable(graph.Sink, n))
                            return n;
                //parte di sickPropagation
                Node malato = null;
                while (malati.Count > 0)
                    malato ??= SourceSickPropagation(graph, malati.Dequeue(), codaSource);
                if (malato != null && sinkRepaired)//&& (malato.NextEdge.Reversed ? malato.NextEdge.Flow : malato.NextEdge.Capacity) > 0 && malato.NextNode.Visited)                
                    return malato;
                if (sinkRepaired)
                    foreach (var n in graph.LastNodesSinkSide.Where(x => (x.SourceValid && x.NextEdge.Reversed ? x.NextEdge.Flow : x.NextEdge.Capacity) > 0 && x.NextNode.Visited && x.SinkValid && x.NextNode.SinkValid))
                        if (Reachable(graph.Source, n))
                            return n;
                sourceRepaired = repaired;
                // fine parte di sickpropagation
                if (!repaired && codaSource.Count == 0)
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
                Node p = null;
                Node momNoCap = null;
                while (noCapsSink.Count > 0)
                {
                    noCapSink = noCapsSink.Pop();
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
                    foreach (var n in graph.LastNodesSinkSide.Where(x => x.SinkValid && x.SourceValid && x.PreviousNode != null && x.PreviousNode.Visited))
                    {
                        if (Reachable(graph.Sink, n))
                            return n;
                    }
                }
                Node malato = null;
                while (malati.Count > 0)
                    malato ??= SickPropagationSink(malati.Dequeue(), codaSink);
                if (malato != null)
                    return malato;
                foreach (var n in graph.LastNodesSinkSide.Where(x => x.SinkValid && (x.PreviousEdge.Reversed ? x.PreviousEdge.Flow : x.PreviousEdge.Capacity) > 0 && x.PreviousNode.Visited && x.SourceValid))
                {
                    if (Reachable(graph.Sink, n))
                        return n;
                }
                sinkRepaired = repaired;
                if (!repaired && codaSink.Count == 0)
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
            do
            {
                if (needsink)
                {
                    graph.ResetSinkSide(0);
                    codaSink.Enqueue(graph.Sink);
                    needsink = false;
                    sinkRepaired = false;
                }
                // ricerca normale del flusso
                while (codaSink.Count > 0 || codaSource.Count > 0)
                {
                    if (codaSource.Count > 0)//&& !sourceRepaired)
                    {
                        var element = codaSource.Dequeue();
                        if (!element.SourceSide || !element.SourceValid || !element.Visited)
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
                                if (n.SourceSide && (!n.Visited || !n.SourceValid))
                                {
                                    n.SetVisited(true);
                                    graph.ChangeLabel(n, true, p.Label + 1);
                                    n.SetPreviousNode(p);
                                    n.SetPreviousEdge(e);
                                    e.SetReversed(false);
                                    n.SetSourceValid(true);
                                    codaSource.Enqueue(n);
                                }
                                else if (n.Visited && !n.SourceSide && n.SinkValid)
                                {
                                    n.SetSourceValid(true);
                                    n.SetPreviousNode(element);
                                    n.SetPreviousEdge(e);
                                    //graph.AddLast(p);
                                    graph.AddLast(n);
                                    e.SetReversed(false);
                                    //n.SetInFlow(f);
                                    return n;
                                }
                                else if (!n.SourceSide && (!n.Visited || !n.SinkValid) && codaSink.Count == 0 && sinkRepaired)
                                {
                                    needsink = true;
                                }
                            }
                            else if (element == n && e.Flow > 0)
                            {
                                if (p.SourceSide && (!p.Visited || !p.SourceValid))
                                {
                                    p.SetVisited(true);
                                    graph.ChangeLabel(p, true, n.Label + 1);
                                    p.SetPreviousEdge(e);
                                    p.SetPreviousNode(n);
                                    e.SetReversed(true);
                                    p.SetSourceValid(true);
                                    codaSource.Enqueue(p);
                                }
                                else if (p.Visited && !p.SourceSide && p.SinkValid)
                                {
                                    p.SetSourceValid(true);
                                    p.SetPreviousNode(n);
                                    p.SetPreviousEdge(e);
                                    graph.AddLast(p);
                                    //graph.AddLast(n);
                                    e.SetReversed(true);
                                    //p.SetInFlow(f);
                                    return p;
                                }
                                else if ((!p.Visited || !p.SinkValid) && !p.SourceSide && codaSink.Count == 0 && sinkRepaired)
                                {
                                    needsink = true;
                                }
                            }
                        }
                    }
                    if (codaSink.Count > 0)//&& !sinkRepaired)
                    {
                        var element = codaSink.Dequeue();
                        if (element.SourceSide || !element.SinkValid || !element.Visited)
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
                                if (p.Visited && p.SinkValid)
                                {
                                    if (!p.SourceSide || !p.SourceValid)
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
                                        n.SetSourceValid(true);
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
                                p.SetSinkValid(true);
                                codaSink.Enqueue(p);
                            }
                            else if (element == p && e.Flow > 0)
                            {
                                if (n.Visited && n.SinkValid)
                                    if (!n.SourceSide || !p.SourceValid)
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
                                        p.SetSourceValid(true);
                                        //n.SetInFlow(f);
                                        return p;
                                    }
                                //n.SetSourceSide(false);
                                n.SetVisited(true);
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
            } while (needsink);

            return null;
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
                    else if (m.PreviousEdge != null && (m.PreviousEdge.Reversed ? m.PreviousEdge.Flow : m.PreviousEdge.Capacity) > 0 && m.PreviousNode.Visited && m.PreviousNode.SourceValid)
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
                    else if (m.NextEdge != null && (m.NextEdge.Reversed ? m.NextEdge.Flow : m.NextEdge.Capacity) > 0 && m.NextNode.Visited && m.SinkValid)
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
                var n = DoBfs(graph, vuotiSource, vuotiSink);
                if (n is null)
                    break;
                int f = GetFlow(n, s, t);
                if (f == 0)
                    break;
                vuotiSource.Clear();
                vuotiSink.Clear();
                Node momsource = n;
                Node momsink = n;
                while (momsource != s)
                {
                    if (momsource.PreviousEdge.AddFlow(f)) // cap = 0
                    {
                        vuotiSource.Push(momsource);
                        momsource.SetSourceValid(false);
                    }
                    momsource = momsource.PreviousNode;

                }
                while (momsink != t)
                {
                    if (momsink.NextEdge.AddFlow(f))
                    {
                        vuotiSink.Push(momsink);
                        momsink.SetSinkValid(false);
                    }
                    momsink = momsink.NextNode;
                }
                fMax += f;

            }
            return fMax;
        }

        private static int GetFlow(Node n, Node s, Node t)
        {
            int source = int.MaxValue, sink = int.MaxValue;
            Node toSource = n;
            while (toSource != s)
            {
                source = Math.Min(source, toSource.PreviousEdge.Reversed ? toSource.PreviousEdge.Flow : toSource.PreviousEdge.Capacity);
                toSource = toSource.PreviousNode;
            }
            while (n != t)
            {
                sink = Math.Min(sink, n.NextEdge.Reversed ? n.NextEdge.Flow : n.NextEdge.Capacity);
                n = n.NextNode;
            }
            return Math.Min(source, sink);
        }
    }
}
