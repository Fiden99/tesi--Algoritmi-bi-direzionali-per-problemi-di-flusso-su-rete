using System;

namespace Bidirezionale.ShortestAugmentingPath
{
    public class SourceNode : Node
    {
        public SourceNode(string name) : base(name)
        {
            this.SourceDistance = 0;
        }
    }
}