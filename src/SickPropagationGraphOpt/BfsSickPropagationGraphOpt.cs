using System;
using System.Collections.Generic;
using System.Linq;
using BFS.Abstractions;

namespace BFS.SickPropagationGraphOpt
{
    public class BfsSickPropagationGraphOpt : IBFS
    {
        public static bool Repair(Graph grafo, Node node, BiEdge edge)
        {
            node.RemovePreviousLabelNode(edge.previousNode);
            if (node.previousLabelNodes.Count != 0)
            {
                Node previous = node.previousLabelNodes.First();
                node.SetInFlow(Math.Min(previous.edges.Single(x => x.nextNode == node).capacity, previous.inFlow));
                node.SetPreviousNode(previous);
                node.SetValid(true);
                //grafo.ChangeLabel(e.previousNode, node.label - 1);
                return true;
                /* TODO da capire se tenere questa condizione o meno
                    if(n.edges.Single(x => x.nextNode == node).capacity <=0)
                        throw new InvalidOperationException();
                */

            }
            return false;
        }
        public static int doBfs(Graph grafo)
        {
            Queue<Node> coda;
            if (grafo.invalidNodes.Count == 0)
            {
                grafo.Reset(0);
                coda = new Queue<Node>();
                coda.Enqueue(grafo.Source);
            }
            else
            {
                var min = grafo.invalidNodes.Min(x => x.label);
                coda = new Queue<Node>(grafo.labeledNodes[min]);
                grafo.Reset(min + 1);
            }
            while (coda.Count > 0)
            {
                var element = coda.Dequeue();
                foreach (var edge in element.edges.Where(x => x.previousNode == element))
                {
                    var n = edge.nextNode;
                    if (edge.capacity < 0)
                        throw new InvalidOperationException();
                    if (edge.capacity == 0 && !grafo.invalidNodes.Contains(n))
                    {
                        if (!Repair(grafo, n, edge))
                        {
                            Queue<Node> malati = new Queue<Node>();
                            malati.Enqueue(n);
                            grafo.InvalidNode(n);
                            grafo.Reset(n);
                            while (malati.Count > 0)
                            {
                                Node z = malati.Dequeue();
                                foreach (var x in z.edges.Where(x => x.nextNode != z))
                                {
                                    var y = x.nextNode;
                                    if (!Repair(grafo, y, x))
                                    {
                                        grafo.Reset(y);
                                        grafo.InvalidNode(y);
                                        malati.Enqueue(y);
                                    }
                                    else if (y is SinkNode)
                                        break;
                                }
                            }
                        }
                    }
                    if (n.valid == true && edge.capacity > 0 && (n.label == 0 || n.label > element.label))
                    {
                        n.SetPreviousNode(element);
                        grafo.ChangeLabel(n, element.label + 1);
                        n.SetInFlow(Math.Min(element.inFlow, edge.capacity));
                        //TODO da valutare se grafo.labeledNodes.Last().Contains(n) sia più efficiente inserirlo nell'if precedente o meno
                        if (n is SinkNode && grafo.labeledNodes.Last().Contains(n))
                            return n.inFlow;
                        else
                            coda.Enqueue(n);
                    }
                }
            }
            return 0;
        }
        public static void PrintGraph(Graph grafo)
        {
            foreach (var set in grafo.labeledNodes)
            {
                foreach (var node in set)
                {
                    Console.Write("node " + node.name + " label = " + node.label);
                    foreach (var x in node.edges.Where(x => x.previousNode == node))
                        Console.Write(" to " + x.nextNode.name + ", f = " + x.flow + ", c  = " + x.capacity + ";");
                    Console.WriteLine();
                }
            }
            if (grafo.invalidNodes.Count > 0)
                Console.WriteLine("nodi malati : ");
            foreach (var node in grafo.invalidNodes)
            {
                Console.Write("node " + node.name);
                foreach (var x in node.edges.Where(x => x.previousNode == node))
                    Console.Write(" to " + x.nextNode.name + ", f = " + x.flow + ", c  = " + x.capacity + ";");
                Console.WriteLine();
            }
        }
        public void Execute()
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
                int f = BfsSickPropagationGraphOpt.doBfs(grafo);
                if (f == 0)
                    break;
                fMax += f;
                Node mom = t;
                while (mom != s)
                {
                    mom.previousNode.AddFlow(f, mom);
                    mom = mom.previousNode;
                }
            }
            PrintGraph(grafo);
            Console.WriteLine("flusso inviato = " + fMax);
        }
    }
}