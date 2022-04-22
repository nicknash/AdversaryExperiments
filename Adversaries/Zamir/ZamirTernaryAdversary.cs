using System.Collections.Generic;
using System.Linq;

namespace AdversaryExperiments.Adversaries.Zamir
{
    public class ZamirTernaryAdversary : IAdversary
    {
        private const int Equal = 0;
        private const int Less = -1;
        private const int Greater = 1;
   
        public string Name { get; }
        public long NumComparisons { get; private set; }
        public List<WrappedInt> CurrentData { get; }

        private Node _root;

        public ZamirTernaryAdversary(int size)
        {
            Name = "Zamir3";
            CurrentData = new List<WrappedInt>(Enumerable.Range(0, size).Select(i => new WrappedInt { Value = i }));
            _root = Node.CreateUnit();
        }
        
        public int Compare(WrappedInt x, WrappedInt y)
        {
            ++NumComparisons;
            if(x == y)
            {
                return Equal;
            }

            var xIsAncestor = ExistsPath(x, y);
            var yIsAncestor = ExistsPath(y, x);

            var inSameNode = xIsAncestor && yIsAncestor;
            var requiresClassification = xIsAncestor || yIsAncestor;

            var isXPaired = TryGetPair(x, out var xPair);
            var isYPaired = TryGetPair(y, out var yPair);


            if (requiresClassification)
            {
                if (inSameNode)
                {
                    // Handle the pairing cases (can I devise a little algorithm that makes them
                    // a special case of the distinct node pairs cases?)

                }
                else
                {
                    if (!isXPaired && !isYPaired)
                    {
                        // These are the 'minimal push down' case of the ancestor
                    }
                    else
                    {
                        // Somewhat complicated cases I need to go through here.
                    }
                }
            }
            else
            {
                // TODO: The ordering is already defined, return it.

                return -1;
            }
        }

        private Node GetNode(WrappedInt v) => throw new System.Exception("TODO1");

        private bool TryGetPair(WrappedInt v, out WrappedInt w) => throw new System.Exception("TODO2");

        private bool ExistsPath(WrappedInt x, WrappedInt y) => throw new System.Exception("TODO3");
    }
}