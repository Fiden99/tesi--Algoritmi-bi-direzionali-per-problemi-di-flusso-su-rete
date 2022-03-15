using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bidirezionale.NodePropagation.LastLevelOptEdgeFlow;
using Bidirezionale.NodePropagation.NoOpt;
using Bidirezionale.NodePropagation.SickPropagation;
using LLOGraph = Bidirezionale.NodePropagation.LastLevelOptEdgeFlow.Graph;
using LLONode = Bidirezionale.NodePropagation.LastLevelOptEdgeFlow.Node;
using NOGraph = Bidirezionale.NodePropagation.NoOpt.Graph;
using NONode = Bidirezionale.NodePropagation.NoOpt.Node;
using SPGraph = Bidirezionale.NodePropagation.SickPropagation.Graph;
using SPNode = Bidirezionale.NodePropagation.SickPropagation.Node;

namespace MaxFlow
{
    public class RandomProgram
    {
        public static (int, NOGraph, LLOGraph, SPGraph) CreateGraph(int cardNodes, int seed)
        {
            Random random = new(seed);
            NOGraph grafoNO = new(cardNodes);
            LLOGraph grafoLLO = new(cardNodes);
            SPGraph grafoSP = new(cardNodes);
            int cardEdges = 0;

            List<NONode> nodesNO = new(cardNodes);
            List<LLONode> nodesLLO = new(cardNodes);
            List<SPNode> nodesSP = new(cardNodes);

            NONode sNO = new Bidirezionale.NodePropagation.NoOpt.SourceNode("0");
            LLONode sLLO = new Bidirezionale.NodePropagation.LastLevelOptEdgeFlow.SourceNode("0");
            SPNode sSP = new Bidirezionale.NodePropagation.SickPropagation.SourceNode("0");
            grafoNO.AddNode(sNO);
            nodesNO.Add(sNO);
            grafoLLO.AddNode(sLLO);
            nodesLLO.Add(sLLO);
            grafoSP.AddNode(sSP);
            nodesSP.Add(sSP);
            for (int i = 1; i < cardNodes; i++)
            {
                NONode nNO = new(i.ToString());
                LLONode nLLO = new(i.ToString());
                SPNode nSP = new(i.ToString());
                grafoNO.AddNode(nNO);
                nodesNO.Add(nNO);
                grafoLLO.AddNode(nLLO);
                nodesLLO.Add(nLLO);
                grafoSP.AddNode(nSP);
                nodesSP.Add(nSP);

            }
            var tNO = new Bidirezionale.NodePropagation.NoOpt.SinkNode(cardNodes.ToString());
            var tLLO = new Bidirezionale.NodePropagation.LastLevelOptEdgeFlow.SinkNode(cardNodes.ToString());
            var tSP = new Bidirezionale.NodePropagation.SickPropagation.SinkNode(cardNodes.ToString());
            grafoNO.AddNode(tNO);
            nodesNO.Add(tNO);
            grafoLLO.AddNode(tLLO);
            nodesLLO.Add(tLLO);
            grafoSP.AddNode(tSP);
            nodesSP.Add(tSP);

            for (int i = 0; i < cardNodes; i++)
            {
                var nNO = nodesNO[i];
                var nLLO = nodesLLO[i];
                var nSP = nodesSP[i];
                var numArc = random.Next(1, cardNodes - i + 1) % 1000;
                cardEdges += numArc;
                for (int x = i + 1; x <= i + numArc; x++)
                {
                    var cap = random.Next(0, 10000);
                    if (cap > 0)
                    {
                        nNO.AddEdge(nodesNO[x], cap);
                        nLLO.AddEdge(nodesLLO[x], cap);
                        nSP.AddEdge(nodesSP[x], cap);
                    }
                }
            }
            return (cardEdges, grafoNO, grafoLLO, grafoSP);
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
            var (cardEdges, graphNO, graphLLO, graphSP) = CreateGraph(cardNodes, seed);
            Console.WriteLine("Graph created,n = " + cardNodes + ", m = " + cardEdges);
            var watch = new Stopwatch();
            watch.Start();
            var res1 = BiNodePropagationNoOpt.FlowFordFulkerson(graphNO);
            watch.Stop();
            Console.WriteLine($"NoOpt Execution Time: {watch.ElapsedMilliseconds} ms");
            var x1 = watch.ElapsedMilliseconds;
            watch.Restart();
            var res2 = BiNodePropagationLastLevelOpt.FlowFordFulkerson(graphLLO);
            watch.Stop();
            Console.WriteLine($"Last Level Opt Execution Time: {watch.ElapsedMilliseconds} ms");
            var x2 = watch.ElapsedMilliseconds;

            watch.Restart();
            var res3 = BiNodePropagationSickPropagation.FlowFordFulkerson(graphSP);
            watch.Stop();
            Console.WriteLine($"Sick Propagation Execution Time: {watch.ElapsedMilliseconds} ms");
            var x3 = watch.ElapsedMilliseconds;

            Console.WriteLine("NoOpt = " + res1 + ", LLO = " + res2 + ", SP = " + res3);
            if (res1 != res2 || res2 != res3)
                throw new InvalidOperationException("valori diversi");
            Console.WriteLine(seed + " & " + res1 + " & " + (cardNodes + 1) + " & " + cardEdges + " & " + x1 + " & " + x2 + " & " + x3 + "\\");

        }

    }
}