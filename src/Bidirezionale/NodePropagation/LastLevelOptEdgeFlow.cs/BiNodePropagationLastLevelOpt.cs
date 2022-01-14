using System;
using System.Collections.Generic;

namespace Bidirezionale.NodePropagation.LastLevelOptEdgeFlow
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
                    //TODO controllare se è corretto il calcolo della label
                    if (node == next && e.Capacity > 0 && previous.Label == (node.Label - 1) && previous.Valid)
                    {
                        //grafo.ChangeLabel(node, true, node.Label);
                        node.SetPreviousNode(previous);
                        node.SetPreviousEdge(e);
                        node.SetVisited(true);
                        e.SetReversed(false);
                        return true;
                    }
                    if (node == previous && e.Flow > 0 && next.Label == (node.Label - 1) && next.Valid)
                    {
                        //grafo.ChangeLabel(node, true, node.Label);
                        node.SetPreviousNode(next);
                        node.SetPreviousEdge(e);
                        node.SetVisited(true);
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
            if (target.SourceSide)
            {
                while (target.Label < n.Label || !n.SourceSide)
                {
                    n = n.PreviousNode;
                    if (n == target)
                        return true;
                }
            }
            else
            {
                while (target.Label < n.Label)
                {
                    n = n.NextNode;
                    if (n == target)
                        return true;
                }
            }
            return false;
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
                            if (!n.Valid)
                                continue;
                            if (Reached(noCapSource, n))
                                return n;
                        }
                    else
                        sourceRepaired = true;
                    //TODO è possibile che nel percorso ci sia un nuovo nodo invalido
                    //da capire se devo fare un GetFlow qui, ma da quale nodo devo partire?

                }
                if (noCapSource is SourceNode)
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
                if (RepairNode(graph, noCapSink, true))
                {
                    sinkRepaired = true;
                    if (noCapSource == null || sourceRepaired)
                    {
                        foreach (var n in graph.LastNodesSinkSide)
                        {
                            if (!n.Valid)
                                continue;
                            bool sourceReached = false;
                            //TODO errore molto probabilmente qui, 
                            if (sourceRepaired && Reached(noCapSource, n))
                                sourceReached = true;
                            if (sourceReached && Reached(noCapSink, n))
                                return n;
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
                if (codaSource.Count > 0 && (noCapSource != null || !sourceRepaired))
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
                                    //TODO capire cosa devo fare nel caso SourceSide sia falso, ma inflow = 0 (cioè non valido)
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
                    if (element.SourceSide || !element.Visited)
                        continue;//TODO valutare se deve essere un continue o un break
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
                                        if (sourceRepaired && Reached(noCapSource, n))
                                        {
                                            return n;
                                        }
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
                                        if (sourceRepaired && Reached(noCapSource, p))
                                            return p;
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
                                if (n.SourceSide && n is not SinkNode)
                                    continue;
                                n.SetVisited(true);
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
            Node vuotoSource = s;
            Node vuotoSink = t;
            int fMax = 0;
            while (true)
            {

                var n = DoBfs(graph, vuotoSource, vuotoSink);
                int f = GetFlow(n);
                if (f == 0)
                    break;
                fMax += f;
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
                    n = n.PreviousNode;

                }
                while (mom != t)
                {
                    if (mom.NextEdge.AddFlow(f))
                    {
                        vuotoSink = mom;
                        mom.SetValid(false);
                    }
                    mom = mom.NextNode;

                }
            }
            return fMax;

        }
    }
}