namespace AdversaryExperiments.Tools
{
    public class DAGUtils
    {
        public static IReadOnlyList<Edge> GetEdges(IReadOnlyList<(int, int)> edges) => edges.Select(e => new Edge(e.Item1, e.Item2)).ToList();

        public static IReadOnlyList<IReadOnlyList<int>> GetVertexToEdgesOut(IReadOnlyList<Edge> edges)
        {
            var numVerts = edges.Select(e => Math.Max(e.Source, e.Target)).Max() + 1;
            var vertexToEdgesOut = Enumerable.Range(0, numVerts).Select(_ => new List<int>()).ToList();
            foreach(var e in edges)
            {
                vertexToEdgesOut[e.Source].Add(e.Target);
            }
            return vertexToEdgesOut;
        }
    }

}