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
        

        public ItalianoDAG(int numVerts)
        {

        }

        // Count the number of edges that would be exist in the transitive closure of the DAG 
        // if the edge source -> target was added.
        public int CountClosureEdges(int source, int target)
        {
            return -1;
        }

        // Add the edge source -> target to the DAG, internally updating the transitive closure of the DAG
        public void AddEdge(int source, int target)
        {

        }

        public bool ExistsDirectedPath(int source, int target)
        {
            throw new System.Exception("TODO");
        }
    }
}