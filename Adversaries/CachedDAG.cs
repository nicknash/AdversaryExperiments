using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdversaryExperiments.Adversaries
{
    public class CachedDAG : IDAG
    {
        private List<HashSet<int>> connectedTo;
        private List<int> numDescendants;

        private List<int> vertexEpochs;
        private int currentEpoch;
        public int NumVerts { get; private set; }

        public CachedDAG(int numVerts)
        {
            NumVerts = numVerts;
            numDescendants = new List<int>(Enumerable.Repeat(0, numVerts));
            connectedTo = new List<HashSet<int>>(Enumerable.Range(0, numVerts).Select(i => new HashSet<int>()));
            currentEpoch = 0;
            vertexEpochs = new List<int>(Enumerable.Repeat(currentEpoch, numVerts));
        }

        public void AddEdge(int source, int target)
        {
            connectedTo[source].Add(target);
        }

        public bool ExistsDirectedPath(int source, int target)
        {
            var worklist = new Stack<int>(connectedTo[source]);
            ++currentEpoch;
            bool exists = false;
            int numDescs = 0;
            while (worklist.Count != 0)
            {
                int v = worklist.Pop();
                vertexEpochs[v] = currentEpoch;
                ++numDescs;
                if (v == target)
                {
                    exists = true;
                }
                PushUnvisitedConnected(worklist, v);
            }
            numDescendants[source] = numDescs;
            return exists;
        }

        private void PushUnvisitedConnected(Stack<int> worklist, int u)
        {
            // The slightly nicer (i.e. more declarative) Linq for this loop
            // is about twice as slow. This is used in the inner loop
            // of searching routines, hence the minor optimization.
            foreach (int v in connectedTo[u])
            {
                if (vertexEpochs[v] != currentEpoch)
                {
                    worklist.Push(v);
                }
            }
        }

        public int CountDescendants(int source)
        {
            return numDescendants[source];
        }
    }
}
