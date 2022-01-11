using System;
using System.Collections.Generic;
using System.Linq;

namespace Bidirezionale.NodePropagation.LastLevelOpt
{
    public class BiNodePropagationLastLevelOpt
    {
        public static bool RepairNode(Graph graph, Node node)
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
                    if (node == next && e.Capacity > 0 && previous.Label == (node.Label - 1))
                    {
                        //grafo.ChangeLabel(node, true, node.Label);
                        node.SetPreviousNode(previous);
                        node.SetPreviousEdge(e);
                        node.SetInFlow(Math.Min(e.Capacity, previous.InFlow));
                        e.SetReversed(false);
                        return true;
                    }
                    if (node == previous && e.Flow > 0 && next.Label == (node.Label - 1))
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
                    if (previous.SourceSide != next.SourceSide)
                    {
                        if (previous == node && e.Flow > 0 && graph.LastNodesSourceSide.Contains(next))
                        {
                            node.SetPreviousEdge(e);
                            node.SetPreviousNode(next);
                            node.SetInFlow(Math.Min(e.Flow, previous.InFlow));
                            e.SetReversed(true);
                            node.SetValid(true);
                            return true;
                        }
                        if (next == previous && e.Capacity > 0 && graph.LastNodesSourceSide.Contains(previous))
                        {
                            node.SetPreviousEdge(e);
                            node.SetPreviousNode(previous);
                            node.SetInFlow(Math.Min(e.Capacity, previous.InFlow));
                            e.SetReversed(false);
                            node.SetValid(true);
                            return true;
                        }
                    }
                    if (node == next && e.Capacity > 0 && node.Label == (next.Label - 1))
                    {
                        node.SetNextEdge(e);
                        node.SetNextNode(next);
                        node.SetInFlow(Math.Min(e.Capacity, next.InFlow));
                        e.SetReversed(false);
                        node.SetValid(true);
                        return true;
                    }
                    if (node == next && e.Flow > 0 && node.Label == (previous.Label - 1))
                    {
                        node.SetNextEdge(e);
                        node.SetNextNode(previous);
                        node.SetInFlow(Math.Min(previous.InFlow, e.Flow));
                        e.SetReversed(true);
                        node.SetValid(true);
                        return true;
                    }
                }
                return false;
            }
        }
        public static Node GetFlow(Node target, Node n)
        {//TODO Da risolvere problema dovuto a n = null
            if (n == null)
                return null;
            if (target.SourceSide)
            {
                if (n.Label < target.Label)
                    return null;
                if (target == n.PreviousNode)
                {
                    int min = Math.Min(n.PreviousEdge.Capacity, target.InFlow);
                    min = Math.Min(min, n.InFlow);
                    target.SetInFlow(min);
                    return target;
                }
                else if (n is not SourceNode && n is not SinkNode)
                {
                    var edge = n.PreviousEdge;
                    if (!edge.Reversed)
                    {
                        var m = GetFlow(target, n.PreviousNode);
                        if (m == null)
                            return null;
                        n.SetInFlow(Math.Min(edge.Capacity, m.InFlow));
                        return n;
                    }
                    else
                    {
                        var m = GetFlow(target, n.PreviousNode);
                        if (m == null)
                            return null;
                        n.SetInFlow(Math.Min(edge.Flow, m.InFlow));
                        return n;
                    }
                }

            }
            else
            {
                if (target == n.NextNode)
                {
                    int min = Math.Min(n.NextEdge.Capacity, target.InFlow);
                    min = Math.Min(min, n.InFlow);
                    target.SetInFlow(min);
                    return target;
                }
                else if (n is not SinkNode && n is not SourceNode)
                {
                    var edge = n.NextEdge;
                    if (!edge.Reversed)
                    {
                        var m = GetFlow(target, n.NextNode);
                        if (m == null)
                            return null;
                        n.SetInFlow(Math.Min(edge.Capacity, m.InFlow));
                        return n;
                    }
                    else
                    {
                        var m = GetFlow(target, n.NextNode);
                        if (m == null)
                            return null;
                        n.SetInFlow(Math.Min(edge.Flow, m.InFlow));
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
            if (noCapSource != null)
            {
                if (RepairNode(graph, noCapSource))
                {
                    if (noCapSink == null)
                        foreach (var n in graph.LastNodesSinkSide)
                        {
                            var node = GetFlow(noCapSource, n);
                            if (node != null && node.InFlow != 0)
                                return (node.InFlow, n);
                        }
                    else
                        sourceRepaired = true;
                    //TODO capire se e come creare un percorso da LastSourceSide/LastSinkSide a Node (non mi interessa inFlow)
                    // possibile idea, aggiungere booleano
                }
                //TODO capire se qui va bene else if o se serve solo if
                else if (noCapSource is SourceNode)
                {
                    codaSource = new();
                    codaSource.Enqueue(graph.Source);
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
                //TODO da testare questa parte 
                if (RepairNode(graph, noCapSink))
                {
                    foreach (var n in graph.LastNodesSinkSide)
                    {
                        var node = GetFlow(noCapSink, n);
                        if (node != null && node.InFlow != 0)
                        {
                            if (node.NextEdge.Reversed)
                                return (Math.Min(Math.Min(node.InFlow, node.NextNode.InFlow), node.NextEdge.Flow), n);
                            else
                                return (Math.Min(Math.Min(node.InFlow, node.NextNode.InFlow), node.NextEdge.Capacity), n);
                        }
                    }
                }

                if (noCapSink is SinkNode)
                {
                    codaSink = new();
                    codaSink.Enqueue(graph.Sink);
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
                if (codaSource.Count > 0 && (noCapSource != null || sourceRepaired))
                {
                    var element = codaSource.Dequeue();
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
                if (codaSink.Count > 0 && noCapSink != null)
                {
                    var element = codaSink.Dequeue();
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
                                if (element.SourceSide)
                                    continue;//TODO valutare se deve essere un continue o un break
                                if (p.InFlow != 0)
                                {
                                    if (!p.SourceSide)
                                        continue;
                                    else
                                    {
                                        if (sourceRepaired)
                                        {
                                            Node m = GetFlow(noCapSource, n);
                                            if (m != null)
                                                if (n.NextEdge.Reversed)
                                                    return (Math.Min(m.InFlow, Math.Min(n.NextEdge.Flow, n.NextNode.InFlow)), n);
                                                else
                                                    return (Math.Min(m.InFlow, Math.Min(n.NextEdge.Capacity, n.NextNode.InFlow)), n);
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
                                        continue;
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
                                        //TODO capire come fare in caso getflow ritorni null                                        
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

                var (f, n) = DoBfs(graph, vuotoSource, vuotoSink);
                if (f == 0)
                    break;
                fMax += f;
                n.SetInFlow(n.InFlow + f);
                vuotoSink = null;
                vuotoSource = null;
                Node mom = n;
                while (n != s)
                {
                    if (n.PreviousEdge.AddFlow(f))
                    {
                        vuotoSource = n;
                        n.SetValid(false);
                    }
                    n.SetInFlow(n.InFlow - f);
                    n = n.PreviousNode;

                }
                while (mom != t)
                {
                    if (mom.NextEdge.AddFlow(f))
                    {
                        vuotoSink = mom;
                        mom.SetValid(false);
                    }
                    mom.SetInFlow(mom.InFlow - f);
                    mom = mom.NextNode;

                }
            }
            return fMax;

        }
    }
}