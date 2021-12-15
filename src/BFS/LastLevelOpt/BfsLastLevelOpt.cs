using System;
using System.Collections.Generic;
using System.Linq;
namespace BFS.LastLevelOpt
//TODO valutare se conviene comunque tenere il set InvalidNodes
{
    public class BfsLastLevelOpt
    {
        private static bool Repair(Graph grafo, Node node)
        {
            foreach (var e in node.Edges.Where(x => x.NextNode == node))
            {
                Node previous = e.PreviousNode;
                if (e.Capacity > 0 && previous.Label == (node.Label - 1) && previous.Valid == true)
                {
                    grafo.RepairNode(node, node.Label);
                    node.SetPreviousNode(previous);
                    node.SetInFlow(Math.Min(e.Capacity, previous.InFlow));
                    return true;
                }
            }
            return false;
        }

        public static int CorrectFlow(Node node)
        {
            if (node.PreviousNode != null && node.InFlow > node.PreviousNode.InFlow)
                node.SetInFlow(CorrectFlow(node.PreviousNode));
            return node.InFlow;

        }
        public static int DoBfs(Graph grafo, Node node)
        {

            Queue<Node> coda;
            if (node == null)
            {

                grafo.ResetLabel(0);
                coda = new Queue<Node>();
                coda.Enqueue(grafo.Source);

                /*                 if (grafo.InvalidNodes.Count == 0)
                                {
                                    grafo.ResetLabel(0);
                                    coda = new Queue<Node>();
                                    coda.Enqueue(grafo.Source);
                                }
                                else
                                {

                                    int x = grafo.InvalidNodes.Min(x => x.Label);
                                    coda = new Queue<Node>(grafo.LabeledNode[x - 1]);
                                    //grafo.ResetLabel(x);
                                    grafo.ResetLabel(x);

                                    //coda = new Queue<Node>(grafo.LabeledNode[startLabel - 1]);
                                    //grafo.ResetLabel(startLabel);
                                }
                 */
            }
            else
            {
                coda = new Queue<Node>(grafo.LabeledNode[node.Label - 1]);
                grafo.ResetLabel(node.Label);
                foreach (Node n in coda)
                    n.SetInFlow(CorrectFlow(n));

            }
            while (coda.Count > 0)
            {
                var element = coda.Dequeue();
                foreach (BiEdge edge in element.Edges.Where(x => x.PreviousNode == element))
                {
                    Node n = edge.NextNode;
                    if (edge.Capacity < 0)
                        throw new InvalidOperationException();
                    if (edge.Capacity == 0 && n.Valid == true)
                    {
                        grafo.InvalidNode(n);
                        if (Repair(grafo, n))
                        {
                            if (n is SinkNode)
                                return n.InFlow;
                            else
                                coda.Enqueue(n);
                        }
                        else
                            return DoBfs(grafo, n);
                    }
                    if (edge.Capacity > 0 && n.InFlow == 0 && element.Valid == true)
                    {
                        if (n.Valid == true)
                            grafo.ChangeLabel(n, element.Label + 1);
                        else
                            grafo.RepairNode(n, element.Label + 1);
                        n.SetInFlow(Math.Min(element.InFlow, edge.Capacity));
                        n.SetPreviousNode(element);
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
            foreach (var set in grafo.LabeledNode)
            {
                foreach (var node in set)
                {
                    Console.Write("node " + node.Name + " label = " + node.Label);
                    foreach (var x in node.Edges.Where(x => x.PreviousNode == node))
                        Console.Write(" to " + x.NextNode.Name + ", f = " + x.Flow + ", c  = " + x.Capacity + ";");
                    Console.WriteLine();
                }
            }
            foreach (var node in grafo.InvalidNodes)
            {
                Console.Write("node " + node.Name);
                foreach (var x in node.Edges.Where(x => x.PreviousNode == node))
                    Console.Write(" to " + x.NextNode.Name + ", f = " + x.Flow + ", c  = " + x.Capacity + ";");
                Console.WriteLine();

            }
        }
        public static int FlowFordFulkerson(Graph grafo)
        {
            Node vuoto = null;
            int fMax = 0;
            var s = grafo.Source;
            var t = grafo.Sink;
            while (true)
            {
                int f;
                //try
                //{
                f = DoBfs(grafo, vuoto);
                if (f == 0)
                    break;
                Node mom = t;
                while (mom != s)
                {

                    if (mom.PreviousNode.AddFlow(f, mom))
                        vuoto = mom;
                    mom = mom.PreviousNode;

                }
                //}
                /*                 catch (ArgumentException)
                                {
                                    PrintGraph(grafo);
                                    Console.WriteLine("flusso inviato = " + fMax);
                                    return fMax;

                                } */
                fMax += f;
            }
            PrintGraph(grafo);
            Console.WriteLine("flusso inviato = " + fMax);
            return fMax;
        }
    }
}