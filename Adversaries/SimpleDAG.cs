using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdversaryExperiments.Adversaries
{
    public class SimpleDAG
    {
        private List<HashSet<int>> connectedTo;
        private List<int> vertexEpochs;
        private int currentEpoch;
        public int NumVerts { get; private set; }

        public SimpleDAG(int numVerts)
        {
            NumVerts = numVerts;
            connectedTo = new List<HashSet<int>>(Enumerable.Range(0, numVerts).Select(i => new HashSet<int>()));
            currentEpoch = 0;
            vertexEpochs = new List<int>(Enumerable.Repeat(currentEpoch, numVerts));            
        }

        public void AddEdge(int v1, int v2)
        {
            connectedTo[v1].Add(v2);
        }

        public bool ExistsDirectedPath(int source, int target)
        {
            var worklist = new Stack<int>(connectedTo[source]);
            ++currentEpoch;
            while (worklist.Count != 0)
            {
                int v = worklist.Pop();
                if (v == target)
                {
                    return true;
                }
                PushUnvisitedConnected(worklist, v);
            }
            return false;
        }

        private void PushUnvisitedConnected(Stack<int> worklist, int u)
        {
            var unvisitedConnected = connectedTo[u].Where(v => vertexEpochs[v] != currentEpoch);
            foreach (int v in unvisitedConnected)
            {
                vertexEpochs[v] = currentEpoch;
                worklist.Push(v);
            }
        }

        public int NumDescendants(int v)
        {
            int result = 0;
            var worklist = new Stack<int>(connectedTo[v]);
            ++currentEpoch;
            while (worklist.Count != 0)
            {
                int u = worklist.Pop();
                ++result;
                PushUnvisitedConnected(worklist, u);
            }
            return result;
        }
    }
}
