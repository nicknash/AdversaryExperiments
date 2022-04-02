namespace AdversaryExperiments.Adversaries.Descendants
{
    public interface IDAG
    {
        int NumVerts { get; }

        void AddEdge(int source, int target);
        bool ExistsDirectedPath(int source, int target);
        int CountDescendants(int source);
    }
}
