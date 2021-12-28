namespace BFS.ShortestAugmentingPath
{
    public class SinkNode : Node
    {
        public SinkNode(string name) : base(name)
        {
            this.Distance = 0;
        }
    }
}