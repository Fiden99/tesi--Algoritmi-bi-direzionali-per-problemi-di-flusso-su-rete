/* using System;
using System.Diagnostics;
using System.Linq;
using Monodirezionale.MaxFlow.LastLevelOpt;
using Monodirezionale.MaxFlow.NoOpt;
using Monodirezionale.MaxFlow.ShortestAugmentingPath;
using Monodirezionale.MaxFlow.SickPropagation;
using LLOGraph = Monodirezionale.MaxFlow.LastLevelOpt.Graph;
using LLONode = Monodirezionale.MaxFlow.LastLevelOpt.Node;
using NoOptGraph = Monodirezionale.MaxFlow.NoOpt.Graph;
using NoOptNode = Monodirezionale.MaxFlow.NoOpt.Node;
using SAPGraph = Monodirezionale.MaxFlow.ShortestAugmentingPath.Graph;
using SAPNode = Monodirezionale.MaxFlow.ShortestAugmentingPath.Node;
using SPGraph = Monodirezionale.MaxFlow.SickPropagation.Graph;
using SPNode = Monodirezionale.MaxFlow.SickPropagation.Node;

namespace MaxFlow
{
    public class Program
    {
        private static (NoOptGraph, LLOGraph, SAPGraph, SPGraph) Read(string[] stringhe)
        {
            NoOptGraph grafoNoOpt;
            LLOGraph grafoLLO;
            SAPGraph grafoSAP;
            SPGraph grafoSP;
            var str = stringhe[0].Split(" ");
            if (String.Equals(str[0], "p"))
            {
                grafoNoOpt = new(int.Parse(str[2]));
                grafoLLO = new(int.Parse(str[2]));
                grafoSAP = new(int.Parse(str[2]));
                grafoSP = new(int.Parse(str[2]));
                for (int i = 3; i < Int64.Parse(str[2]); i++)
                {
                    grafoNoOpt.AddNode(new NoOptNode(i.ToString()));
                    grafoLLO.AddNode(new LLONode(i.ToString()));
                    grafoSAP.AddNode(new SAPNode(i.ToString()));
                    grafoSP.AddNode(new SPNode(i.ToString()));
                }
            }
            else
                return (null, null, null, null);
            var s = stringhe[2].Split(" ");
            if (string.Equals(s[0], "n"))
                if (string.Equals(s[2], "s"))
                {
                    grafoNoOpt.AddNode(new Monodirezionale.MaxFlow.NoOpt.SourceNode(s[1]));
                    grafoLLO.AddNode(new Monodirezionale.MaxFlow.LastLevelOpt.SourceNode(s[1]));
                    grafoSAP.AddNode(new Monodirezionale.MaxFlow.ShortestAugmentingPath.SourceNode(s[1]));
                    grafoSP.AddNode(new Monodirezionale.MaxFlow.SickPropagation.SourceNode(s[1]));

                }
                else if (String.Equals(s[2], "t"))
                {
                    grafoNoOpt.AddNode(new Monodirezionale.MaxFlow.NoOpt.SinkNode(s[1]));
                    grafoLLO.AddNode(new Monodirezionale.MaxFlow.LastLevelOpt.SinkNode(s[1]));
                    grafoSAP.AddNode(new Monodirezionale.MaxFlow.ShortestAugmentingPath.SinkNode(s[1]));
                    grafoSP.AddNode(new Monodirezionale.MaxFlow.SickPropagation.SinkNode(s[1]));
                }
            s = stringhe[3].Split(" ");
            if (String.Equals(s[0], "n"))
                if (String.Equals(s[2], "s"))
                {
                    grafoNoOpt.AddNode(new Monodirezionale.MaxFlow.NoOpt.SourceNode(s[1]));
                    grafoLLO.AddNode(new Monodirezionale.MaxFlow.LastLevelOpt.SourceNode(s[1]));
                    grafoSAP.AddNode(new Monodirezionale.MaxFlow.ShortestAugmentingPath.SourceNode(s[1]));
                    grafoSP.AddNode(new Monodirezionale.MaxFlow.SickPropagation.SourceNode(s[1]));
                }
                else if (String.Equals(s[2], "t"))
                {
                    grafoNoOpt.AddNode(new Monodirezionale.MaxFlow.NoOpt.SinkNode(s[1]));
                    grafoLLO.AddNode(new Monodirezionale.MaxFlow.LastLevelOpt.SinkNode(s[1]));
                    grafoSAP.AddNode(new Monodirezionale.MaxFlow.ShortestAugmentingPath.SinkNode(s[1]));
                    grafoSP.AddNode(new Monodirezionale.MaxFlow.SickPropagation.SinkNode(s[1]));
                }
            foreach (var line in stringhe)
            {
                var x = line.Split(" ");
                if (String.Equals(x[0], "a") && int.Parse(x[3]) != 0)
                {//NoOpt
                    NoOptNode fNO = grafoNoOpt.Nodes.Single(m => String.Equals(x[1], m.Name));
                    NoOptNode tNO = grafoNoOpt.Nodes.Single(m => String.Equals(x[2], m.Name));
                    fNO.AddNext(tNO, int.Parse(x[3]));
                    //LLO
                    LLONode fLLO = grafoLLO.LabeledNode[0].Single(m => String.Equals(x[1], m.Name));
                    LLONode tLLO = grafoLLO.LabeledNode[0].Single(m => String.Equals(x[2], m.Name));
                    fLLO.AddEdge(tLLO, int.Parse(x[3]));
                    //SP
                    SPNode fSP = grafoSP.LabeledNodes[0].Single(m => String.Equals(x[1], m.Name));
                    SPNode tSP = grafoSP.LabeledNodes[0].Single(m => String.Equals(x[2], m.Name));
                    fSP.AddEdge(tSP, int.Parse(x[3]));
                    //SAP
                    SAPNode fSAP = grafoSAP.Nodes.Single(m => String.Equals(x[1], m.Name));
                    SAPNode tSAP = grafoSAP.Nodes.Single(m => String.Equals(x[2], m.Name));
                    fSAP.AddEdge(tSAP, int.Parse(x[3]));
                }
            }
            return (grafoNoOpt, grafoLLO, grafoSAP, grafoSP);
        }

        public static void Main()
        {
            var (graphNN, graphLLO, graphSAP, graphSP) = Read(System.IO.File.ReadAllLines(@"../../dataset/adhead.n6c10.max.bbk.max"));
            Performance(graphNN, graphLLO, graphSAP, graphSP);
        }
        static void Main(string[] x)
        {
            var args = int.Parse(x[0]);
            NoOptGraph graphNN;
            LLOGraph graphLLO;
            SAPGraph graphSAP;
            SPGraph graphSP;
            if (args == 1)
                (graphNN, graphLLO, graphSAP, graphSP) = Read(System.IO.File.ReadAllLines(@"../../dataset/adhead.n6c10.max.bbk.max"));
            else if (args == 2)
                (graphNN, graphLLO, graphSAP, graphSP) = Read(System.IO.File.ReadAllLines(@"../../dataset/adhead.n26c10.max.bbk.max"));
            else if (args == 3)
                (graphNN, graphLLO, graphSAP, graphSP) = Read(System.IO.File.ReadAllLines(@"../../dataset/adhead.n6c100.max.bbk.max"));
            else if (args == 4)
                (graphNN, graphLLO, graphSAP, graphSP) = Read(System.IO.File.ReadAllLines(@"../../dataset/adhead.n26c100.max.bbk.max"));
            else
                return;
            Performance(graphNN, graphLLO, graphSAP, graphSP);

        }


        public static void Performance(NoOptGraph graphNN, LLOGraph graphLLO, SAPGraph graphSAP, SPGraph graphSP)
        {
            if (graphNN is null || graphLLO is null || graphSAP is null || graphSP is null)
                throw new InvalidOperationException("graph = null");
            Stopwatch watch = new();
            Console.WriteLine("grafo letto");
            watch.Restart();
            var res1 = BfsNoOpt.FlowFordFulkerson(graphNN);
            watch.Stop();
            Console.WriteLine($"No Opt Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine("send flow = " + res1);

            watch.Restart();
            var res2 = BfsLastLevelOpt.FlowFordFulkerson(graphLLO);
            watch.Stop();
            Console.WriteLine($"LastLevelOpt Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine("send flow = " + res2);

            watch.Restart();
            var res3 = BfsSickPropagation.FlowFordFulkerson(graphSP);
            watch.Stop();
            Console.WriteLine($"sick Propagation Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine("send flow = " + res3);
            watch.Restart();

            var res4 = ShortestAugmentingPath.FlowFordFulkerson(graphSAP);
            watch.Stop();
            Console.WriteLine($"Shortest Augmentign Path Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine("send flow = " + res4);

        }
    }
}
 */