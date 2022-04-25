namespace AdversaryExperiments.Adversaries.Zamir
{
    //       T 
    //      / \
    //     I  I
    //   /  \/ \
    //   T   T  T
    // T: tree node
    // I: intermediate node
    class Node
    {
        public enum NodeType
        {
            Sentinel,
            Tree,
            Intermediate
        }
        
        public Node Left { get; private set; }
        public Node Right { get; private set; }
        
        public NodeType Type { get; private set; }

        protected Node(Node left, Node right, NodeType type)
        {
            Left = left;
            Right = right;
            Type = type;
        }

        public static Node CreateUnit()
        {
            var (child01, child12) = CreateIntermediateNodes();

            var root = new Node(child01, child12, NodeType.Tree);
            return root;
        }

        public void EnsureInitialized()
        {
            if(Type != NodeType.Sentinel)
            {
                return;
            }
            Type = NodeType.Tree;
            (Left, Right) = CreateIntermediateNodes();
        }

        private static (Node, Node) CreateIntermediateNodes()
        {
            var child0 = CreateFinalLevel();
            var child1 = CreateFinalLevel();
            var child2 = CreateFinalLevel();

            var child01 = new Node(child0, child1, NodeType.Intermediate);
            var child12 = new Node(child1, child2, NodeType.Intermediate);
            return (child01, child12);
        }

        private static Node CreateFinalLevel() => new(CreateSentinel(), CreateSentinel(), NodeType.Tree);
        private static Node CreateSentinel() => new(null, null, NodeType.Sentinel);
    }
}