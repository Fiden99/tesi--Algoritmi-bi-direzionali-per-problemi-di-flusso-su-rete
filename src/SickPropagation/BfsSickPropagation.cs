
using System;
using System.Collections.Generic;
using System.Linq;
using BFS.Abstractions;
using BFS.LastLevelOpt;

namespace BFS.SickPropagation

{
    public class BfsSickPropagation : IBFS
    {
        private static bool Repair(Graph grafo, Node node)
        {
            foreach (var e in node.edges.Where(x => x.nextNode == node && x.capacity > 0))
            {
                if (grafo.labeledNodes[node.label - 1].Contains(e.previousNode))
                {
                    node.setInFlow(Math.Min(e.previousNode.inFlow, e.capacity));
                    node.setPreviousNode(e.previousNode);
                    node.setValid(true);
                    grafo.ChangeLabel(e.previousNode, node.label - 1);
                    return true;
                }
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
                    {//capire se si può fare in maniera molto migliore
                        if (!Repair(grafo, n))
                        {
                            Queue<Node> malati = new Queue<Node>();
                            malati.Enqueue(n);
                            grafo.InvalidNode(n);
                            while (malati.Count > 0)
                            {
                                Node z = malati.Dequeue();
                                foreach (var x in z.edges.Where(x => x.nextNode != z).Select(x => x.nextNode))
                                {
                                    if (!Repair(grafo, x))
                                    {
                                        grafo.Reset(x);
                                        grafo.InvalidNode(x);
                                        malati.Enqueue(x);
                                    }
                                    else if (x is SinkNode)
                                        break;
                                }
                            }
                        }
                    }
                    if (n.valid == true && edge.capacity > 0 && (n.label == 0 || n.label > element.label))
                    {
                        n.setPreviousNode(element);
                        grafo.ChangeLabel(n, element.label + 1);
                        n.setInFlow(Math.Min(element.inFlow, edge.capacity));
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
            n6.addEdge(t, 10);
            Node n5 = new Node("5");
            n5.addEdge((t, 35), (n6, 10));
            Node n4 = new Node("4");
            n4.addEdge(n6, 25);
            Node n3 = new Node("3");
            n3.addEdge((n4, 15), (n5, 15), (n6, 10));
            Node n2 = new Node("2");
            n2.addEdge((n5, 35), (n3, 10));
            SourceNode s = new SourceNode("s");
            s.addEdge((n2, 10), (n3, 30), (n4, 30));
            Graph grafo = new Graph(s, n2, n3, n4, n5, n6, t);
            while (true)
            {
                int f = BfsSickPropagation.doBfs(grafo);
                if (f == 0)
                    break;
                fMax += f;
                Node mom = t;
                while (mom != s)
                {
                    mom.previousNode.addFlow(f, mom);
                    mom = mom.previousNode;
                }
            }
            PrintGraph(grafo);
            Console.WriteLine("flusso inviato = " + fMax);
        }

    }
}