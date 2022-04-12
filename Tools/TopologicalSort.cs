using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdversaryExperiments.Tools
{
    public class TopologicalSort
    {
        readonly record struct Choice(int VertexNumber, IReadOnlyList<int> EndPointsOfDeletedEdges);
        

        public static void GetAllSorts(IReadOnlyList<IReadOnlyList<int>> vertexToEdgesOut)
        {
            int numVertices = vertexToEdgesOut.Count;
            var vertexToNumEdgesIn = new int[numVertices];
            
            for(int i = 0; i < numVertices; ++i)
            {
                foreach(var v in vertexToEdgesOut[i])
                {
                    ++vertexToNumEdgesIn[v];
                }
            }
            
   
            var isProcessed = new bool[numVertices];
            var pendingVertices = new Stack<List<int>>();
            var initialVertices = GetVerticesWithNoEdgesIn(vertexToNumEdgesIn, isProcessed);
            pendingVertices.Push(initialVertices);
            var thisSort = new List<Choice>();

            int numSorts = 0;
            while(pendingVertices.Count != 0) 
            {
                //var stack = String.Join(",", pendingVertices.Select(list => $"[{String.Join(',', list)}]"));

                //Console.WriteLine($"STACK is {stack}");
                var possibleVertices = pendingVertices.Peek();
                if(possibleVertices.Count == 0)
                {
                    pendingVertices.Pop();
                    if (thisSort.Count > 0)
                    {
                        var lastChoice = thisSort[thisSort.Count - 1];
                        foreach (var v in lastChoice.EndPointsOfDeletedEdges)
                        {
                            ++vertexToNumEdgesIn[v];
                        }
                        thisSort.RemoveAt(thisSort.Count - 1);
                        isProcessed[lastChoice.VertexNumber] = false;
                    }
                   continue; 
                }
                
                var vertex = possibleVertices[possibleVertices.Count - 1];
                possibleVertices.RemoveAt(possibleVertices.Count - 1);
       
                var choice = new Choice(vertex, vertexToEdgesOut[vertex]);
                thisSort.Add(choice);
                isProcessed[vertex] = true;
                var edgesOut = vertexToEdgesOut[vertex];
                foreach(var v in edgesOut)
                {
                    --vertexToNumEdgesIn[v];
                }
                if(thisSort.Count == numVertices)
                {
                    var sortStr = String.Join(",", thisSort.Select(c => c.VertexNumber.ToString()));
                    ++numSorts;
                    Console.WriteLine($"TOPOLOGICAL SORT [{numSorts}]: {sortStr}");
                    var lastChoice = thisSort[thisSort.Count - 1];
                    foreach(var v in lastChoice.EndPointsOfDeletedEdges)
                    {
                        ++vertexToNumEdgesIn[v];
                    }
                    thisSort.RemoveAt(thisSort.Count - 1);
                    isProcessed[lastChoice.VertexNumber] = false;
                }
                else
                {
                    var possibleVerticesFromHere = GetVerticesWithNoEdgesIn(vertexToNumEdgesIn, isProcessed);
                    if(possibleVerticesFromHere.Count > 0)
                    {
                        pendingVertices.Push(possibleVerticesFromHere);  
                    }
                }
            }
        }

        private static List<int> GetVerticesWithNoEdgesIn(IReadOnlyList<int> vertexToNumEdgesIn, IReadOnlyList<bool> isProcessed)
        {
            var result = new List<int>();
            for(int vertexNumber = 0; vertexNumber < vertexToNumEdgesIn.Count; ++vertexNumber)
            {
                if(!isProcessed[vertexNumber] && vertexToNumEdgesIn[vertexNumber] == 0)
                {
                    result.Add(vertexNumber);
                }
            }
            return result;
        }

    }

}