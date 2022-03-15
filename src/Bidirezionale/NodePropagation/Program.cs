/* using System;
using System.Diagnostics;
using System.Linq;
using Bidirezionale.NodePropagation.LastLevelOpt;
using Bidirezionale.NodePropagation.LastLevelOptEdgeFlow;
using Bidirezionale.NodePropagation.NoOpt;
using Bidirezionale.NodePropagation.SickPropagation;
using EFGraph = Bidirezionale.NodePropagation.LastLevelOptEdgeFlow.Graph;
using EFNode = Bidirezionale.NodePropagation.LastLevelOptEdgeFlow.Node;
using NoOptGraph = Bidirezionale.NodePropagation.NoOpt.Graph;
using NoOptNode = Bidirezionale.NodePropagation.NoOpt.Node;
using SPGraph = Bidirezionale.NodePropagation.SickPropagation.Graph;
using SPNode = Bidirezionale.NodePropagation.SickPropagation.Node;



namespace Bidirezionale.NodePropagation
{
    public class Program
    {
        private static (NoOptGraph, EFGraph, SPGraph) Read(string[] stringhe)
        {
            NoOptGraph grafoNoOpt;
            EFGraph grafoEF;
            SPGraph grafoSP;
            var str = stringhe[0].Split(" ");
            if (String.Equals(str[0], "p"))
            {
                grafoNoOpt = new(int.Parse(str[2]));
                grafoEF = new(int.Parse(str[2]));
                grafoSP = new(int.Parse(str[2]));


                for (int i = 3; i < Int64.Parse(str[2]); i++)
                {
                    grafoNoOpt.AddNode(new NoOptNode(i.ToString()));
                    grafoEF.AddNode(new EFNode(i.ToString()));
                    grafoSP.AddNode(new SPNode(i.ToString()));
                }
            }
            else
                return (null, null, null);
            var s = stringhe[2].Split(" ");
            if (string.Equals(s[0], "n"))
                if (string.Equals(s[2], "s"))
                {
                    grafoNoOpt.AddNode(new Bidirezionale.NodePropagation.NoOpt.SourceNode(s[1]), false);
                    grafoEF.AddNode(new Bidirezionale.NodePropagation.LastLevelOptEdgeFlow.SourceNode(s[1]), false);
                    grafoSP.AddNode(new Bidirezionale.NodePropagation.SickPropagation.SourceNode(s[1]), false);

                }
                else if (String.Equals(s[2], "t"))
                {
                    grafoNoOpt.AddNode(new Bidirezionale.NodePropagation.NoOpt.SinkNode(s[1]), true);
                    grafoEF.AddNode(new Bidirezionale.NodePropagation.LastLevelOptEdgeFlow.SinkNode(s[1]), true);
                    grafoSP.AddNode(new Bidirezionale.NodePropagation.SickPropagation.SinkNode(s[1]), true);
                }
            s = stringhe[3].Split(" ");
            if (String.Equals(s[0], "n"))
                if (String.Equals(s[2], "s"))
                {
                    grafoNoOpt.AddNode(new Bidirezionale.NodePropagation.NoOpt.SourceNode(s[1]), false);
                    grafoEF.AddNode(new Bidirezionale.NodePropagation.LastLevelOptEdgeFlow.SourceNode(s[1]), false);
                    grafoSP.AddNode(new Bidirezionale.NodePropagation.SickPropagation.SourceNode(s[1]), false);
                }
                else if (String.Equals(s[2], "t"))
                {
                    grafoNoOpt.AddNode(new Bidirezionale.NodePropagation.NoOpt.SinkNode(s[1]), true);
                    grafoEF.AddNode(new Bidirezionale.NodePropagation.LastLevelOptEdgeFlow.SinkNode(s[1]), true);
                    grafoSP.AddNode(new Bidirezionale.NodePropagation.SickPropagation.SinkNode(s[1]), true);
                }
            foreach (var line in stringhe)
            {
                var x = line.Split(" ");
                if (String.Equals(x[0], "a") && int.Parse(x[3]) != 0)
                {//NoOpt
                    NoOptNode fNO = grafoNoOpt.SourceNodes.Single(m => String.Equals(x[1], m.Name));
                    NoOptNode tNO = grafoNoOpt.SourceNodes.SingleOrDefault(m => String.Equals(x[2], m.Name));
                    if (tNO is null)
                        tNO = grafoNoOpt.SinkNodes.Single(m => String.Equals(x[2], m.Name));
                    fNO.AddEdge(tNO, int.Parse(x[3]));
                    //SP
                    SPNode fSP = grafoSP.LabeledNodeSourceSide[0].Single(m => String.Equals(x[1], m.Name));
                    SPNode tSP = grafoSP.LabeledNodeSourceSide[0].SingleOrDefault(m => String.Equals(x[2], m.Name));
                    if (tSP is null)
                        tSP = grafoSP.LabeledNodeSinkSide[0].Single(m => String.Equals(x[2], m.Name));
                    fSP.AddEdge(tSP, int.Parse(x[3]));
                    //SAP
                    EFNode fSAP = grafoEF.LabeledNodeSourceSide[0].Single(m => String.Equals(x[1], m.Name));
                    EFNode tSAP = grafoEF.LabeledNodeSourceSide[0].SingleOrDefault(m => String.Equals(x[2], m.Name));
                    if (tSAP is null)
                        tSAP = grafoEF.LabeledNodeSinkSide[0].Single(m => String.Equals(x[2], m.Name));
                    fSAP.AddEdge(tSAP, int.Parse(x[3]));
                }
            }
            return (grafoNoOpt, grafoEF, grafoSP);
        }

        public static void Main()
        {
            var (graphNN, grafoEF, graphSP) = Read(System.IO.File.ReadAllLines(@"../../dataset/adhead.n6c10.max.bbk.max"));
            Performance(graphNN, grafoEF, graphSP);
        }
        public static void Main(int args)
        {
            NoOptGraph graphNN;
            EFGraph graphEF;
            SPGraph graphSP;
            if (args == 1)
                (graphNN, graphEF, graphSP) = Read(System.IO.File.ReadAllLines(@"../../dataset/adhead.n6c10.max.bbk.max"));
            else if (args == 2)
                (graphNN, graphEF, graphSP) = Read(System.IO.File.ReadAllLines(@"../../dataset/adhead.n26c10.max.bbk.max"));
            else if (args == 3)
                (graphNN, graphEF, graphSP) = Read(System.IO.File.ReadAllLines(@"../../dataset/adhead.n6c100.max.bbk.max"));
            else if (args == 4)
                (graphNN, graphEF, graphSP) = Read(System.IO.File.ReadAllLines(@"../../dataset/adhead.n26c100.max.bbk.max"));
            else
                return;
            Performance(graphNN, graphEF, graphSP);

        }


        public static void Performance(NoOptGraph graphNN, EFGraph graphEF, SPGraph graphSP)
        {
            if (graphNN is null || graphEF is null || graphSP is null)
                throw new InvalidOperationException("graph = null");
            Stopwatch watch = new();
            Console.WriteLine("grafo letto");

            watch.Start();
            var res1 = BiNodePropagationNoOpt.FlowFordFulkerson(graphNN);
            watch.Stop();
            Console.WriteLine($"No Opt Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine("send flow = " + res1);

            watch.Restart();
            var res2 = LastLevelOptEdgeFlow.BiNodePropagationLastLevelOpt.FlowFordFulkerson(graphEF);
            watch.Stop();
            Console.WriteLine($"Last Level Opt Path Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine("send flow = " + res2);

            watch.Restart();
            var res3 = BiNodePropagationSickPropagation.FlowFordFulkerson(graphSP);
            watch.Stop();
            Console.WriteLine($"sick Propagation Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine("send flow = " + res3);
        }
    }
}
 */