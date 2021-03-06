using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Monodirezionale.MaxFlow.ShortestAugmentingPath
{
    public class ShortestAugmentingPath
    {
        public static void SendFlow(int flow, Node from)
        {
            if (from is SourceNode)
                while (from is not SinkNode)
                {
                    Node.AddFlow(flow, from.PreviousEdge);
                    from = from.PreviousNode;
                }
            else
                while (from is not SourceNode)
                {
                    Node.AddFlow(flow, from.PreviousEdge);
                    from = from.PreviousNode;
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
                    //Node n = e.NextNode;
                    Node p = e.PreviousNode;
                    //n.SetPrevious(e);
                    // da t retrocedo verso s, non cambiando i nodi già cambiati
                    if (p.PreviousEdge == null && e.Capacity > 0)
                    {
                        p.SetPrevious(e);
                        f = Math.Min(f, e.Capacity);
                    }
                    if (p.Distance == -1)
                    {
                        p.SetDistance(element.Distance + 1);
                        coda.Enqueue(p);
                    }
                    //if (n.PreviousEdge == null && n.Distance)
                }
            }
            return f;
        }
        public static int Dfs(Graph graph, Node node, Queue<Node> esplorati)
        {
            Stack<(Node, int)> pila = new();
            pila.Push((node, int.MaxValue));
            while (pila.Count > 0)
            {
                bool advanced = false;
                var x = pila.Pop();
                var start = x.Item1;
                var f = x.Item2;
                if (start.Distance < graph.Nodes.Count)
                {
                    //Advance
                    foreach (var e in start.Edges)
                    {
                        Node n = e.NextNode;
                        Node p = e.PreviousNode;
                        if (start == p && n.Distance == (p.Distance - 1) && e.Capacity > 0)
                        {
                            f = Math.Min(f, e.Capacity);
                            n.SetPrevious(e);
                            esplorati.Enqueue(n);
                            if (n is SinkNode)
                                return f;
                            //return Dfs(graph,n,f,esplorati);
                            pila.Push((n, f));
                            advanced = true;
                            break;
                        }
                    }
                    //retreat
                    if (!advanced)
                    {
                        int min = int.MaxValue - graph.Nodes.Count;
                        foreach (var e in start.Edges)
                        {
                            if (start == e.PreviousNode && e.Capacity > 0)
                                min = Math.Min(min, e.NextNode.Distance);
                        }
                        Node mom;
                        if (start is SourceNode)
                            mom = start;
                        else
                            mom = start.PreviousNode;
                        start.SetDistance(min + 1);
                        pila.Push((mom, f));
                        //return Dfs(graph,mom,f,esplorati);
                    }
                }
                else
                    return 0;
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
            Queue<Node> esplorati = new();
            //primo flusso inviato (già ottenuto grazie a Bfs, servita per avere le distanze)

            SendFlow(fMax, s);

            foreach (var n in graph.Nodes)
                n.Reset();

            while (s.Distance < graph.Nodes.Count)
            {
                int f = Dfs(graph, s, esplorati);
                fMax += f;
                if (f == 0)
                    break;
                //augment
                SendFlow(f, t);
                while (esplorati.Count > 0)
                    esplorati.Dequeue().Reset();
            }
            //PrintGraph(graph);
            return fMax;
        }
    }
}