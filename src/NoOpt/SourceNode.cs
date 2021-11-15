using System.Linq;

namespace NoOpt
{
    public class SourceNode : Node
    {

        //non può avere valid = false
        public SourceNode(string name) : base(name) 
        {
            this.inFlow = int.MaxValue-this.next.Select(x =>x.capacity).Sum();
        }

        public SourceNode(string name, params MonoEdge[] edge) : base(name,edge) 
        {
            this.inFlow = int.MaxValue-this.next.Select(x =>x.capacity).Sum();

        }
        public SourceNode(string name, params (Node,int)[] edge) : base(name, edge) 
        {
            this.inFlow = int.MaxValue-this.next.Select(x =>x.capacity).Sum();

        }
    }
}