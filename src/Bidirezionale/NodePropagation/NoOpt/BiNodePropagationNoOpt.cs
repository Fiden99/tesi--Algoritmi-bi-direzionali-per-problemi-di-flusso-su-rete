using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bidirezionale.NodePropagation.NoOpt
{
    public class BiNodePropagationNoOpt
    {

        private static (int, Node) DoBfs(Graph graph, bool sourceSide, bool sinkSide)
        {
            var codaSource = new Queue<Node>();
            var codaSink = new Queue<Node>();
            if (sourceSide && sinkSide)
            {
                graph.Reset();
                codaSource.Enqueue(graph.Source);
                codaSink.Enqueue(graph.Sink);
            }
            else if (sourceSide)
            {
                graph.ResetSourceSide();
                codaSource.Enqueue(graph.Source);
            }
            else if (sinkSide)
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
                if (codaSource.Count > 0 && sourceSide)
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
                            if (n.InFlow != 0)
                                if (n.NextNode is null)
                                    continue;
                                else
                                {
                                    int f = Math.Min(n.InFlow, p.InFlow);
                                    f = Math.Min(f, e.Capacity);
                                    if (f == 0)
                                        continue;
                                    n.SetPreviousNode(p);
                                    n.SetPreviousEdge(e);
                                    //TODO capire dove e come aggiornare le label dei nodi trovati da t
                                    graph.SetMeanLabel(n.Label - 1);
                                    e.SetReversed(false);
                                    //n.SetInFlow(f);
                                    return (f, n);
                                }
                            n.SetInFlow(Math.Min(p.InFlow, e.Capacity));
                            n.SetLabel(p.Label + 1);
                            n.SetPreviousNode(p);
                            n.SetPreviousEdge(e);
                            e.SetReversed(false);
                            codaSource.Enqueue(n);
                        }
                        else if (element == n && e.Flow > 0)
                        {
                            if (p.InFlow != 0)
                                if (p.NextNode is null)
                                    continue;
                                else
                                {
                                    int f = Math.Min(p.InFlow, n.InFlow);
                                    f = Math.Min(f, e.Flow);
                                    if (f == 0)
                                        continue;
                                    p.SetPreviousNode(n);
                                    p.SetPreviousEdge(e);
                                    graph.SetMeanLabel(p.Label - 1);
                                    e.SetReversed(true);
                                    //p.SetInFlow(f);
                                    return (f, p);
                                }
                            p.SetInFlow(Math.Min(n.InFlow, e.Flow));
                            p.SetLabel(n.Label + 1);
                            p.SetPreviousEdge(e);
                            p.SetPreviousNode(n);
                            e.SetReversed(true);
                            codaSource.Enqueue(p);
                        }
                    }
                }
                if (codaSink.Count > 0 && sinkSide)
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
                            if (p.InFlow != 0)
                                if (p.PreviousEdge is null)
                                    continue;
                                else
                                {
                                    int f = Math.Min(p.InFlow, n.InFlow);
                                    f = Math.Min(f, e.Capacity);
                                    if (f == 0)
                                        continue;
                                    n.SetPreviousEdge(e);
                                    n.SetPreviousNode(p);
                                    //TODO valutare se inserire come meanLabel n.label+1 oppure p.label
                                    graph.SetMeanLabel(n.Label - 1);
                                    e.SetReversed(false);
                                    //p.SetInFlow(f);
                                    return (f, n);
                                }
                            p.SetInFlow(Math.Min(e.Capacity, n.InFlow));
                            p.SetNextEdge(e);
                            p.SetNextNode(n);
                            p.SetLabel(n.Label - 1);
                            e.SetReversed(false);
                            codaSink.Enqueue(p);
                        }
                        else if (element == p && e.Flow > 0)
                        {
                            if (n.InFlow != 0)
                                if (n.PreviousEdge is null)
                                    continue;
                                else
                                {
                                    int f = Math.Min(p.InFlow, n.InFlow);
                                    f = Math.Min(f, e.Flow);
                                    if (f == 0)
                                        continue;
                                    p.SetPreviousEdge(e);
                                    p.SetPreviousNode(n);
                                    graph.SetMeanLabel(p.Label - 1);
                                    e.SetReversed(true);
                                    //n.SetInFlow(f);
                                    return (f, p);
                                }
                            n.SetInFlow(Math.Min(e.Flow, p.InFlow));
                            n.SetNextEdge(e);
                            n.SetNextNode(p);
                            n.SetLabel(p.Label - 1);
                            e.SetReversed(true);
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
            bool vuotoSink = true;
            bool vuotoSource = true;
            int fMax = 0;
            while (true)
            {
                var (f, n) = DoBfs(graph, vuotoSource, vuotoSink);
                if (f == 0)
                    break;
                fMax += f;
                n.SetInFlow(n.InFlow + f);
                vuotoSink = false;
                vuotoSource = false;
                Node mom = n;
                while (n != s)
                {
                    if (n.PreviousEdge.AddFlow(f))
                        vuotoSource = true;
                    n.SetInFlow(n.InFlow - f);
                    n = n.PreviousNode;
                }
                while (mom != t)
                {
                    if (mom.NextEdge.AddFlow(f))
                        vuotoSink = true;
                    mom.SetInFlow(mom.InFlow - f);
                    mom = mom.NextNode;
                }
            }
            return fMax;
        }
    }
}