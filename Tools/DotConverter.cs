namespace AdversaryExperiments.Tools
{
    public class DotConverter
    {
        public static void ToDotDigraph(string graphName, IReadOnlyList<Edge> dag, TextWriter output)
        {
            output.WriteLine($"digraph {graphName} {{");
            foreach(var edge in dag)
            {
                output.WriteLine($"{edge.Source} -> {edge.Target};");
            }
            output.WriteLine("}");
        }

        public static void ToDotDigraph(string graphName, IReadOnlyList<Edge> dag, string filePath)
        {
            var output = new StreamWriter(filePath);
            ToDotDigraph(graphName, dag, output);
            output.Close();
        }
    }
}