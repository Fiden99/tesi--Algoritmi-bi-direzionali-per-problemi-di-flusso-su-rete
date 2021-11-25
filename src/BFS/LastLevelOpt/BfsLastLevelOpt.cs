using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BFS.Abstractions;
namespace BFS.LastLevelOpt
{
    public class BfsLastLevelOpt
    {
        private static bool Repair(Graph grafo, Node node)
        {
            bool changed = false;
            foreach (var e in node.Edges.Where(x => x.NextNode == node))
            {
                if (e.Capacity > 0)
                {
                    if (!changed)
                    {
                        grafo.RepairNode(node, e.PreviousNode.Label + 1);
                        changed = true;
                    }
                    else if (changed && e.PreviousNode.Label < node.Label)
                        grafo.ChangeLabel(node, e.PreviousNode.Label + 1);
                }
            }
            return changed;

        }
        public static int DoBfs(Graph grafo)
        {
            Queue<Node> coda;
            if (grafo.InvalidNodes.Count == 0)
            {
                grafo.ResetLabel(0);
                coda = new Queue<Node>();
                coda.Enqueue(grafo.Source);
            }
            else
            {
                Node x = grafo.InvalidNodes.MinBy(x => x.Label);
                coda = new Queue<Node>(grafo.LabeledNode[x.Label]);
                Graph.ResetLabel(x);
                grafo.ResetLabel(x.Label + 1);

            }
            while (coda.Count > 0)
            {
                var element = coda.Dequeue();
                foreach (BiEdge edge in element.Edges.Where(x => x.PreviousNode == element))
                {
                    Node n = edge.NextNode;
                    if (edge.Capacity < 0)
                        throw new InvalidOperationException();
                    if (edge.Capacity == 0)
                    {
                        if (!grafo.InvalidNodes.Contains(n))
                            grafo.InvalidNode(n);
                        if (!Repair(grafo, n))
                            continue;
                    }
                    if (n.PreviousNode == null && edge.Capacity > 0)
                    {
                        n.SetPreviousNode(element);
                        grafo.ChangeLabel(n, element.Label + 1);
                        n.SetInFlow(Math.Min(element.InFlow, edge.Capacity));
                        if (n is SinkNode)
                            return n.InFlow;
                        else
                            coda.Enqueue(n);
                    }
                }
            }
            return 0;
        }

        public static void PrintGraph(Graph grafo)
        {
            foreach (var set in grafo.LabeledNode.Append(grafo.InvalidNodes))
            {
                foreach (var node in set)
                {
                    Console.Write("node " + node.Name + " label = " + node.Label);
                    foreach (var x in node.Edges.Where(x => x.PreviousNode == node))
                        Console.Write(" to " + x.NextNode.Name + ", f = " + x.Flow + ", c  = " + x.Capacity + ";");
                    Console.WriteLine();
                }
            }
        }
        public static int FlowFordFulkerson()
        {
            int fMax = 0;
            SinkNode t = new SinkNode("t");
            Node n6 = new Node("6");
            n6.AddEdge(t, 10);
            Node n5 = new Node("5");
            n5.AddEdge((t, 35), (n6, 10));
            Node n4 = new Node("4");
            n4.AddEdge(n6, 25);
            Node n3 = new Node("3");
            n3.AddEdge((n4, 15), (n5, 15), (n6, 10));
            Node n2 = new Node("2");
            n2.AddEdge((n5, 35), (n3, 10));
            SourceNode s = new SourceNode("s");
            s.AddEdge((n2, 10), (n3, 30), (n4, 30));
            Graph grafo = new Graph(s, n2, n3, n4, n5, n6, t);
            while (true)
            {
                int f = BfsLastLevelOpt.DoBfs(grafo);
                if (f == 0)
                    break;
                fMax += f;
                Node mom = t;
                while (mom != s)
                {
                    mom.PreviousNode.AddFlow(f, mom);
                    mom = mom.PreviousNode;
                }
            }
            PrintGraph(grafo);
            Console.WriteLine("flusso inviato = " + fMax);
            return fMax;
        }
    }
}