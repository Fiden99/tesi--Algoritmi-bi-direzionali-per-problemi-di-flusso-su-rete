using System;

namespace Bidirezionale.ShortestAugmentingPath
{
    public class SinkNode : Node
    {
        public SinkNode(string name) : base(name)
        {
            this.SinkDistance = 0;
        }
    }
}