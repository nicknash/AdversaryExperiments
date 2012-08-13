using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdversaryExperiments.Adversaries
{
    public class DAGAdversary
    {
        private IDAG dag;
        private int length;
        public List<WrappedInt> CurrentData { get; private set; }
        public int NumComparisons { get; private set; }

        public DAGAdversary(int length)
        {
            this.length = length;
            dag = new CachedDAG(length);
            InitData();
        }

        public DAGAdversary(IDAG initial)
        {
            length = initial.NumVerts;            
            dag = initial;
            InitData();
        }

        private void InitData()
        {
            CurrentData = new List<WrappedInt>(Enumerable.Range(0, length).Select(i => new WrappedInt { Value = i }));         
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
            if (dag.CountDescendants(x.Value) < dag.CountDescendants(y.Value))
            {
                dag.AddEdge(y.Value, x.Value);
                return 1;
            }
            dag.AddEdge(x.Value, y.Value);
            return -1;
        }
    }
}
