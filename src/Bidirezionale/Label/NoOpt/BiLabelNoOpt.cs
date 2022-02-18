using System;
using System.Collections.Generic;

namespace Bidirezionale.Label.NoOpt
{
    public class BiLabelNoOpt
    {

        private static Node DoBfs(Graph graph, bool sourceSide, bool sinkSide)
        {
            var codaSource = new Queue<Node>();
            var buffer = new Queue<Node>();
            var codaSink = new Queue<Node>();
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
                while (codaSource.Count > 0)
                {
                    var element = codaSource.Dequeue();
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
                                    n.SetPreviousNode(p);
                                    n.SetPreviousEdge(e);
                                    e.SetReversed(false);
                                    //n.SetInFlow(f);
                                    return n;
                                }
                            n.SetSourceSide(true);
                            n.SetVisited(true);
                            n.SetLabel(p.Label + 1);
                            n.SetPreviousNode(p);
                            n.SetPreviousEdge(e);
                            e.SetReversed(false);
                            buffer.Enqueue(n);
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
                                    e.SetReversed(true);
                                    //p.SetInFlow(f);
                                    return p;
                                }
                            p.SetSourceSide(true);
                            p.SetVisited(true);
                            p.SetLabel(n.Label + 1);
                            p.SetPreviousEdge(e);
                            p.SetPreviousNode(n);
                            e.SetReversed(true);
                            buffer.Enqueue(p);
                        }
                    }
                }
                var mom = codaSource;
                codaSource = buffer;
                buffer = mom;
                while (codaSink.Count > 0)
                {
                    var element = codaSink.Dequeue();
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
                                if (!n.SourceSide)
                                    continue;
                                else
                                {
                                    n.SetPreviousEdge(e);
                                    n.SetPreviousNode(p);
                                    e.SetReversed(false);
                                    //p.SetInFlow(f);
                                    return n;
                                }
                            p.SetSourceSide(false);
                            p.SetVisited(true);
                            p.SetNextEdge(e);
                            p.SetNextNode(n);
                            p.SetLabel(n.Label - 1);
                            e.SetReversed(false);
                            buffer.Enqueue(p);
                        }
                        else if (element == p && e.Flow > 0)
                        {
                            if (n.Visited)
                                if (!n.SourceSide)
                                    continue;
                                else
                                {
                                    p.SetPreviousEdge(e);
                                    p.SetPreviousNode(n);
                                    e.SetReversed(true);
                                    //n.SetInFlow(f);
                                    return p;
                                }
                            n.SetSourceSide(false);
                            n.SetVisited(true);
                            n.SetNextEdge(e);
                            n.SetNextNode(p);
                            n.SetLabel(p.Label - 1);
                            e.SetReversed(true);
                            buffer.Enqueue(n);
                        }
                    }

                }
                mom = codaSink;
                codaSink = buffer;
                buffer = mom;
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
                int f = GetFlow(n);
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