using System.Linq;

namespace BFS.LastLevelOpt
{
    public class SourceNode : Node
    {
        public SourceNode(string name) : base(name)
        {
            this.setInFlow(int.MaxValue - this.edges.Select(x => x.capacity).Sum());
        }
    }
}