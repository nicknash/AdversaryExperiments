namespace AdversaryExperiments.Tools
{
    public class DotConverter
    {
        public static void ToDotDigraph(IReadOnlyList<Edge> dag, TextWriter output)
        {
            output.WriteLine("digraph G {");
            foreach(var edge in dag)
            {
                output.WriteLine($"{edge.Source} -> {edge.Target};");
            }
            output.WriteLine("}");
        }
    }
}