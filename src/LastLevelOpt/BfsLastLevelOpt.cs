using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//TODO da completare ottimizzazione 
namespace LastLevelOpt
{
    public class BfsLastLevelOpt
    {
        public static int doBfs(Graph grafo)
        {
            var coda = new Queue<Node>();
            coda.Enqueue(grafo.Source);
            while (coda.Count > 0)
            {
                //fare if (grafo.invalidNode.isEmpty()) ???
                var element = coda.Dequeue();
                foreach (BiEdge edge in element.edges)
                {
                    Node n = edge.nextNode;
                    if (edge.capacity < 0)
                        throw new InvalidOperationException();
                    if (edge.capacity == 0)
                    {
                        grafo.InvalidNode(n);
                        if (!Repair(grafo,n))
                            break;
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
        //TODO continuare con il node interessato
        private static int doBfs(Graph grafo, Node node)
        {
            return 0;
        }
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
    }
}