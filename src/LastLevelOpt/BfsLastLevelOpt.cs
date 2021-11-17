using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//TODO capire se nella bfs privata e in quella pubblica è possibile riciclare il codice dentro una funzione 
namespace LastLevelOpt
{
    public class BfsLastLevelOpt
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
    }
}