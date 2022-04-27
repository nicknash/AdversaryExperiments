using System;
using System.Collections.Generic;
using System.Linq;

namespace AdversaryExperiments.Adversaries.Zamir
{
    public class ZamirTernaryAdversary : IAdversary
    {
        public const int Equal = 0;
        public const int Less = -1;
        public const int Greater = 1;
        
        private enum Direction
        {
            Left,
            Right
        }


        private readonly Node[] _elementToNode;
        private readonly (WrappedInt, WrappedInt)[] _elementToPair;


        public string Name { get; }
        public long NumComparisons { get; private set; }
        public List<WrappedInt> CurrentData { get; }

        private readonly Node _root;

        public ZamirTernaryAdversary(int size)
        {
            Name = "Zamir3";
            CurrentData = new List<WrappedInt>(Enumerable.Range(0, size).Select(i => new WrappedInt { Value = i }));
            _root = Node.CreateUnit();
            _elementToNode = Enumerable.Range(0, size).Select(_ => _root).ToArray();
            _elementToPair = new (WrappedInt, WrappedInt)[size];
        }
        
        public int Compare(WrappedInt x, WrappedInt y)
        {
            ++NumComparisons;
            if(x == y)
            {
                return Equal;
            }

            var inPair = TryGetPair(x, out var pair);
            if(inPair)
            {
                if(y == pair.Item2)
                {
                    return Less;
                }
                else if(y == pair.Item1)
                {
                    return Greater;
                }
            }

            var xIsAncestor = ExistsPath(x, y);
            var yIsAncestor = ExistsPath(y, x);

            var inSameNode = xIsAncestor && yIsAncestor;
            var xNode = GetNode(x);
            var yNode = GetNode(y);
            var areLeftRightIntermediateSiblings = xNode.Right == yNode.Left;
            var areRightLeftIntermediateSiblings = xNode.Left == yNode.Right;

            if (xIsAncestor || yIsAncestor)
            {
                return DoAncestorClassifyingComparison(x, y, xIsAncestor, yIsAncestor, inSameNode);
            }
            else if(areLeftRightIntermediateSiblings)
            {
                PushApart(x, y);
                return Less;
            }
            else if(areRightLeftIntermediateSiblings)
            {
                PushApart(y, x);
                return Greater; 
            }
            foreach(var node in OrderedTraversal.Traverse(_root))
            {
                if(node == xNode)
                {
                    return Less;
                }
                if(node == yNode)
                {
                    return Greater;
                }
            }
            throw new Exception($"Should never happen: Couldn't determine order.");
        }


        private int DoAncestorClassifyingComparison(WrappedInt x, WrappedInt y, bool xIsAncestor, bool yIsAncestor, bool inSameNode)
        {
            var isXPaired = TryGetPair(x, out var xPair);
            var isYPaired = TryGetPair(y, out var yPair);
            if (inSameNode)
            {
                var node = GetNode(x);
                if (!isXPaired && !isYPaired)
                {
                    switch (node.Type)
                    {
                        case Node.NodeType.Tree:
                            CreatePair(x, y);
                            return Less;
                        case Node.NodeType.Intermediate:
                            Push(x, Direction.Left);
                            Push(y, Direction.Right);
                            return Less;
                        default:
                            throw new Exception($"Unexpected {nameof(Node.NodeType)} {node.Type}");
                    }
                }
                else if (!isXPaired && isYPaired)
                {
                    return OtherSense(ComparePairToSingletonInSameNode(yPair, y, x));
                }
                else if (isXPaired && !isYPaired)
                {
                    return ComparePairToSingletonInSameNode(xPair, x, y);
                }
                else
                {
                    DestroyPair(xPair);
                    DestroyPair(yPair);
                    var (a, b) = xPair;
                    var (c, d) = yPair;
                    if (x == a)
                    {
                        Push(a, Direction.Left, Direction.Left);
                        Push(b, Direction.Right);
                        Push(c, Direction.Left, Direction.Right);
                        Push(d, Direction.Right, Direction.Right);
                        return Less;
                    }
                    else if (x == b && y == c)
                    {
                        Push(a, Direction.Left, Direction.Right);
                        Push(b, Direction.Right, Direction.Right);
                        Push(c, Direction.Left, Direction.Left);
                        Push(d, Direction.Right);
                        return Less;
                    }
                    else if (x == b && y == d)
                    {
                        // This is a not exactly symmetrical case that (right now) it looks like Zamir et al don't cover
                        Push(a, Direction.Left, Direction.Left);
                        Push(b, Direction.Left, Direction.Right);
                        Push(c, Direction.Left);
                        Push(d, Direction.Right, Direction.Right);
                        return Less;
                    }
                }
                throw new Exception($"Should never happen: Did not resolve same node comparison.");
            }
            else
            {
                var isAncestorPaired = xIsAncestor && isXPaired || yIsAncestor && isYPaired;
                if (isAncestorPaired)
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
                    // because subsequent moves could conceivably send them both to T2, which is inconsistent with p < q.
                    //
                    var result = xIsAncestor ? PushDownPair(xPair, y) : PushDownPair(yPair, x);
                    return result;
                }
                else
                {
                    // In this case, the comparison is between two unpaired elements in distinct nodes.
                    var result = xIsAncestor ? PushDownSingle(x, y) : OtherSense(PushDownSingle(y, x));
                    return result;
                }
            }
        }

