using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdversaryExperiments.Adversaries
{
    public interface IDAG
    {
        int NumVerts { get; }

        void AddEdge(int source, int target);
        bool ExistsDirectedPath(int source, int target);
        int CountDescendants(int source);
    }
}
