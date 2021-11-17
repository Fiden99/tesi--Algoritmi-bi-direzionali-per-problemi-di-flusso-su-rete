using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BFS.Abstractions;
//TODO capire se nella bfs privata e in quella pubblica è possibile riciclare il codice dentro una funzione 
namespace BFS.LastLevelOpt
{
    public class BfsLastLevelOpt : IBFS
    {
        private static bool Repair(Graph grafo, Node node)
        {
            bool changed = false;
            //TODO capire se lo devi considerare anche come previous node
            foreach (var e in node.edges.Where(x => x.nextNode == node))
            {
                if (e.capacity > 0)
                {
                    if (!changed)
                    {
                        grafo.RepairNode(node, e.previousNode.label + 1);
                        changed = true;
                    }
                    //TODO capire se devo controllare anche changed o meno (probabilmente no, ma lo tengo comunque per sicurezza)
                    else if (changed && e.previousNode.label < node.label)
                        grafo.ChangeLabel(node, e.previousNode.label + 1);
                }
            }
            return changed;

        }
        public static int doBfs(Graph grafo)
        {
            Queue<Node> coda;
            if (grafo.invalidNode.Count == 0)
            {
                grafo.ResetLabel(0);
                coda = new Queue<Node>();
                coda.Enqueue(grafo.Source);
            }
            else
            {
                Node x = grafo.invalidNode.MinBy(x => x.label);
                //TODO capire come inserire sia i nodi che precedono x sia i nodi con label x 
                coda = new Queue<Node>(x.edges.Where(e => e.nextNode == x).Select(x => x.previousNode).Union(grafo.labeledNode[x.label]));
                grafo.ResetLabel(x);
                grafo.ResetLabel(x.label + 1);

            }
            while (coda.Count > 0)
            {
                var element = coda.Dequeue();
                foreach (BiEdge edge in element.edges)
                {
                    Node n = edge.nextNode;
                    if (edge.capacity < 0)
                        throw new InvalidOperationException();
                    if (edge.capacity == 0 && !grafo.invalidNode.Contains(n))
                    {
                        grafo.InvalidNode(n);
                        if (!Repair(grafo, n))
                            continue;
                        //TODO controllare che vada bene questo metodo per riparare i nodi
                    }
                    if (n.previousNode == null && edge.capacity > 0)
                    {
                        n.setPreviousNode(element);
                        grafo.ChangeLabel(n, element.label + 1);
                        n.setInFlow(Math.Min(element.inFlow, edge.capacity));
                        if (n is SinkNode)
                            return n.inFlow;
                        else
                            coda.Enqueue(n);

                    }
                }
            }

            return 0;
        }
        //TODO capire se ho un nodo malato se devo fare le stesso operazione oppure no
        private static int doBfs(Graph grafo, Node node)
        {
            HashSet<Node> set = grafo.labeledNode[node.label - 1];
            grafo.ResetLabel(node.label + 1);
            grafo.ResetLabel(node);
            var coda = new Queue<Node>(grafo.labeledNode[node.label]);
            while (coda.Count > 0)
            {
                var element = coda.Dequeue();
                foreach (var edge in element.edges)
                {
                    if (edge.capacity < 0)
                        throw new InvalidOperationException();

                }

            }


            return 0;
        }

        public static void PrintGraph(Graph grafo)
        {
            foreach (var set in grafo.labeledNode.Append(grafo.invalidNode))
            {
                foreach (var node in set)
                {
                    Console.Write("node " + node.name + " label = " + node.label);
                    foreach (var x in node.edges.Where(x => x.previousNode == node))
                        Console.Write(" to " + x.nextNode.name + ", f = " + x.flow + ", c  = " + x.capacity + ";");
                    Console.WriteLine();
                }
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
                int f = BfsLastLevelOpt.doBfs(grafo);
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