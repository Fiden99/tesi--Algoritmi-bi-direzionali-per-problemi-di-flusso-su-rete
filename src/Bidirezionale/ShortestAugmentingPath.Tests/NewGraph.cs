using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bidirezionale.ShortestAugmentingPath;
using Xunit;

namespace ShortestAugmetingPath.Tests
{
    public class NewGraph
    {
        [Fact]
        public void TestNewGraph()
        {
            int numNodes = 10000;
            var (numArcs, graph) = CreateGraph(numNodes);
            Console.WriteLine("Graph created, number of nodes = " + numNodes + 1 + ", number of arcs = " + numArcs);
            Stopwatch watch = new();
            watch.Start();
            var res = ShortestAugmentingPath.FlowFordFulkerson(graph);
            watch.Stop();
            Console.WriteLine($"Bidirectional Shortest Augmenting Path Execution Time: {watch.ElapsedMilliseconds} ms");

            Assert.Equal(39083, res);

        }
        private static (int, Graph) CreateGraph(int cardNodes)
        {
            //seed usati : 42,          valore out : 80521, 
            Random random = new(1779933806);
            Graph grafo = new(cardNodes);
            int cardArcs = 0;
            List<Node> nodes = new(cardNodes);

            var s = new SourceNode("0");
            grafo.AddNode(s);
            nodes.Add(s);
            for (int i = 1; i < cardNodes; i++)
            {
                Node n = new(i.ToString());
                grafo.AddNode(n);
                nodes.Add(n);
            }
            //var t = new NoOpt.SinkNode(cardNodes.ToString());
            //var t = new LastLevelOpt.SinkNode(cardNodes.ToString());
            //var t = new SickPropagation.SinkNode(cardNodes.ToString());
            var t = new SinkNode(cardNodes.ToString());
            grafo.AddNode(t);
            nodes.Add(t);

            for (int i = 0; i < cardNodes; i++)
            {
                var n = nodes[i];
                var numArc = random.Next(1, cardNodes - i + 1) % (cardNodes / 10);
                cardArcs += numArc;
                for (int x = i + 1; x <= i + numArc; x++)
                {
                    var cap = random.Next(0, 10000);
                    if (cap > 0)
                    {
                        n.AddEdge(nodes[x], cap);
                    }
                }
            }
            return (cardArcs, grafo);
        }

    }

}

