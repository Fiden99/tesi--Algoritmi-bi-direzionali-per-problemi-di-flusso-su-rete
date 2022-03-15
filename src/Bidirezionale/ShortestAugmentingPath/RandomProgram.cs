using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bidirezionale.ShortestAugmentingPath;

namespace ShortestAugmetingPath
{
    public class RandomProgram
    {
        public static (int, Graph) CreateGraph(int cardNodes, int seed)
        {
            Random random = new(seed);
            Graph grafo = new(cardNodes);
            int cardEdges = 0;

            List<Node> nodes = new(cardNodes);

            Node s = new SourceNode("0");
            grafo.AddNode(s);
            nodes.Add(s);
            for (int i = 1; i < cardNodes; i++)
            {
                Node n = new(i.ToString());
                grafo.AddNode(n);
                nodes.Add(n);
            }

            var t = new SinkNode(cardNodes.ToString());
            grafo.AddNode(t);
            nodes.Add(t);

            for (int i = 0; i < cardNodes; i++)
            {
                var n = nodes[i];
                var numArc = random.Next(1, cardNodes - i + 1) % 1000;
                cardEdges += numArc;
                for (int x = i + 1; x <= i + numArc; x++)
                {
                    var cap = random.Next(0, 10000);
                    if (cap > 0)
                    {
                        n.AddEdge(nodes[x], cap);
                    }
                }
            }
            return (cardEdges, grafo);
        }

        static void Main(string[] args)
        {
            int seed;
            if (args.Length == 0)
            {
                Random rnd = new();
                var x = rnd.Next();
                Console.WriteLine(x);
                seed = x;
            }
            else
                seed = int.Parse(args[0]);
            int cardNodes = 10000;
            var (cardEdges, graph) = CreateGraph(cardNodes, seed);
            Console.WriteLine("Graph created,n = " + cardNodes + ", m = " + cardEdges);
            var watch = new Stopwatch();
            watch.Start();
            var res = ShortestAugmentingPath.FlowFordFulkerson(graph);
            watch.Stop();
            Console.WriteLine($"SAP Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine("flusso inviato = " + res);

        }

    }
}