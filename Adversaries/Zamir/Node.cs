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

        public static Node CreateUnit()
        {
            var child0 = CreateFinalLevel();
            var child1 = CreateFinalLevel();
            var child2 = CreateFinalLevel();

            var child01 = new Node { Left = child0, Right = child1, Type = NodeType.Intermediate};
            var child12 = new Node { Left = child1, Right = child2, Type = NodeType.Intermediate};

            var root = new Node
            {
                Left = child01,
                Right = child12
            };

            return root;
        }

        private static Node CreateFinalLevel() => new() { Type = NodeType.Tree, Left = CreateSentinel(), Right = CreateSentinel() };
        private static Node CreateSentinel() => new() { Type = NodeType.Sentinel };
        
    }
}