using System;
using System.Diagnostics;
using System.Linq;
using Bidirezionale.NodePropagation.SickPropagation;
using Xunit;

namespace NodePropagation.Tests
{
    public class ReadGraph
    {
        private static Graph Read(string[] stringhe)
        { 
            Graph grafo;
            var str = stringhe[0].Split(" ");
            if(String.Equals(str[0],"p"))
            {
                grafo = new(int.Parse(str[2]));
                for(int i = 3; i<Int64.Parse(str[2]);i++)
                    grafo.AddNode(new Node(i.ToString()),false);
            }
            else 
                return null;
            var s = stringhe[2].Split(" ");
            if (string.Equals(s[0],"n"))
                if (string.Equals(s[2],"s"))
                    grafo.AddNode(new SourceNode(s[1]),false);
                else if (String.Equals(s[2], "t"))
                    grafo.AddNode(new SinkNode(s[2]),true);
            s = stringhe[3].Split(" ");
            if (String.Equals(s[0], "n"))
                if (String.Equals(s[2], "s"))
                    grafo.AddNode(new SourceNode(s[1]),false);
                else if (String.Equals(s[2], "t"))
                    grafo.AddNode(new SinkNode(s[1]),true);
            foreach(var line in stringhe)
            {
                var x = line.Split(" ");
                if(String.Equals(x[0],"a") && int.Parse(x[3])!= 0)
                    {
                        Node f = grafo.LabeledNodeSourceSide[0].Single(m => String.Equals(x[1],m.Name));
                        Node t = grafo.LabeledNodeSourceSide[0].SingleOrDefault(m => String.Equals(x[2],m.Name));
                        if(t == null)
                            t = grafo.LabeledNodeSinkSide[0].Single(m => String.Equals(x[2], m.Name));
                        f.AddEdge(t,int.Parse(x[3]));
                    }
            }       
            return grafo;
        }
        public static Graph ReadGraph1()
        {
            var g = Read(System.IO.File.ReadAllLines(@"C:\Users\Filippo\Desktop\tesi\src\dataset\adhead.n6c10.max.bbk.max"));
            return g;
        }
        /*         public static Graph ReadGraph2()
                {

                }
                public static Graph ReadGraph3()
                {

                }
                public static Graph ReadGraph4()
                {

                }
         */
        [Fact]
        public void Test1()
        {
            var graph = ReadGraph1();
            if (graph is null)
                throw new InvalidOperationException("graph = null");
            Stopwatch watch = new();
            Console.WriteLine("grafo letto");
            watch.Start();
            var res = BiNodePropagationSickPropagation.FlowFordFulkerson(graph);
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
            Assert.Equal(0, res);

        }

    }
}