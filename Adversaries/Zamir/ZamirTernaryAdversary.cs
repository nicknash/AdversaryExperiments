using System;
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
                    if(!isXPaired && !isYPaired)
                    {

                    }
                    // Handle the pairing cases (can I devise a little algorithm that makes them
                    // a special case of the distinct node pairs cases?)

                }
                else
                {
                    var isAncestorPaired = xIsAncestor && isXPaired || yIsAncestor && isYPaired;
                    if(isAncestorPaired)
                    {
                        // In this case, we need to split a pair in an ancestor node.
                        // Since pairing only occurs in tree nodes and not intermediate nodes
                        // the ancestor with the pair must be a tree node.
                        // Splitting a pair correctly means that at most one element of the pair
                        // can reside at an intermediate node after this split. Otherwise, a subsequent move
                        // could send them both to a 'central' node. That is, consider the pair (p, q): 
                        //    (p,q)
                        //    /   \
                        //   I1    I2
                        //  /  \ /  \
                        // T1  T2   T3
                        //
                        // In this diagram 'T2' is a central node, and p and q could not be split to reside in I1 and I2,
                        // as a subsequent move could conceivably send them both to T2, which is inconsistent with p < q.
                        //
                        var result = xIsAncestor ? PushDownPair(xPair, y) : PushDownPair(yPair, x);
                        return result;
                    }
                    else
                    {
                        // These are the 'minimal push down' case of the ancestor
                        var result = xIsAncestor ? PushDownSingle(x, y) : OtherSense(PushDownSingle(y, x));
                        return result;
                    }
                }
            }
            //else
            {
                // TODO: The ordering is already defined, return it.

                return -1;
            }
        }

        private int PushDownPair((WrappedInt, WrappedInt) pairInAncestor, WrappedInt descendant)
        {
            var (p, q) = pairInAncestor;
            //    (p,q)
            //    /   \
            //   I1    I2
            //  /  \ /  \
            // T1  T2   T3
            //
            // The potential consistent destinations of p and q with respect to descendant are
            //
            // p in  | q in | description 
            // ------|------|
            //  I1   |  T3  | p left once, q right twice
            //  T1   |  T2  | p left twice, q right-then-left
            //  T1   |  I2  | p left twice, q right once
            //  T1   |  T3  | p left twice, q right twice
            //  T2   |  T3  | p left-right, q right twice
            //
            bool CanPushP(params Direction[] where) => CanPush(p, descendant, where);
            bool CanPushQ(params Direction[] where) => CanPush(q, descendant, where);

            if(CanPushP(Direction.Left) && CanPushQ(Direction.Right, Direction.Right))
            {
                Push(p, Direction.Left);
                Push(q, Direction.Right, Direction.Right);
            }
            else if(CanPushP(Direction.Left, Direction.Left) && CanPushQ(Direction.Right, Direction.Left))
            {
                Push(p, Direction.Left, Direction.Left);
                Push(q, Direction.Right, Direction.Left);
            }
            else if(CanPushP(Direction.Left, Direction.Left) && CanPushQ(Direction.Right))
            {
                Push(p, Direction.Left, Direction.Left);
                Push(q, Direction.Right, Direction.Right);
            }           
            else if(CanPushP(Direction.Left, Direction.Left) && CanPushQ(Direction.Right, Direction.Right))
            {
                Push(p, Direction.Left, Direction.Left);
                Push(q, Direction.Right, Direction.Right);
            }
            else if(CanPushP(Direction.Left, Direction.Right) && CanPushQ(Direction.Right, Direction.Right))
            {
                Push(p, Direction.Left, Direction.Right);
                Push(q, Direction.Right, Direction.Right);
            }
            throw new Exception($"Should never happen: Cannot resolve order when splitting ancestor pair");
        }

        private int PushDownSingle(WrappedInt ancestor, WrappedInt descendant)
        {
            bool CanPushAncestor(params Direction[] directions) => CanPush(ancestor, descendant, directions);
            void PushAncestor(params Direction[] directions) => Push(ancestor, directions);
            
            if(CanPushAncestor(Direction.Left))
            {
                PushAncestor(Direction.Left);
                return Less;
            }
            else if(CanPushAncestor(Direction.Right))
            {
                PushAncestor(Direction.Right);
                return Greater;
            }
            else
            {
                if(CanPushAncestor(Direction.Left, Direction.Left))
                {
                    PushAncestor(Direction.Left, Direction.Left);
                    return Less;
                }
                if(CanPushAncestor(Direction.Right, Direction.Right))
                {
                    PushAncestor(Direction.Right, Direction.Right);
                    return Greater;
                }
                throw new Exception($"Should never happen: Cannot resolve comparison of two unpaired elements in distinct nodes.");
            }
        }

        private void Push(WrappedInt n, params Direction[] directions) => throw new Exception($"Handle sentinels and update the mapping");

        private int OtherSense(int r)
         => r switch 
         {
             Greater => Less,
             Less => Greater,
             _ => throw new Exception($"Unexpected comparison result {r}")
         };

        private Node GetNode(WrappedInt v) => throw new Exception("TODO1");

        private bool TryGetPair(WrappedInt v, out (WrappedInt, WrappedInt) p) => throw new Exception("TODO2");

        private bool ExistsPath(WrappedInt x, WrappedInt y) => throw new Exception("TODO3");

        private bool ExistsPath(Node x, Node y) => throw new Exception($"TODO4");
 
        private bool Connected(Node x, Node y) => throw new Exception("TODO");

        private bool Ordered(Node x, Node y) => throw new Exception("TODO"); 

        private bool CanPushLeft(Node x, Node y) => !ExistsPath(x.Left, y) && !ExistsPath(y, x.Left);
        private bool CanPushRight(Node x, Node y) => throw new Exception($"TODO");

        private bool CanPushLeft(WrappedInt x, WrappedInt y) => CanPushLeft(GetNode(x), GetNode(y));

        private bool CanPushTwiceLeft(WrappedInt x, WrappedInt y) => CanPushLeft(GetNode(x).Left, GetNode(y));

        private bool CanPushRight(WrappedInt x, WrappedInt y) => throw new Exception($"TODO");

        private bool CanPushTwiceRight(WrappedInt x, WrappedInt y) => throw new Exception($"TODO");

        enum Direction
        {
            Left,
            Right
        }

        private bool CanPush(WrappedInt x, WrappedInt y, params Direction[] directions) => throw new Exception($"TODO");

    }
}