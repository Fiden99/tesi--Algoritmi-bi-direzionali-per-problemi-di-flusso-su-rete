using Monodirezionale.MaxFlow.LastLevelOpt;

using Xunit;

namespace Monodirezionale.MaxFlow.Tests;

public class LastLevelOptTests
{
    [Fact]
    public void TestBase()
    {
        SinkNode t = new SinkNode("t");
        Node n6 = new Node("6");
        n6.AddEdge(t, 10);
        Node n5 = new Node("5");
        n5.AddEdge((t, 35), (n6, 10));
        Node n4 = new Node("4");
        n4.AddEdge(n6, 25);
        Node n3 = new Node("3");
        n3.AddEdge((n4, 15), (n5, 15), (n6, 10));
        Node n2 = new Node("2");
        n2.AddEdge((n5, 35), (n3, 10));
        SourceNode s = new SourceNode("s");
        s.AddEdge((n2, 10), (n3, 30), (n4, 30));
        Graph graph = new Graph(s, n2, n3, n4, n5, n6, t);

        var res = BfsLastLevelOpt.FlowFordFulkerson(graph);

        Assert.Equal(35, res);
    }

    [Fact]
    public void Test1()
    {
        SinkNode t = new SinkNode("t");
        Node n9 = new Node("9");
        Node n8 = new Node("8");
        Node n7 = new Node("7");
        Node n6 = new Node("6");
        Node n5 = new Node("5");
        Node n4 = new Node("4");
        Node n3 = new Node("3");
        Node n2 = new Node("2");
        Node n1 = new Node("1");
        SourceNode s = new SourceNode("s");
        n9.AddEdge(t, 30);
        n8.AddEdge(t, 30);
        n7.AddEdge((n8, 25), (n9, 25));
        n6.AddEdge((n7, 30), (n9, 20));
        n5.AddEdge(n8, 10);
        n4.AddEdge(n7, 10);
        n3.AddEdge(n6, 40);
        n2.AddEdge(n6, 30);
        n1.AddEdge((n4, 5), (n5, 5));
        s.AddEdge((n1, 10), (n2, 20), (n3, 30));
        Graph graph = new Graph(s, n1, n2, n3, n4, n5, n6, n7, n8, n9, t);
        var res = BfsLastLevelOpt.FlowFordFulkerson(graph);
        Assert.Equal(60, res);
    }
    [Fact]
    public void Test2()
    {
        SinkNode t = new SinkNode("t");
        Node n9 = new Node("9");
        Node n8 = new Node("8");
        Node n7 = new Node("7");
        Node n6 = new Node("6");
        Node n5 = new Node("5");
        Node n4 = new Node("4");
        Node n3 = new Node("3");
        Node n2 = new Node("2");
        Node n1 = new Node("1");
        SourceNode s = new SourceNode("s");
        n9.AddEdge(t, 50);
        n8.AddEdge(t, 40);
        n7.AddEdge(n9, 60);
        n6.AddEdge((n8, 20), (n9, 10));
        n5.AddEdge(n8, 10);
        n4.AddEdge((n5, 20), (n6, 30), (n7, 50));
        n3.AddEdge((n4, 10), (n7, 10));
        n2.AddEdge(n4, 10);
        n1.AddEdge((n4, 10), (n5, 10));
        s.AddEdge((n1, 30), (n2, 30), (n3, 30));
        Graph graph = new Graph(s, n1, n2, n3, n4, n5, n6, n7, n8, n9, t);
        var res = BfsLastLevelOpt.FlowFordFulkerson(graph);
        Assert.Equal(50, res);
    }
    [Fact]
    public void Test3()
    {
        SinkNode t = new SinkNode("t");
        Node n9 = new Node("9");
        Node n8 = new Node("8");
        Node n7 = new Node("7");
        Node n6 = new Node("6");
        Node n5 = new Node("5");
        Node n4 = new Node("4");
        Node n3 = new Node("3");
        Node n2 = new Node("2");
        Node n1 = new Node("1");
        SourceNode s = new SourceNode("s");
        n9.AddEdge(t, 60);
        n8.AddEdge((n9, 20), (t, 50));
        n7.AddEdge((n8, 40), (n9, 10));
        n6.AddEdge(n8, 30);
        n5.AddEdge((n6, 50), (n7, 60));
        n4.AddEdge((n6, 30), (n7, 40));
        n3.AddEdge(n5, 20);
        n2.AddEdge(n5, 20);
        n1.AddEdge((n4, 20), (n6, 20));
        s.AddEdge((n1, 10), (n2, 10), (n3, 10));
        Graph graph = new Graph(s, n1, n2, n3, n4, n5, n6, n7, n8, n9, t);
        var res = BfsLastLevelOpt.FlowFordFulkerson(graph);
        Assert.Equal(30, res);
    }
    [Fact]
    public void Test4()
    {
        SinkNode t = new SinkNode("t");
        Node n9 = new Node("9");
        Node n8 = new Node("8");
        Node n7 = new Node("7");
        Node n6 = new Node("6");
        Node n5 = new Node("5");
        Node n4 = new Node("4");
        Node n3 = new Node("3");
        Node n2 = new Node("2");
        Node n1 = new Node("1");
        SourceNode s = new SourceNode("s");
        n9.AddEdge(t, 30);
        n8.AddEdge(t, 20);
        n7.AddEdge(t, 30);
        n6.AddEdge(n3, 20);
        n5.AddEdge((n8, 15), (n9, 15));
        n4.AddEdge(n5, 40);
        n3.AddEdge((n5, 20), (n7, 40));
        n2.AddEdge((n3, 20), (n4, 10), (n6, 10));
        n1.AddEdge((n2, 30), (n6, 40));
        s.AddEdge((n1, 50), (n4, 20));
        Graph graph = new Graph(s, n1, n2, n3, n4, n5, n6, n7, n8, n9, t);
        var res = BfsLastLevelOpt.FlowFordFulkerson(graph);
        Assert.Equal(60, res);
    }

