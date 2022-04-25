﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AdversaryExperiments.Adversaries.Zamir
{
    public class ZamirTernaryAdversary : IAdversary
    {
        private const int Equal = 0;
        private const int Less = -1;
        private const int Greater = 1;
        
        enum Direction
        {
            Left,
            Right
        }


        private readonly Node[] _elementToNode;

        public string Name { get; }
        public long NumComparisons { get; private set; }
        public List<WrappedInt> CurrentData { get; }

        private Node _root;

        public ZamirTernaryAdversary(int size)
        {
            Name = "Zamir3";
            CurrentData = new List<WrappedInt>(Enumerable.Range(0, size).Select(i => new WrappedInt { Value = i }));
            _root = Node.CreateUnit();
            _elementToNode = Enumerable.Range(0 , size).Select(_ => _root).ToArray();
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
                    var node = GetNode(x);
                    if(!isXPaired && !isYPaired)
                    {
                        switch(node.Type)
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
                        return OtherSense(ComparePairToSingleton(yPair, y, x));
                    }
                    else if (isXPaired && !isYPaired)
                    {
                        return ComparePairToSingleton(xPair, x, y);
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
            throw new NotImplementedException($"The ordering between {x.Value} and {y.Value} is already defined, but this is not implemented yet.");
        }

        private int ComparePairToSingleton((WrappedInt, WrappedInt) pair, WrappedInt x, WrappedInt y)
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
            var d = GetDestination(x, directions);
            var yNode = GetNode(y);
            return !ExistsPath(d, yNode) && !ExistsPath(yNode, d);
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

        private bool TryGetPair(WrappedInt v, out (WrappedInt, WrappedInt) p) => throw new Exception("TODO2");

        private void CreatePair(WrappedInt x, WrappedInt y)
        {
            throw new NotImplementedException();
        }

        private void DestroyPair((WrappedInt, WrappedInt) pair)
        {
            throw new NotImplementedException();
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