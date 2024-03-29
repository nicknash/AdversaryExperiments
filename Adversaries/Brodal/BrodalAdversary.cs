﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AdversaryExperiments.Adversaries.Brodal
{
    // This is Brodal et al.'s adversary from "The Randomized Complexity of Maintaining the Minimum",
    // Nordic Journal of Computing 3(4):337-351, 1996
    //
    // In fact, it appears this adversary has been discovered independently quite a few times. It is described
    // by Borodin, Guibas, Lynch and Yao at the end of "Efficient Searching Using Partial Ordering", 
    // Information Processing Letters, 12:(2),pp71-75, 1980
    //
    // It was also discovered independently in Richards and Vaidya 
    // "On the Distribution of Comparisons in Sorting Algorithms" BIT(28),pp764-774, 1988
    // where they cite an unpublished 1979 manuscript of Lynch (of the preceding IPL paper).
    //
    //
    // The description given by Brodal et al. represents the partial order, P, via a function V that maps
    // elements of the partial order to nodes in a binary tree, T
    //
    // Given x, y elements of P, then x < y in P iff:
    //
    //   (i) V(x) appears before V(y) in the in-order traversal of T.
    //   (ii) V(x) is not a descendant of V(y) and V(y) is not a descendant of V(x)
    //
    // Initially, V(p) = root-node for all elements p of P
    //
    // The adversary is then an algorithm for producing a new function V' from V in response to a comparison
    // of two elements of P.
    // 
    // In response to a comparison of elements x, y of P (i.e. is x < y?) the adversary behaves as follows:
    //
    // (i)   Answers according to the partial order if x < y in P (i.e., according to the two rules above)
    // (ii)  If V(x) = V(y) = n, then V' is identical to V except V'(x) = left-child(n) and V'(y) = right-child(n), and the adversary answers 'yes'.
    // (iii) If x is an ancestor of y, then V' is identical to V except V'(x) is the child of V(x) that is not an ancestor of y. If this child is the left-child
    //       the adversary answers 'yes', and otherwise it answers 'no'.
    // (iv)  Symmetrical to (iii), where y is an ancestor of x.
    //
    // Now consider sorting against this adversary. When a total ordering of the elements of P has been established, then each of the n elements of P must reside
    // in a leaf-node of the associated binary tree T. Let D be the sum of the depths of all the leaves of T. Each comparison (invocation of the adversary)
    // increases the depth of at most two elements of P in T by 1, and so each comparison contributes at most 2 to D. Thus, D <= 2*C, where C is the number of comparisons performed. 
    // Finally note that in any binary tree of n leaves, D <= nlog2(n) (this fact is easily seen be considering any binary tree as embedded within a complete binary tree).
    // Thus C >= 0.5*nlog2(n)
    //
    //
    // Note that this adversary has an equivalent formulation in terms of intervals on the real-line.
    // The correspondence is as follows: Each node, n, in the binary tree T has an associated interval (a, b). The interval for left-child(n) is (a, m) and the interval
    // for right-child(n) is (m, b) where m = (a+b)/2. 
    //
    // As an aside, this implies the interval associated with any node of the binary tree is (0.b_1b_2..b_n, 1 + 0.b_1b_2..b_n) where 0.b_1_b2..b_n is obtained by reading
    // the sequence of left and right turns down the binary tree as 0 or 1 respectively. Or, said another way, every interval is of the form (j/2^d, (j+1)/2^d) for some nonnegative integers
    // j, d
    // 
    // Operationally, a function I that maps elements of the partial order to open intervals of (0, 1) is maintained by the adversary.
    //
    // Initially I(p) = (0, 1) for all elements of P. Given x, y, elements of P then x < y in P iff I(x) = (a, b) and I(y) = (c, d) and b < c.
    // A comparison of the form 'x < y' is the answered according to the existing mapping I, or a new mapping is produced replacing one of the intervals end-points with
    // its mid-point. 
    // 
    // The implementation below is a reasonably efficient 'one-pass' realization of the above. Where a single in-order traversal of the tree is made where either the
    // result of the comparison is resolved, and the tree is updated if necessary. It could be made much more efficient by eliminating the tree traversal by associating
    // the index in the in-order traversal of an element's associated tree node, where it is known for both elements in the comparison.
    public class BrodalAdversary : IAdversary
    {
        private const int Equal = 0;
        private const int Less = -1;
        private const int Greater = 1;
        
        private readonly Node _root;
        private readonly Node[] _elementToNode;
        private readonly Stack<Node> _pending;

        public string Name { get; }
        public List<WrappedInt> CurrentData { get; }
        public long NumComparisons { get; private set; }
        
        public BrodalAdversary(int length)
        {
            Name = "Brodal";
            CurrentData = new List<WrappedInt>(Enumerable.Range(0, length).Select(i => new WrappedInt { Value = i }));
            _root = new Node(false);
            _elementToNode = Enumerable.Range(0, length).Select(_ => _root).ToArray();
            _pending = new Stack<Node>(length);
        }

        public int Compare(WrappedInt x, WrappedInt y)
        {
            ++NumComparisons;
            if (x == y)
            {
                return Equal;
            }
            var xNode = GetNode(x);
            var yNode = GetNode(y);
            if (xNode == yNode)
            {
                PushLeft(x);
                PushRight(y);
                return Less;
            }
            _pending.Clear();
            _pending.Push(_root);
            while (_pending.Count > 0)
            {
                var here = _pending.Peek();
                var stateHere = here.GetState(NumComparisons);
                switch (stateHere)
                {
                    case Node.VisitState.Unvisited:
                        _pending.Push(here.Left);
                        here.SetState(Node.VisitState.VisitingLeft, NumComparisons);
                        break;
                    case Node.VisitState.VisitingLeft:
                        _pending.Push(here.Right);
                        here.SetState(Node.VisitState.VisitingRight, NumComparisons);
                        break;
                    case Node.VisitState.VisitingRight:
                        here.SetState(Node.VisitState.Complete, NumComparisons);
                        break;
                    case Node.VisitState.Complete:
                        _pending.Pop();
                        var hereIsXNode = here == xNode;
                        if (!hereIsXNode && here != yNode)
                        {
                            break;
                        }
                        var otherElement = hereIsXNode ? y : x;
                        var other = GetNode(otherElement);
                        var stateOther = other.GetState(NumComparisons);
                        switch (stateOther)
                        {
                            case Node.VisitState.Unvisited:
                                // 'other' is unvisited but 'here' is complete, 'here' is less than it (if here == xNode)
                                return hereIsXNode ? Less : Greater;
                            case Node.VisitState.VisitingLeft:
                                // 'here' is in the left subtree of 'other', move 'other' to its own right child so it's
                                // no longer an ancestor.
                                // Hence, 'here' is now less than 'other' (if here == xNode)
                                PushRight(otherElement);
                                return hereIsXNode ? Less : Greater;
                            case Node.VisitState.VisitingRight:
                                // 'here' is in the right subtree of 'other', move 'other' to its left child so it's
                                // no longer an ancestor.
                                // Hence, 'other' is now less than 'here' (if here == xNode)
                                PushLeft(otherElement);
                                return hereIsXNode ? Greater : Less;
                            case Node.VisitState.Complete:
                                // 'other' is completely visited before 'here', i.e. 'other' is less than here (if here == xNode)
                                return hereIsXNode ? Greater : Less;
                            default:
                                throw new Exception($"Unrecognised visit state {stateOther}");
                        }
                    default:
                        throw new Exception($"Unrecognised visit state {stateHere}");
                }
            }
            throw new Exception($"Unable to determine ordering of {x.Value} and {y.Value}");
        }
        
        private Node GetNode(WrappedInt x) => _elementToNode[x.Value];

        private void PushDown(WrappedInt v, Node where)
        {
            where.EnsureInitialized();
            _elementToNode[v.Value] = where;
        }
        
        private void PushLeft(WrappedInt v)
        {
            var current = GetNode(v);
            PushDown(v, current.Left);
        }
        
        private void PushRight(WrappedInt v)
        {
            var current = GetNode(v);
            PushDown(v, current.Right);
        }
    }
}