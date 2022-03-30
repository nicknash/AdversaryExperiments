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
    public class BrodalAdversary
    {
        
        
        public int Compare(WrappedInt x, WrappedInt y)
        {
            return -1;
        }
    }
}