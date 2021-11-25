using BFS.LastLevelOpt;
using BFS.SickPropagation;
using Xunit;

namespace BFS.Tests;

public class SickPropagationTests
{
    [Fact]
    public void Execute()
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
        SickPropagation.Graph graph = new SickPropagation.Graph(s, n2, n3, n4, n5, n6, t);

        var res = BfsSickPropagation.FlowFordFulkerson(graph);

        Assert.Equal(35, res);
    }
}