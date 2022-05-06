namespace AdversaryExperiments.Adversaries.Closure
{
    // This maintains the transitive closure of a DAG under edge addition using the algorithm from
    // Italiano, G.F. "Amortized efficiency of a path retrieval data structure", 
    // Theoretical Computer Science, Volume 48, Issue 2-3Dec., 1986 pp 273–281
    //
    // For an n vertex graph, adding a new edge takes O(n) amortized time. Querying the existence of a path takes O(1) time.
    // O(n^2) space is required.
    class ItalianoDAG
    {
        // _index[i, j] = The node for vertex number j in the spanning tree for vertex number i if j is a descendant of i
        //                otherwise, null
        private readonly SpanningTreeNode[,] _index; 
        private readonly int _numVerts;

        public ItalianoDAG(int numVerts)
        {
            _index = new SpanningTreeNode[numVerts, numVerts];
            _numVerts = numVerts;
        }

        // Count the number of edges that would be exist in the transitive closure of the DAG 
        // if the edge source -> target was added.
        public int CountClosureEdges(int source, int target)
        {
            if(ExistsDirectedPath(source, target))
            {
                return 0;
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
                    Meld(x, target, source, target);
                }
            }
            return -1;
        }

        private void Meld(int nodeWithNewDescendants, int newlyReachableNode, int newNodeRoot, int newlyReachableNodeRoot)
        {

        }


        // Add the edge source -> target to the DAG, internally updating the transitive closure of the DAG
        public void AddEdge(int source, int target)
        {

        }

        public bool ExistsDirectedPath(int source, int target) => _index[source, target] != null;
    }

    class SpanningTreeNode
    {

    }
}