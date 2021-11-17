namespace BFS.NoOpt
{
    public class SinkNode : Node
    {
        public SinkNode(string name) : base(name) { }

        public SinkNode(string name, params MonoEdge[] edge) : base(name, edge) { }

        public SinkNode(string name, params (Node, int)[] edge) : base(name, edge) { }

    }
}