using System;
using System.Collections.Generic;

namespace AdversaryExperiments.Adversaries.Zamir
{
    class OrderedTraversal
    {
        // Traverse a ternary lattice 'in order'
        // The algorithm applies recursively to a 'unit' of the lattice:
        //
        //      A
        //    /   \
        //   B     C
        // /  \  /   \
        // D    E     F
        //
        // Then the visiting order is DBEACF
        // That is, it is an in-order traversal that avoid repetition (of the sub-tree rooted at 'E' reachable from both 'B' and 'C').
        // 
        // If it holds that the elements in the partial order we are trying to determine the relative order of are such that:
        //
        // 1. They are not in the same node.
        // 2. They are not in an ancestor-descendant relationship.
        // 3. They are not in intermediate node siblings (i.e., B and C above)
        //
        // Then this traversal can be used to define their ordering. 
        public static IEnumerable<Node> Traverse(Node root)
        {
            var pending = new Stack<Node>();
            pending.Push(root);
            var parent = root;
            var here = root.Left;
            while(true)
            {
                switch(here.Type)
                {
                    case Node.NodeType.Intermediate: 
                        var isLeftIntermediate = parent.Left == here;
                        if(isLeftIntermediate) 
                        {
                            pending.Push(here);
                            parent = here;
                            here = here.Left;
                        }
                        else
                        {
                            yield return here;
                            here = here.Right;
                        }
                        break;
                    case Node.NodeType.Tree:
                        pending.Push(here);
                        parent = here;
                        here = here.Left;
                        break;
                    case Node.NodeType.Sentinel:
                        if(pending.Count == 0)
                        {
                            yield break;
                        }
                        parent = pending.Pop();
                        yield return parent;
                        here = parent.Right;
                        break;
                    default:
                        throw new Exception($"Unrecognised {nameof(Node.NodeType)}: {here.Type}");
                }
            }
        }
    }
}