namespace AdversaryExperiments.Tools
{
    public class TransitiveClosure
    {
        public static IReadOnlyList<Edge> GetTransitiveClosure(IReadOnlyList<IReadOnlyList<int>> vertexToEdgesOut)
        {
            var numVertices = vertexToEdgesOut.Count;
            var haveFoundPath = new bool[numVertices, numVertices];
            var edgesInClosure = new List<Edge>();
            for(int vertexNumber = 0; vertexNumber < numVertices; ++vertexNumber)
            {
                var pendingVertices = new Stack<int>(vertexToEdgesOut[vertexNumber]);
                while(pendingVertices.Count > 0)
                {
                    var here = pendingVertices.Pop();
                    edgesInClosure.Add(new Edge(vertexNumber, here));
                    haveFoundPath[vertexNumber, here] = true;
                    foreach (var newVertex in vertexToEdgesOut[here])
                    {
                        if (!haveFoundPath[vertexNumber, newVertex])
                        {
                            pendingVertices.Push(newVertex);
                        }
                    }
                }
            }
            return edgesInClosure;
        } 
    }

}