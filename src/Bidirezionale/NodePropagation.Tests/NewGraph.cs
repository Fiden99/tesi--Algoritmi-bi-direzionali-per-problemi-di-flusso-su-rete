using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bidirezionale.NodePropagation.LastLevelOpt;
using Bidirezionale.NodePropagation.LastLevelOptEdgeFlow;
using Bidirezionale.NodePropagation.NoOpt;
using Bidirezionale.NodePropagation.SickPropagation;
using Xunit;
using Xunit.Sdk;
using EFGraph = Bidirezionale.NodePropagation.LastLevelOptEdgeFlow.Graph;
using EFNode = Bidirezionale.NodePropagation.LastLevelOptEdgeFlow.Node;
using NOGraph = Bidirezionale.NodePropagation.NoOpt.Graph;
using NONode = Bidirezionale.NodePropagation.NoOpt.Node;
using SPGraph = Bidirezionale.NodePropagation.SickPropagation.Graph;
using SPNode = Bidirezionale.NodePropagation.SickPropagation.Node;

namespace Bidirezionale.NodePropagation.Tests
{
    public class NewGraph
    {
        public static (int, NOGraph, SPGraph, EFGraph) CreateGraph(int cardNodes)
        {
            //seed usati : 42,          valore out : 80521, 
            //Random rnd1 = new();
            //int rand = rnd1.Next();
            Random random = new(78946516);
            //Console.WriteLine(rand);
            NOGraph grafoNO = new(cardNodes);
            SPGraph grafoSP = new(cardNodes);
            EFGraph grafoEF = new(cardNodes);
            int cardEdges = 0;

            List<NONode> nodesNO = new(cardNodes);
            List<SPNode> nodesSP = new(cardNodes);
            List<EFNode> nodesEF = new(cardNodes);

            NONode sNO = new NoOpt.SourceNode("0");
            EFNode sEF = new LastLevelOptEdgeFlow.SourceNode("0");
            SPNode sSP = new SickPropagation.SourceNode("0");
            grafoNO.AddNode(sNO);
            nodesNO.Add(sNO);
            grafoSP.AddNode(sSP);
            nodesSP.Add(sSP);
            grafoEF.AddNode(sEF);
            nodesEF.Add(sEF);
            for (int i = 1; i < cardNodes; i++)
            {
                NONode nNO = new(i.ToString());
                SPNode nSP = new(i.ToString());
                EFNode nEF = new(i.ToString());
                grafoNO.AddNode(nNO);
                nodesNO.Add(nNO);
                grafoSP.AddNode(nSP);
                nodesSP.Add(nSP);
                grafoEF.AddNode(nEF);
                nodesEF.Add(nEF);

            }
            var tNO = new NoOpt.SinkNode(cardNodes.ToString());
            var tSP = new SickPropagation.SinkNode(cardNodes.ToString());
            var tEF = new LastLevelOptEdgeFlow.SinkNode(cardNodes.ToString());
            grafoNO.AddNode(tNO);
            nodesNO.Add(tNO);
            grafoSP.AddNode(tSP);
            nodesSP.Add(tSP);
            grafoEF.AddNode(tEF);
            nodesEF.Add(tEF);

            for (int i = 0; i < cardNodes; i++)
            {
                var nNO = nodesNO[i];
                var nSP = nodesSP[i];
                var nEF = nodesEF[i];
                var numArc = random.Next(1, cardNodes - i + 1) % 1000;
                cardEdges += numArc;
                for (int x = i + 1; x <= i + numArc; x++)
                {
                    var cap = random.Next(0, 10000);
                    if (cap > 0)
                    {
                        nNO.AddEdge(nodesNO[x], cap);
                        nEF.AddEdge(nodesEF[x], cap);
                        nSP.AddEdge(nodesSP[x], cap);
                    }
                }
            }
            return (cardEdges, grafoNO, grafoSP, grafoEF);
        }
        [Fact]
        public void TestNewGraph()
        {
            int cardNodes = 10000;
            var (cardEdges, graphNO, graphSP, graphEF) = CreateGraph(cardNodes);
            Console.WriteLine("Graph created,n = " + cardNodes + ", m = " + cardEdges);
            var watch = new Stopwatch();
            watch.Start();
            var res1 = BiNodePropagationNoOpt.FlowFordFulkerson(graphNO);
            watch.Stop();
            Console.WriteLine($"Bidirectional NodePropagation NoOpt Execution Time: {watch.ElapsedMilliseconds} ms");

            /*             watch.Restart();
                        var res2 = LastLevelOpt.BiNodePropagationLastLevelOpt.FlowFordFulkerson(graphLLO);
                        watch.Stop();
                        Console.WriteLine($"Last Level Opt Execution Time: {watch.ElapsedMilliseconds} ms");
             */
            watch.Restart();
            var res2 = BiNodePropagationSickPropagation.FlowFordFulkerson(graphSP);
            watch.Stop();
            Console.WriteLine($"Bidirectional NodePropagation Sick Propagation Execution Time: {watch.ElapsedMilliseconds} ms");

            watch.Restart();
            var res3 = LastLevelOptEdgeFlow.BiNodePropagationLastLevelOpt.FlowFordFulkerson(graphEF);
            watch.Stop();
            Console.WriteLine($"Bidirectional NodePropagation Last Level Opt Execution Time: {watch.ElapsedMilliseconds} ms");

            if (res1 != res2)
                throw new InvalidOperationException("r2 != r1");
            if (res2 != res3)
                throw new InvalidOperationException("r2 != r3");
            Assert.Equal(69985, res1);
        }
        [Fact]
        public void TestOneNewGraph()
        {
            int cardNodes = 10000;
            var grafo = CreateOneGraph(cardNodes);
            Console.WriteLine("Graph created");
            var watch = new Stopwatch();
            watch.Start();
            //var res = BiNodePropagationNoOpt.FlowFordFulkerson(grafo);
            //var res = LastLevelOpt.BiNodePropagationLastLevelOpt.FlowFordFulkerson(grafo);
            var res = LastLevelOptEdgeFlow.BiNodePropagationLastLevelOpt.FlowFordFulkerson(grafo);
            //var res = BiNodePropagationSickPropagation.FlowFordFulkerson(grafo);
            watch.Stop();
            Assert.Equal(69985, res);
        }
        private static EFGraph CreateOneGraph(int cardNodes)
        {
            {
                //seed usati : 42,          valore out : 80521, 
                Random random = new(42);
                EFGraph grafo = new(cardNodes);

                List<EFNode> nodes = new(cardNodes);

                //var s = new NoOpt.SourceNode("0");
                //var s = new LastLevelOpt.SourceNode("0");
                //var s = new SickPropagation.SourceNode("0");
                var s = new LastLevelOptEdgeFlow.SourceNode("0");
                grafo.AddNode(s);
                nodes.Add(s);
                for (int i = 1; i < cardNodes; i++)
                {
                    EFNode n = new(i.ToString());
                    grafo.AddNode(n);
                    nodes.Add(n);
                }
                //var t = new NoOpt.SinkNode(cardNodes.ToString());
                //var t = new LastLevelOpt.SinkNode(cardNodes.ToString());
                //var t = new SickPropagation.SinkNode(cardNodes.ToString());
                var t = new LastLevelOptEdgeFlow.SinkNode(cardNodes.ToString());
                grafo.AddNode(t);
                nodes.Add(t);

                for (int i = 0; i < cardNodes; i++)
                {
                    var n = nodes[i];
                    var numArc = random.Next(1, cardNodes - i + 1) % (cardNodes / 10);
                    ;
                    for (int x = i + 1; x <= i + numArc; x++)
                    {
                        var cap = random.Next(0, 10000);
                        if (cap > 0)
                        {
                            n.AddEdge(nodes[x], cap);
                        }
                    }
                }
                return grafo;
            }

        }
    }
}