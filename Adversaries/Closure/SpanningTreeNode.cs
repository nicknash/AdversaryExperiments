using System.Collections.Generic;

namespace AdversaryExperiments.Adversaries.Closure
{
    class SpanningTreeNode
    {
        private readonly List<SpanningTreeNode> _children;

        public readonly int VertexNumber;

        public IReadOnlyList<SpanningTreeNode> Children;
        public SpanningTreeNode(int vertexNumber)
        {
            VertexNumber = vertexNumber;
            _children = new List<SpanningTreeNode>();
            Children = _children;
        }

        public void AddChild(SpanningTreeNode child)
        {
            _children.Add(child);
        }
    }
}