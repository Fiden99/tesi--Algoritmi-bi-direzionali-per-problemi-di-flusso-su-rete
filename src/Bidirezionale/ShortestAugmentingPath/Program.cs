/* using System;
using System.Diagnostics;
using System.Linq;

namespace Bidirezionale.ShortestAugmentingPath
{
    public class Program
    {
        private static Graph Read(string[] stringhe)
        {
            Graph grafo;
            var str = stringhe[0].Split(" ");
            if (String.Equals(str[0], "p"))
            {
                grafo = new(int.Parse(str[2]));
                for (int i = 3; i < Int64.Parse(str[2]); i++)
                {
                    grafo.AddNode(new Node(i.ToString()));
                }
            }
            else
                return null;
            var s = stringhe[2].Split(" ");
            if (string.Equals(s[0], "n"))
                if (string.Equals(s[2], "s"))
                {
                    grafo.AddNode(new SourceNode(s[1]));

                }
                else if (String.Equals(s[2], "t"))
                {
                    grafo.AddNode(new SinkNode(s[1]));
                }
            s = stringhe[3].Split(" ");
            if (String.Equals(s[0], "n"))
                if (string.Equals(s[2], "s"))
                {
                    grafo.AddNode(new SourceNode(s[1]));

                }
                else if (String.Equals(s[2], "t"))
                {
                    grafo.AddNode(new SinkNode(s[1]));
                }
            foreach (var line in stringhe)
            {
                var x = line.Split(" ");
                if (String.Equals(x[0], "a") && int.Parse(x[3]) != 0)
                {
                    Node f = grafo.Nodes.Single(m => String.Equals(x[1], m.Name));
                    Node t = grafo.Nodes.Single(m => String.Equals(x[2], m.Name));
                    f.AddEdge(t, int.Parse(x[3]));
                }
            }
            return grafo;
        }
                 public static void Main()
                {
                    var grafo = Read(System.IO.File.ReadAllLines(@"../../dataset/adhead.n6c10.max.bbk.max"));
                    if (grafo == null)
                        throw new ArgumentNullException("grafo null");
                    Console.WriteLine("grafo letto");
                    Stopwatch watch = new();
                    watch.Start();
                    var res = ShortestAugmentingPath.FlowFordFulkerson(grafo);
                    watch.Stop();
                    Console.WriteLine($"Shortest augmenting path Execution Time: {watch.ElapsedMilliseconds} ms");
                    Console.WriteLine("send flow = " + res);
                }
        public static void Main(int args)
        {
            Graph grafo;
            if (args == 1)
                grafo = Read(System.IO.File.ReadAllLines(@"../../dataset/adhead.n6c10.max.bbk.max"));
            else if (args == 2)
                grafo = Read(System.IO.File.ReadAllLines(@"../../dataset/adhead.n26c10.max.bbk.max"));
            else if (args == 3)
                grafo = Read(System.IO.File.ReadAllLines(@"../../dataset/adhead.n6c100.max.bbk.max"));
            else if (args == 4)
                grafo = Read(System.IO.File.ReadAllLines(@"../../dataset/adhead.n26c100.max.bbk.max"));
            else
                return;
            if (grafo == null)
                throw new ArgumentNullException("grafo null");
            Console.WriteLine("grafo letto");
            Stopwatch watch = new();
            watch.Start();
            var res = ShortestAugmentingPath.FlowFordFulkerson(grafo);
            watch.Stop();
            Console.WriteLine($"Shortest augmenting path Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine("send flow = " + res);
        }
    }
} */