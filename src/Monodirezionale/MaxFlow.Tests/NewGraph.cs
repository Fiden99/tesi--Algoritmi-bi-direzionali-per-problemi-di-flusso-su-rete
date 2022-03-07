using System;
using System.Collections.Generic;
using System.Diagnostics;
using Monodirezionale.MaxFlow.LastLevelOpt;
using Monodirezionale.MaxFlow.NoOpt;
using Monodirezionale.MaxFlow.ShortestAugmentingPath;
using Monodirezionale.MaxFlow.SickPropagation;
using Xunit;
using LLOGraph = Monodirezionale.MaxFlow.LastLevelOpt.Graph;
using LLONode = Monodirezionale.MaxFlow.LastLevelOpt.Node;
using NOGraph = Monodirezionale.MaxFlow.NoOpt.Graph;
using NONode = Monodirezionale.MaxFlow.NoOpt.Node;
using SAPGraph = Monodirezionale.MaxFlow.ShortestAugmentingPath.Graph;
using SAPNode = Monodirezionale.MaxFlow.ShortestAugmentingPath.Node;
using SPGraph = Monodirezionale.MaxFlow.SickPropagation.Graph;
using SPNode = Monodirezionale.MaxFlow.SickPropagation.Node;

namespace MaxFlow.Tests
{
    public class NewGraph
    {
        public static (int, NOGraph, LLOGraph, SPGraph, SAPGraph) CreateGraph(int cardNodes)
        {
            //seed usati : 42,          valore out : 80521, 
            //Random rnd1 = new();
            //int rand = rnd1.Next();
            Random random = new(42);
            //Console.WriteLine(rand);
            NOGraph grafoNO = new(cardNodes);
            LLOGraph grafoLLO = new(cardNodes);
            SPGraph grafoSP = new(cardNodes);
            SAPGraph grafoSAP = new(cardNodes);
            int cardEdges = 0;

            List<NONode> nodesNO = new(cardNodes);
            List<LLONode> nodesLLO = new(cardNodes);
            List<SPNode> nodesSP = new(cardNodes);
            List<SAPNode> nodesSAP = new(cardNodes);

            NONode sNO = new Monodirezionale.MaxFlow.NoOpt.SourceNode("0");
            LLONode sLLO = new Monodirezionale.MaxFlow.LastLevelOpt.SourceNode("0");
            SAPNode sSAP = new Monodirezionale.MaxFlow.ShortestAugmentingPath.SourceNode("0");
            SPNode sSP = new Monodirezionale.MaxFlow.SickPropagation.SourceNode("0");
            grafoNO.AddNode(sNO);
            nodesNO.Add(sNO);
            grafoLLO.AddNode(sLLO);
            nodesLLO.Add(sLLO);
            grafoSP.AddNode(sSP);
            nodesSP.Add(sSP);
            grafoSAP.AddNode(sSAP);
            nodesSAP.Add(sSAP);
            for (int i = 1; i < cardNodes; i++)
            {
                NONode nNO = new(i.ToString());
                LLONode nLLO = new(i.ToString());
                SPNode nSP = new(i.ToString());
                SAPNode nSAP = new(i.ToString());
                grafoNO.AddNode(nNO);
                nodesNO.Add(nNO);
                grafoLLO.AddNode(nLLO);
                nodesLLO.Add(nLLO);
                grafoSP.AddNode(nSP);
                nodesSP.Add(nSP);
                grafoSAP.AddNode(nSAP);
                nodesSAP.Add(nSAP);

            }
            var tNO = new Monodirezionale.MaxFlow.NoOpt.SinkNode(cardNodes.ToString());
            var tLLO = new Monodirezionale.MaxFlow.LastLevelOpt.SinkNode(cardNodes.ToString());
            var tSP = new Monodirezionale.MaxFlow.SickPropagation.SinkNode(cardNodes.ToString());
            var tSAP = new Monodirezionale.MaxFlow.ShortestAugmentingPath.SinkNode(cardNodes.ToString());
            grafoNO.AddNode(tNO);
            nodesNO.Add(tNO);
            grafoLLO.AddNode(tLLO);
            nodesLLO.Add(tLLO);
            grafoSP.AddNode(tSP);
            nodesSP.Add(tSP);
            grafoSAP.AddNode(tSAP);
            nodesSAP.Add(tSAP);

            for (int i = 0; i < cardNodes; i++)
            {
                var nNO = nodesNO[i];
                var nLLO = nodesLLO[i];
                var nSP = nodesSP[i];
                var nSAP = nodesSAP[i];
                var numArc = random.Next(1, cardNodes - i + 1) % 1000;
                cardEdges += numArc;
                for (int x = i + 1; x <= i + numArc; x++)
                {
                    var cap = random.Next(0, 10000);
                    if (cap > 0)
                    {
                        nNO.AddNext(nodesNO[x], cap);
                        nLLO.AddEdge(nodesLLO[x], cap);
                        nSAP.AddEdge(nodesSAP[x], cap);
                        nSP.AddEdge(nodesSP[x], cap);
                    }
                }
            }
            return (cardEdges, grafoNO, grafoLLO, grafoSP, grafoSAP);
        }
        [Fact]
        public void TestNewGraph()
        {
            int cardNodes = 10000;
            var (cardEdges, graphNO, graphLLO, graphSP, graphSAP) = CreateGraph(cardNodes);
            Console.WriteLine("Graph created,n = " + cardNodes + ", m = " + cardEdges);
            var watch = new Stopwatch();
            watch.Start();
            var res1 = BfsNoOpt.FlowFordFulkerson(graphNO);
            watch.Stop();
            Console.WriteLine($"NoOpt Execution Time: {watch.ElapsedMilliseconds} ms");

            watch.Restart();
            var res2 = BfsLastLevelOpt.FlowFordFulkerson(graphLLO);
            watch.Stop();
            Console.WriteLine($"Last Level Opt Execution Time: {watch.ElapsedMilliseconds} ms");

            watch.Restart();
            var res3 = BfsSickPropagation.FlowFordFulkerson(graphSP);
            watch.Stop();
            Console.WriteLine($"Sick Propagation Execution Time: {watch.ElapsedMilliseconds} ms");

            watch.Restart();
            var res4 = ShortestAugmentingPath.FlowFordFulkerson(graphSAP);
            watch.Stop();
            Console.WriteLine($"Shortest Augmenting Path Execution Time: {watch.ElapsedMilliseconds} ms");

            if (res1 != res2 || res2 != res3 || res3 != res4)
                throw new InvalidOperationException("valori diversi");
            Assert.Equal(69985, res1);
        }
        [Fact]
        public void TestOneNewGraph()
        {
            var graph = CreateOneGraph(10000);
            Console.WriteLine("Graph created");
            var watch = new Stopwatch();
            watch.Start();
            var res = BfsNoOpt.FlowFordFulkerson(graph);
            //var res = BfsLastLevelOpt.FlowFordFulkerson(graph);
            //var res= BfsSickPropagation.FlowFordFulkerson(graph);
            //var res = ShortestAugmentingPath.FlowFordFulkerson(graph);
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            Assert.Equal(69985, res);
        }

