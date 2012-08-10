using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdversaryExperiments.Adversaries
{
    public class DAGAdversary
    {
        private SimpleDAG dag;
        private int length;
        public List<WrappedInt> CurrentData { get; private set; }
        public int NumComparisons { get; private set; }

        public DAGAdversary(int length)
        {
            this.length = length;
            Reset();
        }

        public DAGAdversary(SimpleDAG initial)
        {
            length = initial.NumVerts;
            Reset();
            dag = initial;
        }

        private void Reset()
        {
            NumComparisons = 0;
            CurrentData = new List<WrappedInt>(Enumerable.Range(0, length).Select(i => new WrappedInt { Value = i }));
            dag = new SimpleDAG(length);
        }

        public int Compare(WrappedInt x, WrappedInt y)
        {
            ++NumComparisons;
            if (x.Value == y.Value)
            {
                return 0;
            }
            if (dag.ExistsDirectedPath(x.Value, y.Value))
            {
                return -1;
            }
            if (dag.ExistsDirectedPath(y.Value, x.Value))
            {
                return 1;
            }

            if (dag.NumDescendants(x.Value) < dag.NumDescendants(y.Value))
            {
                dag.AddEdge(y.Value, x.Value);
                return 1;
            }
            dag.AddEdge(x.Value, y.Value);
            return -1;
        }
    }
}
