
namespace Monodirezionale.MaxFlow.NoOpt
{
    public class SourceNode : Node
    {

        public SourceNode(string name) : base(name)
        {
            this.InFlow = int.MaxValue;
        }
        /*
        public SourceNode(string name, params MonoEdge[] edge) : base(name, edge) { }
        public SourceNode(string name, params (Node, int)[] edge) : base(name, edge) { }
        */
    }
}