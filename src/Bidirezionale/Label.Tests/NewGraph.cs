using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bidirezionale.Label.LastLevelOpt;
using Bidirezionale.Label.LastLevelOptEdgeFlow;
using Bidirezionale.Label.NoOpt;
using Bidirezionale.Label.SickPropagation;
using Xunit;
using Xunit.Sdk;
using EFGraph = Bidirezionale.Label.LastLevelOptEdgeFlow.Graph;
using EFNode = Bidirezionale.Label.LastLevelOptEdgeFlow.Node;
using LLOGraph = Bidirezionale.Label.LastLevelOpt.Graph;
using LLONode = Bidirezionale.Label.LastLevelOpt.Node;
using NOGraph = Bidirezionale.Label.NoOpt.Graph;
using NONode = Bidirezionale.Label.NoOpt.Node;
using SPGraph = Bidirezionale.Label.SickPropagation.Graph;
using SPNode = Bidirezionale.Label.SickPropagation.Node;

namespace Bidirezionale.Label.Tests
{
    public class NewGraph
    {
        public static (int, NOGraph, SPGraph, EFGraph) CreateGraph(int cardNodes)
        {
            //seed usati : 42,          valore out : 80521, 
            //Random rnd1 = new();
            //int rand = rnd1.Next();
            Random random = new(1779933806);
            //Console.WriteLine(rand);
            NOGraph grafoNO = new(cardNodes);
            //LLOGraph grafoLLO = new(cardNodes);
            SPGraph grafoSP = new(cardNodes);
            EFGraph grafoEF = new(cardNodes);
            int cardEdges = 0;

            List<NONode> nodesNO = new(cardNodes);
            //List<LLONode> nodesLLO = new(cardNodes);
            List<SPNode> nodesSP = new(cardNodes);
            List<EFNode> nodesEF = new(cardNodes);

            NONode sNO = new NoOpt.SourceNode("0");
            LLONode sLLO = new LastLevelOpt.SourceNode("0");
            EFNode sEF = new LastLevelOptEdgeFlow.SourceNode("0");
            SPNode sSP = new SickPropagation.SourceNode("0");
            grafoNO.AddNode(sNO);
            nodesNO.Add(sNO);
            //grafoLLO.AddNode(sLLO);
            //nodesLLO.Add(sLLO);
            grafoSP.AddNode(sSP);
            nodesSP.Add(sSP);
            grafoEF.AddNode(sEF);
            nodesEF.Add(sEF);
            for (int i = 1; i < cardNodes; i++)
            {
                NONode nNO = new(i.ToString());
                LLONode nLLO = new(i.ToString());
                SPNode nSP = new(i.ToString());
                EFNode nEF = new(i.ToString());
                grafoNO.AddNode(nNO);
                nodesNO.Add(nNO);
                //grafoLLO.AddNode(nLLO);
                //nodesLLO.Add(nLLO);
                grafoSP.AddNode(nSP);
                nodesSP.Add(nSP);
                grafoEF.AddNode(nEF);
                nodesEF.Add(nEF);

            }
            var tNO = new NoOpt.SinkNode(cardNodes.ToString());
            //var tLLO = new LastLevelOpt.SinkNode(cardNodes.ToString());
            var tSP = new SickPropagation.SinkNode(cardNodes.ToString());
            var tEF = new LastLevelOptEdgeFlow.SinkNode(cardNodes.ToString());
            grafoNO.AddNode(tNO);
            nodesNO.Add(tNO);
            //grafoLLO.AddNode(tLLO);
            //nodesLLO.Add(tLLO);
            grafoSP.AddNode(tSP);
            nodesSP.Add(tSP);
            grafoEF.AddNode(tEF);
            nodesEF.Add(tEF);

            for (int i = 0; i < cardNodes; i++)
            {
                var nNO = nodesNO[i];
                //var nLLO = nodesLLO[i];
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
                        //nLLO.AddEdge(nodesLLO[x], cap);
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
            var res1 = BiLabelNoOpt.FlowFordFulkerson(graphNO);
            watch.Stop();
            Console.WriteLine($"Bidirectional Label NoOpt Execution Time: {watch.ElapsedMilliseconds} ms");

            /*             watch.Restart();
                        var res2 = LastLevelOpt.BiLabelLastLevelOpt.FlowFordFulkerson(graphLLO);
                        watch.Stop();
                        Console.WriteLine($"Last Level Opt Execution Time: {watch.ElapsedMilliseconds} ms");
             */
            watch.Restart();
            var res2 = LastLevelOptEdgeFlow.BiLabelLastLevelOpt.FlowFordFulkerson(graphEF);
            watch.Stop();
            Console.WriteLine($"Bidirectional Label Last Level Opt Execution Time: {watch.ElapsedMilliseconds} ms");

            watch.Restart();
            var res3 = BiLabelSickPropagation.FlowFordFulkerson(graphSP);
            watch.Stop();
            Console.WriteLine($"Bidirectional Label Sick Propagation Execution Time: {watch.ElapsedMilliseconds} ms");

            if (res1 != res2)
                throw new InvalidOperationException("r2 != r1");
            if (res2 != res3)
                throw new InvalidOperationException("r2 != r3");
            Assert.Equal(39083, res1);
        }
        [Fact]
        public void TestOneNewGraph()
        {
            int cardNodes = 10000;
            var grafo = CreateOneGraph(cardNodes);
            Console.WriteLine("Graph created");
            var watch = new Stopwatch();
            watch.Start();
            //var res = BiLabelNoOpt.FlowFordFulkerson(grafo);
            //var res = LastLevelOpt.BiLabelLastLevelOpt.FlowFordFulkerson(grafo);
            //var res = LastLevelOptEdgeFlow.BiLabelLastLevelOpt.FlowFordFulkerson(grafo);
            var res = BiLabelSickPropagation.FlowFordFulkerson(grafo);
            watch.Stop();
            Assert.Equal(27929, res);
        }
        private static SPGraph CreateOneGraph(int cardNodes)
        {
            {
                //seed usati : 42,          valore out : 80521, 
                Random random = new(1598168404);
                SPGraph grafo = new(cardNodes);

                List<SPNode> nodes = new(cardNodes);

                //var s = new NoOpt.SourceNode("0");
                //var s = new LastLevelOpt.SourceNode("0");
                //var s = new LastLevelOptEdgeFlow.SourceNode("0");
                var s = new SickPropagation.SourceNode("0");
                grafo.AddNode(s);
                nodes.Add(s);
                for (int i = 1; i < cardNodes; i++)
                {
                    SPNode n = new(i.ToString());
                    grafo.AddNode(n);
                    nodes.Add(n);
                }
                //var t = new NoOpt.SinkNode(cardNodes.ToString());
                //var t = new LastLevelOpt.SinkNode(cardNodes.ToString());
                //var t = new LastLevelOptEdgeFlow.SinkNode(cardNodes.ToString());
                var t = new SickPropagation.SinkNode(cardNodes.ToString());
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