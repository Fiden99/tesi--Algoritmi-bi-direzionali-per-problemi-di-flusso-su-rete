using System;
using System.Collections.Generic;
using System.Linq;

namespace Bidirezionale.NodeCount.NoOpt
{
    public class BiNodeCountNoOpt
    {

        private static Node DoBfs(Graph graph, bool sourceSide, bool sinkSide)
        {
            var codaSource = new Queue<Node>();
            var codaSink = new Queue<Node>();
            var codaEdgeSource = new Queue<BiEdge>();
            var codaEdgeSink = new Queue<BiEdge>();
            Node elementSource = null;
            Node elementSink = null;
            if (sourceSide)
            {
                graph.ResetSourceSide();
                codaSource.Enqueue(graph.Source);
            }
            if (sinkSide)
            {
                graph.ResetSinkSide();
                codaSink.Enqueue(graph.Sink);
            }
#if DEBUG
            else if (!sourceSide && !sinkSide)
                throw new InvalidOperationException();
#endif
            while (codaSink.Count > 0 || codaSource.Count > 0)
            {
                if (codaSource.Count > 0 && (codaEdgeSource.Count == 0 || (codaSink.Count == 0 && codaEdgeSink.Count == 0)))
                {
                    elementSource = codaSource.Dequeue();
                    foreach (var e in elementSource.Edges.Where(x => (x.PreviousNode == elementSource && x.Capacity > 0 && (!x.NextNode.Visited || !x.NextNode.SourceSide)) || (x.NextNode == elementSource && x.Flow > 0 && (!x.PreviousNode.Visited || !x.PreviousNode.SourceSide))))
                        codaEdgeSource.Enqueue(e);
                }
                if (codaSink.Count > 0 && (codaEdgeSink.Count == 0 || (codaSource.Count == 0 && codaEdgeSource.Count == 0)))
                {
                    elementSink = codaSink.Dequeue();
                    foreach (var e in elementSink.Edges.Where(x => (x.NextNode == elementSink && x.Capacity > 0 && (!x.PreviousNode.Visited || x.PreviousNode.SourceSide)) || (x.PreviousNode == elementSink && x.Flow > 0 && (!x.NextNode.Visited || x.NextNode.SourceSide))))
                        codaEdgeSink.Enqueue(e);
                }
                while ((codaEdgeSink.Count > 0 || !sinkSide) && (codaEdgeSource.Count > 0 || !sourceSide))
                {
                    if (sourceSide)
                    {
                        var sourceEdge = codaEdgeSource.Dequeue();
                        Node p = sourceEdge.PreviousNode;
                        Node n = sourceEdge.NextNode;
#if DEBUG
                        if (sourceEdge.Capacity < 0 || sourceEdge.Flow < 0)
                            throw new InvalidOperationException("capacità negativa");
#endif
                        if (elementSource == p && sourceEdge.Capacity > 0)
                        {
                            if (n.Visited)
                            {
                                if (!n.SourceSide)
                                {
                                    n.SetPreviousNode(p);
                                    n.SetPreviousEdge(sourceEdge);
                                    //TODO capire dove e come aggiornare le label dei nodi trovati da t
                                    sourceEdge.SetReversed(false);
                                    //n.SetInFlow(f);
                                    return n;
                                }
                            }
                            else
                            {
                                n.SetVisited(true);
                                n.SetLabel(p.Label + 1);
                                graph.ChangeSide(n, true);
                                n.SetPreviousNode(p);
                                n.SetPreviousEdge(sourceEdge);
                                sourceEdge.SetReversed(false);
                                codaSource.Enqueue(n);
                            }
                        }
                        else if (elementSource == n && sourceEdge.Flow > 0)
                        {
                            if (p.Visited)
                            {
                                if (!p.SourceSide)
                                {
                                    p.SetPreviousNode(n);
                                    p.SetPreviousEdge(sourceEdge);
                                    sourceEdge.SetReversed(true);
                                    //p.SetInFlow(f);
                                    return p;
                                }
                            }
                            else
                            {
                                p.SetVisited(true);
                                p.SetLabel(n.Label + 1);
                                graph.ChangeSide(p, true);
                                p.SetPreviousEdge(sourceEdge);
                                p.SetPreviousNode(n);
                                sourceEdge.SetReversed(true);
                                codaSource.Enqueue(p);
                            }
                        }
                    }
                    if (sinkSide)
                    {
                        var edgeSink = codaEdgeSink.Dequeue();
                        var p = edgeSink.PreviousNode;
                        var n = edgeSink.NextNode;
#if DEBUG
                        if (edgeSink.Capacity < 0 || edgeSink.Flow < 0)
                            throw new InvalidOperationException("capacità negativa");
#endif
                        if (elementSink == n && edgeSink.Capacity > 0)
                        {
                            if (p.Visited)
                                if (!p.SourceSide)
                                    continue;
                                else
                                {
                                    n.SetPreviousEdge(edgeSink);
                                    n.SetPreviousNode(p);
                                    //TODO valutare se inserire come meanLabel n.label+1 oppure p.label
                                    edgeSink.SetReversed(false);
                                    //p.SetInFlow(f);
                                    return n;
                                }
                            p.SetVisited(true);
                            graph.ChangeSide(p, false);
                            p.SetNextEdge(edgeSink);
                            p.SetNextNode(n);
                            p.SetLabel(n.Label - 1);
                            edgeSink.SetReversed(false);
                            codaSink.Enqueue(p);
                        }
                        else if (elementSink == p && edgeSink.Flow > 0)
                        {
                            if (n.Visited)
                                if (!n.SourceSide)
                                    continue;
                                else
                                {
                                    p.SetSourceSide(false);
                                    p.SetPreviousEdge(edgeSink);
                                    p.SetPreviousNode(n);
                                    edgeSink.SetReversed(true);
                                    //n.SetInFlow(f);
                                    return p;
                                }
                            n.SetVisited(true);
                            n.SetNextEdge(edgeSink);
                            graph.ChangeSide(n, false);
                            n.SetNextNode(p);
                            n.SetLabel(p.Label - 1);
                            edgeSink.SetReversed(true);
                            codaSink.Enqueue(n);
                        }
                    }

                }
            }
            return null;
        }
        public static int FlowFordFulkerson(Graph graph)
        {
            Node s = graph.Source;
            Node t = graph.Sink;
            bool vuotoSink = true;
            bool vuotoSource = true;
            int fMax = 0;
            while (true)
            {
                var n = DoBfs(graph, vuotoSource, vuotoSink);
                if (n == null)
                    break;
                var f = GetFlow(n);
                if (f == 0)
                    break;
                fMax += f;
                vuotoSink = false;
                vuotoSource = false;
                Node mom = n;
                while (n != s)
                {
                    if (n.PreviousEdge.AddFlow(f))
                        vuotoSource = true;
                    n = n.PreviousNode;
                }
                while (mom != t)
                {
                    if (mom.NextEdge.AddFlow(f))
                        vuotoSink = true;
                    mom = mom.NextNode;
                }
            }
            return fMax;
        }

        private static int GetFlow(Node n)
        {
            int f = int.MaxValue;
            var mom = n;
            while (mom is not SourceNode)
            {
                f = Math.Min(f, mom.PreviousEdge.Reversed ? mom.PreviousEdge.Flow : mom.PreviousEdge.Capacity);
                mom = mom.PreviousNode;
            }
            while (n is not SinkNode)
            {
                f = Math.Min(f, n.NextEdge.Reversed ? n.NextEdge.Flow : n.NextEdge.Capacity);
                n = n.NextNode;
            }
            return f;
        }

    }
}