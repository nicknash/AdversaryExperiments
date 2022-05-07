using System.IO;

namespace AdversaryExperiments.Adversaries.Closure
{
    // This maintains the transitive closure of a DAG under edge addition using the algorithm from
    // Italiano, G.F. "Amortized efficiency of a path retrieval data structure", 
    // Theoretical Computer Science, Volume 48, Issue 2-3Dec., 1986 pp 273–281
    //
    // For an n vertex graph, adding a new edge takes O(n) amortized time. Querying the existence of a path takes O(1) time.
    // O(n^2) space is required.
    public class ItalianoDAG
    {
        // _index[i, j] = The spanning tree node for vertex number j in the spanning tree for vertex number i if j is a descendant of i
        //                otherwise, null
        //
        private readonly SpanningTreeNode[,] _index; 
        private readonly int _numVerts;

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
           => Meld(vertexWithNewDescendants, 
                   newDescendantVertex, 
                   GetSpanningTreeNode(vertexWithNewDescendants, immediateAncestorOfNewDescendantVertex),
                   newDescendantVertex);

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

        private SpanningTreeNode GetSpanningTreeRoot(int vertexNumber) => _index[vertexNumber, vertexNumber];

        private SpanningTreeNode GetSpanningTreeNode(int rootVertexNumber, int descendantVertexNumber) => _index[rootVertexNumber, descendantVertexNumber];
    }
}