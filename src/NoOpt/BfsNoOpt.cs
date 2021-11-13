using System;
using System.Collections.Generic;

namespace BFS
{
    public class BfsNoOpt
    {
        public static int doBfs(Graph grafo)
        {
            grafo.resetLabel();
            var coda = new Queue<Node>();
            coda.Enqueue(grafo.Source);
            while (coda.Count > 0)
            {
                var element = coda.Dequeue();
                foreach (MonoEdge x in element.next)
                {
                    Node n = x.nextNode;
                    if (x.capacity < 0)
                        throw new InvalidOperationException("capacità negativa");

                    else if ( n.previousNode == null && x.capacity > 0)
                    {
                        n.setPreviousNode(element);
                        n.initLabel(element.label + 1);
                        n.setInFlow(Math.Min(element.inFlow, x.capacity));
                        if (n is SinkNode)
                            return n.inFlow;
                        else
                            coda.Enqueue(n);
                    }
                }
            }
            return 0;
        }

    }
}