        private static NOGraph CreateOneGraph(int cardNodes)
        {
            //seed usati : 42,          valore out : 80521, 
            Random rand = new();
            int rnd = rand.Next();
            Random random = new(42);
            Console.WriteLine("seed = " + rnd);
            NOGraph grafo = new(cardNodes);

            List<NONode> nodes = new(cardNodes);

            NONode s = new Monodirezionale.MaxFlow.NoOpt.SourceNode("0");
            //LLONode s = new Monodirezionale.MaxFlow.LastLevelOpt.SourceNode("0");
            //SPNode s = new Monodirezionale.MaxFlow.SickPropagation.SourceNode("0");
            //SAPNode s = new Monodirezionale.MaxFlow.ShortestAugmentingPath.SourceNode("0");
            grafo.AddNode(s);
            nodes.Add(s);
            for (int i = 1; i < cardNodes; i++)
            {
                NONode n = new(i.ToString());
                grafo.AddNode(n);
                nodes.Add(n);
            }
            var t = new Monodirezionale.MaxFlow.NoOpt.SinkNode(cardNodes.ToString());
            //var t = new Monodirezionale.MaxFlow.LastLevelOpt.SinkNode(cardNodes.ToString());
            //var t = new Monodirezionale.MaxFlow.SickPropagation.SinkNode(cardNodes.ToString());
            //var t = new Monodirezionale.MaxFlow.ShortestAugmentingPath.SinkNode(cardNodes.ToString());
            grafo.AddNode(t);
            nodes.Add(t);

            for (int i = 0; i < cardNodes; i++)
            {
                var n = nodes[i];
                var numArc = random.Next(1, cardNodes - i + 1) % 1000;
                ;
                for (int x = i + 1; x <= i + numArc; x++)
                {
                    var cap = random.Next(0, 10000);
                    if (cap > 0)
                    {
                        n.AddNext(nodes[x], cap);
                    }
                }
            }
            return grafo;
        }
    }
}