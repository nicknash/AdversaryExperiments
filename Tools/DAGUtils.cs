namespace AdversaryExperiments.Tools
{
    public class DAGUtils
    {
        public static IReadOnlyList<Edge> GetEdges(IReadOnlyList<(int, int)> edges) => edges.Select(e => new Edge(e.Item1, e.Item2)).ToList();

        public static IReadOnlyList<IReadOnlyList<int>> GetVertexToEdgesOut(IReadOnlyList<Edge> edges)
        {
            var numVerts = GetNumVertices(edges);
            var vertexToEdgesOut = Enumerable.Range(0, numVerts).Select(_ => new List<int>()).ToList();
            foreach(var e in edges)
            {
                vertexToEdgesOut[e.Source].Add(e.Target);
            }
            return vertexToEdgesOut;
        }

        // This function assumes the vertex numbers are contiguous
        public static int GetNumVertices(IReadOnlyList<Edge> edges) => edges.Select(e => Math.Max(e.Source, e.Target)).Max() + 1;
    }

}