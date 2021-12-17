using System.Linq;

namespace BFS.NoOpt
{
    public class SourceNode : Node
    {

        public SourceNode(string name) : base(name) { }
        /*
        public SourceNode(string name, params MonoEdge[] edge) : base(name, edge) { }
        public SourceNode(string name, params (Node, int)[] edge) : base(name, edge) { }
        */
    }
}