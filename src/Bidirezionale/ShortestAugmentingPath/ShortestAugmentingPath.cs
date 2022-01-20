using System;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace Bidirezionale.ShortestAugmentingPath
{
    public class ShortestAugmentingPath
    {
        //TODO valutare se serve e come implementare funzione reset (o rimozione durante i nodi )

        public static int BfsFromSource(Node s)
        {
            Queue<Node> coda = new();
            coda.Enqueue(s);
            int f = int.MaxValue;
            while (coda.Count > 0)
            {
                Node element = coda.Dequeue();
                foreach (var edge in element.Edges)
                {
                    Node n = edge.NextNode;
                    if (n.PreviousEdge == null && edge.Capacity > 0)
                    {
                        n.SetPrevious(edge);
                        f = Math.Min(edge.Capacity, f);
                    }
                    if (n.SourceDistance == -1)
                    {
                        n.SetSourceDistance(element.SourceDistance + 1);
                        coda.Enqueue(n);
                    }
                    //if (n.PreviousEdge == null && (element.SourceDistance + 1) == n.SourceDistance && edge.Capacity > 0)
                    //n.SetNext(edge);
                }
            }
            return f;
        }
        private static int BfsFromSink(Node t)
        {
            Queue<Node> coda = new();
            coda.Enqueue(t);
            int f = int.MaxValue;
            while (coda.Count > 0)
            {
                Node element = coda.Dequeue();
                foreach (var edge in element.Edges)
                {
                    Node p = edge.PreviousNode;
                    if (p.NextEdge == null && edge.Capacity > 0 && (edge.NextNode == t || edge.NextNode.NextEdge != null))
                    {
                        p.SetNext(edge);
                        f = Math.Min(f, edge.Capacity);
                    }
                    if (p.SinkDistance == -1)
                    {
                        p.SetSinkDistance(element.SinkDistance + 1);
                        coda.Enqueue(p);
                    }
                    //if (p.NextEdge == null && (element.SinkDistance + 1) == p.SinkDistance && edge.Capacity > 0)
                    //p.SetNext(edge);
                }
            }
            return f;
        }
        public static void SendFlow(Node reached, int flow)
        {
            Node m;
            if (reached is SourceNode)
            {
                while (reached is not SinkNode)
                {
                    m = reached;
                    reached.NextEdge.AddFlow(flow);
                    reached = reached.NextNode;
                    m.Reset();
                }
            }
            else if (reached is SinkNode)
            {
                while (reached is not SourceNode)
                {
                    m = reached;
                    reached.PreviousEdge.AddFlow(flow);
                    reached = reached.PreviousNode;
                    m.Reset();
                }
            }
            else
            {
                Node mom = reached.NextNode;
                while (reached is not SourceNode)
                {
                    m = reached;
                    reached.AddFlow(flow);
                    reached = reached.PreviousNode;
                    m.Reset();
                }
                while (mom is not SinkNode)
                {
                    m = mom;
                    mom.AddFlow(flow);
                    mom = mom.NextNode;
                    m.Reset();
                }
            }
        }

        public static int FlowFordFulkerson(Graph graph)
        {
            Node s = graph.Source;
            Node t = graph.Sink;
            Node startSource = s;
            Node startSink = t;
            int fMax = BfsFromSource(s);
            SendFlow(t, fMax);
            int f = BfsFromSink(t);
            SendFlow(s, f);
            foreach (var n in graph.Nodes)
                n.Reset();
            fMax += f;
            int fso = int.MaxValue, fsi = int.MaxValue;
            while (f != 0 && s.SinkDistance < graph.Nodes.Count && t.SourceDistance < graph.Nodes.Count)
            {
                (fso, fsi, startSource, startSink) = Dfs(graph, startSource, startSink, fso, fsi);

                if (startSink == startSource && startSink != null)
                {
                    f = Math.Min(fso, fsi);
                    SendFlow(startSink, f);
                    fso = int.MaxValue;
                    fsi = int.MaxValue;
                    startSink = t;
                    startSource = s;
                }
                else if (startSink is SourceNode)
                {
                    f = fsi;
                    SendFlow(startSink, f);
                    fsi = int.MaxValue;
                    startSink = t;
                }
                else if (startSource is SinkNode)
                {
                    f = fso;
                    SendFlow(startSource, f);
                    fso = int.MaxValue;
                    startSource = s;
                }
                else
                    break;
                fMax += f;
            }
            return fMax;
        }


        private static (int, int, Node, Node) Dfs(Graph graph, Node startSource, Node startSink, int sourceflow, int sinkflow)
        {
            if (startSink == startSource)
                return (sourceflow, sinkflow, startSource, startSink);
            if (startSink.SourceDistance < graph.Nodes.Count && startSource.SinkDistance < graph.Nodes.Count)
            {
                foreach (var e in startSource.Edges)
                {
                    Node n = e.NextNode;
                    Node p = e.PreviousNode;
                    if (startSource == p && n.SinkDistance == (p.SinkDistance - 1) && e.Capacity > 0)
                    {
                        sourceflow = Math.Min(sourceflow, e.Capacity);
                        //e.SetReversed(false);
                        n.SetPrevious(e);
                        if (n is SinkNode || n.NextEdge != null)
                            return (sourceflow, sinkflow, n, n);
                        return SinkDfs(graph, n, startSink, sourceflow, sinkflow);
                    }
                    /*                     else if (startSource == n && p.SinkDistance == (n.SinkDistance - 1) && e.Flow > 0)
                                        {
                                            sourceflow = Math.Min(sourceflow, e.Flow);
                                            e.SetReversed(true);
                                            p.SetPrevious(e);
                                            if (p is SinkNode || p.NextEdge != null)
                                                return (sourceflow, sinkflow, p, p);
                                            return SinkDfs(graph, p, startSink, sourceflow, sinkflow);
                                        }
                     */                    //TODO da capire come si deve retreat
                }
                int min = int.MaxValue - 1;
                foreach (var e in startSource.Edges)
                {
                    //TODO controllare
                    if (e.PreviousNode == startSource && e.Capacity > 0)
                        min = Math.Min(min, e.NextNode.SinkDistance);
                    /*                     else
                                            min = Math.Min(min, e.NextNode.SinkDistance);
                     */
                }
                startSource.SetSinkDistance(min + 1);
                Node mom;
                if (startSource is SourceNode)
                    mom = startSource;
                else
                    mom = startSource.PreviousNode;
                startSource.Reset();
                return Dfs(graph, mom, startSink, sourceflow, sinkflow);
            }
            return (0, 0, null, null);
        }
        private static (int, int, Node, Node) SinkDfs(Graph graph, Node startSource, Node startSink, int sourceflow, int sinkflow)
        {
            if (startSink == startSource)
                return (sourceflow, sinkflow, startSource, startSink);
            if (startSink.SourceDistance < graph.Nodes.Count && startSource.SinkDistance < graph.Nodes.Count)
            {
                foreach (var e in startSink.Edges)
                {
                    Node n = e.NextNode;
                    Node p = e.PreviousNode;
                    //TODO controllare che sia corretto
                    if (startSink == n && p.SourceDistance == (n.SourceDistance - 1) && e.Capacity > 0)
                    {
                        sinkflow = Math.Min(sinkflow, e.Capacity);
                        //e.SetReversed(false);
                        p.SetNext(e);
                        if (p is SourceNode || p.PreviousEdge != null)
                            return (sourceflow, sinkflow, p, p);
                        return Dfs(graph, startSource, p, sourceflow, sinkflow);
                    }
                    /*                     else if (startSink == p && n.SourceDistance == (p.SourceDistance - 1) && e.Flow > 0)
                                        {
                                            sinkflow = Math.Min(sinkflow, e.Flow);
                                            e.SetReversed(true);
                                            n.SetNext(e);
                                            if (p is SourceNode || n.PreviousEdge != null)
                                                return (sourceflow, sinkflow, n, n);
                                        }
                     */
                }
                //retreat
                int min = int.MaxValue - 1;
                foreach (var e in startSink.Edges)
                {
                    if (startSink == e.NextNode && e.Capacity > 0)
                        min = Math.Min(min, e.PreviousNode.SourceDistance);
                    /*                     if (startSink == e.NextNode && e.Capacity > 0)
                                            min = Math.Min(min, e.PreviousNode.SourceDistance);
                     */
                }
                startSink.SetSourceDistance(min + 1);
                Node mom;
                if (startSink is SinkNode)
                    mom = startSink;
                else
                    mom = startSink.NextNode;
                startSink.Reset();
                return SinkDfs(graph, startSource, mom, sourceflow, sinkflow);
            }
            return (0, 0, null, null);
        }
    }
}