using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BFS.ShortestAugmentingPath
{
    public class ShortestAugmentingPath
    {
        public static void SendFlow(Graph graph, int flow)
        {
            Node mom = graph.Sink;
            while (mom is not SourceNode)
            {
                mom.PreviousNode.AddFlow(flow, mom.PreviousEdge);
                mom = mom.PreviousNode;
            }
        }
        public static int Bfs(Graph graph)
        {
            int f = int.MaxValue;
            Queue<Node> coda = new Queue<Node>();
            coda.Enqueue(graph.Sink);
            while (coda.Count > 0)
            {
                var element = coda.Dequeue();
                foreach (var e in element.Edges)
                {
                    Node n = e.NextNode;
                    Node p = e.PreviousNode;
                    //n.SetPrevious(e);
                    // da t retrocedo verso s, non cambiando i nodi già cambiati
                    if (p.Distance == -1)
                    {
                        f = Math.Min(f, e.Capacity);
                        if (n.PreviousEdge == null)
                            n.SetPrevious(e);
                        p.SetDistance(n.Distance + 1);
                        coda.Enqueue(p);
                    }
                    if (n.PreviousEdge == null)
                    {
                        f = Math.Min(f, e.Capacity);
                        n.SetPrevious(e);
                    }
                }
            }
            return f;
        }
        public static int Dfs(Graph graph, Node start, int f)
        {
            if (start.Distance < graph.Nodes.Count)
            {
                foreach (var e in start.Edges)
                {
                    Node n = e.NextNode;
                    Node p = e.PreviousNode;
                    if (start == p && n.Distance == (p.Distance - 1) && e.Capacity > 0)
                    {
                        f = Math.Min(f, e.Capacity);
                        e.SetReversed(false);
                        n.SetPrevious(e);
                        if (n is SinkNode)
                            return f;
                        return Dfs(graph, n, f);
                    }
                    else if (start == n && p.Distance == (n.Distance - 1) && e.Flow > 0)
                    {
                        f = Math.Min(f, e.Flow);
                        e.SetReversed(true);
                        p.SetPrevious(e);
                        if (p is SinkNode)
                            return f;
                        return Dfs(graph, p, f);
                    }
                }
                int min = int.MaxValue - 1;
                foreach (var e in start.Edges)
                {
                    if (start == e.NextNode && e.Flow > 0)
                        min = Math.Min(min, e.PreviousNode.Distance);
                    else if (start == e.PreviousNode && e.Capacity > 0)
                        min = Math.Min(min, e.NextNode.Distance);
                }
                start.SetDistance(min + 1);
                if (start is SourceNode)
                    return Dfs(graph, start, f);
                else
                    return Dfs(graph, start.PreviousNode, f);
            }
            return 0;

        }
        public static void PrintGraph(Graph grafo)
        {
            foreach (var node in grafo.Nodes)
            {
                Console.Write("node " + node.Name + " distance = " + node.Distance);
                foreach (var x in node.Edges.Where(x => x.PreviousNode == node))
                    Console.Write(" to " + x.NextNode.Name + ", f = " + x.Flow + ", c  = " + x.Capacity + ";");
                Console.WriteLine();
            }
        }

        public static int FlowFordFulkerson(Graph graph)
        {
            Node s = graph.Source;
            Node t = graph.Sink;
            int fMax = Bfs(graph);
            Node mom = t;
            //primo flusso inviato (già ottenuto grazie a Bfs, servita per avere le distanze)

            SendFlow(graph, fMax);

            while (s.Distance < graph.Nodes.Count)
            {
                int f = Dfs(graph, s, int.MaxValue);
                fMax += f;
                if (f != 0)
                {
                    SendFlow(graph, f);
                }
                else
                    break;
            }
            PrintGraph(graph);
            return fMax;
        }
    }
}