        private void PushApart(WrappedInt a, WrappedInt b)
        {
            Push(a, Direction.Left);
            Push(b, Direction.Right);
        }

        private int ComparePairToSingletonInSameNode((WrappedInt, WrappedInt) pair, WrappedInt x, WrappedInt y)
        {
            DestroyPair(pair);
            var (p, q) = pair;
            if (x == p)
            {
                Push(p, Direction.Left, Direction.Left);
                Push(q, Direction.Right);
                Push(y, Direction.Right);
                return Less;
            }
            else if (x == q)
            {
                Push(p, Direction.Left);
                Push(q, Direction.Right, Direction.Right);
                Push(y, Direction.Left);
                return Greater;
            }
            throw new Exception($"Should never happen: exhaustiveness check on pairing");
        }

        private int PushDownPair((WrappedInt, WrappedInt) pairInAncestor, WrappedInt descendant)
        {
            DestroyPair(pairInAncestor);
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
                Push(q, Direction.Right);
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

        private void Push(WrappedInt n, params Direction[] directions)
        {
            var node = GetDestination(n, directions);
            _elementToNode[n.Value] = node;
        }

        private bool CanPush(WrappedInt x, WrappedInt y, params Direction[] directions) 
        {
            var destinationNode = GetDestination(x, directions);
            var yNode = GetNode(y);
            var areSiblingIntermediateNodes = destinationNode.Right == yNode.Left || yNode.Right == destinationNode.Left;
            return !areSiblingIntermediateNodes && !ExistsPath(destinationNode, yNode) && !ExistsPath(yNode, destinationNode);
        }

        private Node GetDestination(WrappedInt n, Direction[] directions)
        {
            var node = GetNode(n);
            foreach (var d in directions)
            {
                node.EnsureInitialized();
                switch (d)
                {
                    case Direction.Left:
                        node = node.Left;
                        break;
                    case Direction.Right:
                        node = node.Right;
                        break;
                    default:
                        throw new Exception($"Unrecognised {nameof(Direction)}: {d}");
                }
            }

            return node;
        }

        private Node GetNode(WrappedInt v) => _elementToNode[v.Value];

        private bool ExistsPath(WrappedInt here, WrappedInt target) => ExistsPath(GetNode(here), GetNode(target));

        private bool ExistsPath(Node here, Node target)
        {
            if(here.Type == Node.NodeType.Sentinel)
            {
                return false;
            }
            if(here == target)
            {
                return true;
            }
            return ExistsPath(here.Left, target) || ExistsPath(here.Right, target);
        }

        private bool TryGetPair(WrappedInt v, out (WrappedInt, WrappedInt) p) 
        {
            p = _elementToPair[v.Value];
            return p != (null, null);
        }

        private void CreatePair(WrappedInt first, WrappedInt second)
        {
            if(_elementToPair[first.Value] != (null, null))
            {
                throw new Exception($"Cannot create pair: pair already exists for {first.Value}");
            }
            if(_elementToPair[second.Value] != (null, null))
            {
                throw new Exception($"Cannot create pair: pair already exists for {second.Value}");
            }
            _elementToPair[first.Value] = (first, second);
            _elementToPair[second.Value] = (first, second);
        }

        private void DestroyPair((WrappedInt, WrappedInt) pair)
        {
            var (x, y) = (pair.Item1.Value, pair.Item2.Value);
            if(_elementToPair[x] == (null, null))
            {
                throw new Exception($"Cannot destroy pair: pair does not exist for {x}");
            }
            if(_elementToPair[y] == (null, null))
            {
                throw new Exception($"Cannot destroy pair: pair does not exist for {y}");
            }
            _elementToPair[x] = _elementToPair[y] = (null, null);
        }
        
        private static int OtherSense(int r)
         => r switch 
         {
             Greater => Less,
             Less => Greater,
             _ => throw new Exception($"Unexpected comparison result {r}")
         };
    }
}