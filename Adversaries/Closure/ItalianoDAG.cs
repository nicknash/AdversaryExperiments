using System;
using System.Collections.Generic;
using System.IO;

namespace AdversaryExperiments.Adversaries.Closure
{
    // This maintains the transitive closure of a DAG under edge addition using the algorithm from
    // Italiano, G.F. "Amortized efficiency of a path retrieval data structure", 
    // Theoretical Computer Science, Volume 48, Issue 2-3Dec., 1986 pp 273–281
    //
    // For an n vertex graph, adding a new edge takes O(n) amortized time. Querying the existence of a path takes O(1) time.
    // O(n^2) space is required.
    //
    //
    // Italiano's amortized analysis using a potential function is pretty weird. Mainly because he uses a potential function
    // that is zero for the empty graph, and otherwise, negative!
    //
    // As a result, his amortized bound isn't an upper bound, because the final (positive) contribution of the final value of the potential function needs
    // to be bounded from above.
    //
    // For my sanity, here is a much simpler amortized analysis (more easily done on an iterative version of the algorithm than Italiano's recursive presentation). This is
    // a direct 'aggregate analysis' with the minute details spelled out:
    //
    // Let n be the number of vertices in the DAG. Consider a sequence of k calls to the 'AddEdge' function below. 
    // Any call to 'ExistsDirectedPath' takes O(1) time. So, in total over this sequence O(nk) time is spent 
    // making nk calls to 'CopySpanningTree'.
    //
    // 'CopySpanningTree' calls 'MeldIterative'. The outer loop of the latter function iterates exactly as many times
    // as items are added to '_worklist', but, except for the very first item, all items are added by the inner loop of this function.
    // Thus, the total time spent in inner loop iterations equals the total time spent in outer loop iterations.
    // Moreover, each outer-loop iteration adds exactly 1 edge to the transitive closure of the DAG. That is, 'CopySpanningTree' takes O(P) time to add
    // P edges to the transitive closure of the DAG.
    //
    // Let T be the total number of edges adding to the transitive closure by the nk calls made to 'CopySpanningTree'.
    //
    // Then the total time taken by the sequence of k calls to 'AddEdge' is:
    //
    // O(nk) + O(T) 
    //
    // Now It must be the case that T = O(min(n^2,k^2)), since adding k edges to the DAG can add at most k(k-1)/2 edges to the transitive closure, and the 
    // transitive closure can never have more edges than the complete graph on n vertices which has n(n-1)/2 edges.
    //
    // And so the total time taken by the sequence of k calls to 'AddEdge' is:
    //
    // O(nk) + O(T) = O(nk) + O(min(n^2, k^2))
    //
    // And so the amortized cost per call to 'AddEdge' (in excruciating detail) is:
    //
    //  1/k * (O(nk) + O(min(n^2,k^2))
    // = O(n) + O(min(n^2/k, k))
    // = O(n) + O(n)
    // = O(n)
    //
    public class ItalianoDAG // TODONICK: Write some tests for this.
    {
        // _index[i, j] = The spanning tree node for vertex number j in the spanning tree for vertex number i if j is a descendant of i
        //                otherwise, null
        //
        private readonly SpanningTreeNode[,] _index; 
        private readonly int _numVerts;
        private readonly Stack<(SpanningTreeNode, int)> _worklist = new();

        public ItalianoDAG(int numVerts)
        {
            _numVerts = numVerts;
            _index = new SpanningTreeNode[_numVerts, _numVerts];
            
            // Amusingly enough Italiano's paper omits this in the 'Initialize' 
            // procedure, meaning that the 'Add' operation always does nothing, because there
            // is no way an entry of _index can ever be non-null. Thankfully with this small fix
            // everything works :)
            for(int i = 0; i < _numVerts; ++i)
            {
                _index[i, i] = new SpanningTreeNode(i);
            }
        }

        // Count the number of edges that would be exist in the transitive closure of the DAG 
        // if the edge source -> target was added.
        //
        // This is the same algorithm as 'AddEdge' but it just counts the edges instead.
        public int CountClosureEdges(int source, int target)
        {
            if (ExistsDirectedPath(target, source))
            {
                throw new Exception($"Adding the edge {source} -> {target} would create a cycle");
            }
            if(ExistsDirectedPath(source, target))
            {
                return 0;
            }
            var numNewEdges = 0;
            for(int x = 0; x < _numVerts; ++x)
            {
                if(ExistsDirectedPath(x, source) && !ExistsDirectedPath(x, target))
                {
                    numNewEdges += CountMeld(x, target, target);
                }
            }
            return numNewEdges;
        }