    [Fact]
    public void Test5()
    {
        SinkNode t = new SinkNode("t");
        Node n17 = new Node("17");
        Node n16 = new Node("16");
        Node n15 = new Node("15");
        Node n14 = new Node("14");
        Node n13 = new Node("13");
        Node n12 = new Node("12");
        Node n11 = new Node("11");
        Node n10 = new Node("10");
        Node n9 = new Node("9");
        Node n8 = new Node("8");
        Node n7 = new Node("7");
        Node n6 = new Node("6");
        Node n5 = new Node("5");
        Node n4 = new Node("4");
        Node n3 = new Node("3");
        Node n2 = new Node("2");
        Node n1 = new Node("1");
        SourceNode s = new SourceNode("s");
        s.AddEdge((n1, 20), (n2, 30), (n3, 30));
        n1.AddEdge((n5, 5), (n6, 10));
        n2.AddEdge((n1, 5), (n3, 5), (n5, 15));
        n3.AddEdge((n4, 30), (n5, 10));
        n4.AddEdge((n7, 25), (n8, 20));
        n5.AddEdge(n7, 10);
        n6.AddEdge((n7, 15), (n9, 10));
        n7.AddEdge((n8, 30), (n10, 10));
        n8.AddEdge(n10, 15);
        n9.AddEdge((n7, 5), (n10, 15));
        n10.AddEdge((n11, 30), (n12, 40), (n13, 25), (n14, 20));
        n11.AddEdge(n15, 5);
        n12.AddEdge(n15, 20);
        n13.AddEdge(n16, 25);
        n14.AddEdge(n16, 30);
        n15.AddEdge(t, 50);
        n16.AddEdge(t, 50);
        t.AddEdge(n17, 100);
        n17.AddEdge(n14, 100);
        Graph graph = new Graph(s, n1, n2, n3, n4, n5, n6, n7, n8, n9, n10, n11, n12, n13, n14, n15, n16, n17, t);

        var res = BfsLastLevelOpt.FlowFordFulkerson(graph);
        Assert.Equal(35, res);
    }
}