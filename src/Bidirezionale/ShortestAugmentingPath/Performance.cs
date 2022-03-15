/* using System;
using System.Diagnostics;
using System.Linq;
using Bidirezionale.ShortestAugmentingPath;

namespace ShortestAugmetingPath
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

                for (int i = 3; i < int.Parse(str[2]); i++)
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
                if (String.Equals(s[2], "s"))
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
                    Node fNO = grafo.Nodes.Single(m => String.Equals(x[1], m.Name));
                    Node tNO = grafo.Nodes.Single(m => String.Equals(x[2], m.Name));
                    fNO.AddEdge(tNO, int.Parse(x[3]));
                }
            }
            return grafo;
        }

        public static void Main()
        {
            var grafo = Read(System.IO.File.ReadAllLines(@"C:\Users\Filippo\Desktop\tesi\src\dataset\adhead.n6c10.max.bbk.max"));
            Performance(grafo);
        }
        public static void Main(int args)
        {
            Graph grafo;
            if (args == 1)
                grafo = Read(System.IO.File.ReadAllLines(@"C:\Users\Filippo\Desktop\tesi\src\dataset\adhead.n6c10.max.bbk.max"));
            else if (args == 2)
                grafo = Read(System.IO.File.ReadAllLines(@"C:\Users\Filippo\Desktop\tesi\src\dataset\adhead.n26c10.max.bbk.max"));
            else if (args == 3)
                grafo = Read(System.IO.File.ReadAllLines(@"C:\Users\Filippo\Desktop\tesi\src\dataset\adhead.n6c100.max.bbk.max"));
            else if (args == 4)
                grafo = Read(System.IO.File.ReadAllLines(@"C:\Users\Filippo\Desktop\tesi\src\dataset\adhead.n26c100.max.bbk.max"));
            else
                return;
            Performance(grafo);

        }


        public static void Performance(Graph grafo)
        {

            var watch = new Stopwatch();
            watch.Start();
            var res4 = ShortestAugmentingPath.FlowFordFulkerson(grafo);
            watch.Stop();
            Console.WriteLine($"Bidirectional Shortest Augmentign Path Execution Time: {watch.ElapsedMilliseconds} ms");
            Console.WriteLine("send flow = " + res4);
        }
    }
}
 */