        // Add the edge source -> target to the DAG, internally updating the transitive closure of the DAG
        public void AddEdge(int source, int target)
        {
            if (ExistsDirectedPath(target, source))
            {
                throw new Exception($"Adding the edge {source} -> {target} would create a cycle");
            }
            if(ExistsDirectedPath(source, target))
            {
                return;
            }

            for(int x = 0; x < _numVerts; ++x)
            {
                // The condition in this 'if' is true if 'x' is a vertex of this type
                //  
                // x----->source        target---->v
                //
                // That is, the addition of the edge source--->target gives rise to a new path
                // from x to target (and the descendants of target)
                if(ExistsDirectedPath(x, source) && !ExistsDirectedPath(x, target))
                {
                    CopySpanningTree(x, target, source);
                }
            }

        }

        // Copy the sub-spanning-tree of 'newDescendantVertex' rooted at 'immediateAncestorOfNewDescendantVertex' into the spanning-tree of 'vertexWithNewDescendants', but
        // exclude all nodes that are already in the spanning tree of 'vertexWithNewDescendants'
        private void CopySpanningTree(int vertexWithNewDescendants, int newDescendantVertex, int immediateAncestorOfNewDescendantVertex)
            => MeldIterative(vertexWithNewDescendants, newDescendantVertex, immediateAncestorOfNewDescendantVertex);/*Meld(vertexWithNewDescendants, 
                   newDescendantVertex, 
                   GetSpanningTreeNode(vertexWithNewDescendants, immediateAncestorOfNewDescendantVertex),
                   newDescendantVertex);*/

        private void Meld(int vertexWithNewDescendants, int newDescendantVertex, SpanningTreeNode parent, int newChildVertexNumber)
        {
            var newChild = new SpanningTreeNode(newChildVertexNumber);
            parent.AddChild(newChild);
            AddDescendant(vertexWithNewDescendants, newChild);

            var nodeInOtherTree = GetSpanningTreeNode(newDescendantVertex, newChild.VertexNumber);
            var children = nodeInOtherTree.Children;
            for (int i = 0; i < children.Count; ++i)
            {
                var child = children[i];
                if (!ExistsDirectedPath(vertexWithNewDescendants, child.VertexNumber))
                {
                    Meld(vertexWithNewDescendants, newDescendantVertex, newChild, child.VertexNumber);
                }
            }
        }
        
        private void MeldIterative(int vertexWithNewDescendants, int newDescendantVertex, int immediateAncestorOfNewDescendantVertex)
        {
            var root = GetSpanningTreeNode(vertexWithNewDescendants, immediateAncestorOfNewDescendantVertex);
            _worklist.Push((root, newDescendantVertex));
            while (_worklist.Count > 0)
            {
                var (parent, newChildVertexNumber) = _worklist.Pop();
                var newChild = new SpanningTreeNode(newChildVertexNumber);
                parent.AddChild(newChild);
                AddDescendant(vertexWithNewDescendants, newChild);
                var nodeInOtherTree = GetSpanningTreeNode(newDescendantVertex, newChild.VertexNumber);
                var children = nodeInOtherTree.Children;
                for (int i = 0; i < children.Count; ++i)
                {
                    var child = children[i];
                    if (!ExistsDirectedPath(vertexWithNewDescendants, child.VertexNumber))
                    {
                        _worklist.Push((nodeInOtherTree, child.VertexNumber));
                    }
                }
            }
        }
        
        private int CountMeld(int vertexWithNewDescendants, int newDescendantVertex, int newChildVertexNumber)
        {
            var nodeInOtherTree = GetSpanningTreeNode(newDescendantVertex, newChildVertexNumber);
            var children = nodeInOtherTree.Children;
            var numNewEdges = 1;
            for (int i = 0; i < children.Count; ++i)
            {
                var child = children[i];
                if (!ExistsDirectedPath(vertexWithNewDescendants, child.VertexNumber))
                {
                    numNewEdges += CountMeld(vertexWithNewDescendants, newDescendantVertex, child.VertexNumber);
                }
            }
            return numNewEdges;
        }


        public bool ExistsDirectedPath(int source, int target) => _index[source, target] != null;

        public void DumpAsDot(TextWriter output)
        {
            output.WriteLine($"digraph {nameof(ItalianoDAG)} {{");
            for(int i = 0; i < _numVerts; ++i)
            {
                output.WriteLine(i);
                for(int j = 0; j < _numVerts; ++j)
                {
                    if(i != j && ExistsDirectedPath(i, j))
                    {
                        output.WriteLine($"{i} -> {j}");
                    }
                }
            }
            output.WriteLine("}");
        }

        private void AddDescendant(int ancestor, SpanningTreeNode node) => _index[ancestor, node.VertexNumber] = node;
        
        private SpanningTreeNode GetSpanningTreeNode(int rootVertexNumber, int descendantVertexNumber) => _index[rootVertexNumber, descendantVertexNumber];
    }
}