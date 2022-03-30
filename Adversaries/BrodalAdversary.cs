namespace AdversaryExperiments.Adversaries
{
    // This is Brodal et al.'s adversary from "The Randomized Complexity of Maintaining the Minimum",
    // Nordic Journal of Computing 3(4):337-351, 1996
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
    // increases the depth of at most two elements of P in T by 1, and so each comparison contributes at most 2 to D. Thus, D <= 2*C. 
    // Finally note that in any binary tree of n leaves, D >= nlog2(n). Thus C >= 0.5*nlog2(n)
    //
    // Note that this adversary has an equivalent formulation in terms of intervals on the real-line.
    public class BrodalAdversary
    {
        public int Compare(WrappedInt x, WrappedInt y)
        {
            return -1;
        }
    }
}