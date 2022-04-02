using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdversaryExperiments.Adversaries
{
    public class DAGAdversary : IAdversary
    {
        private readonly IDAG _dag;
        private readonly int _length;

        public string Name { get; }
        public IReadOnlyList<WrappedInt> CurrentData { get; }
        public long NumComparisons { get; private set; }

        public DAGAdversary(int length) : this(new CachedDAG(length))
        {
        }

        public DAGAdversary(IDAG initial)
        {
            _length = initial.NumVerts;            
            _dag = initial;
            CurrentData = new List<WrappedInt>(Enumerable.Range(0, _length).Select(i => new WrappedInt { Value = i }));
        }

        public int Compare(WrappedInt x, WrappedInt y)
        {
            ++NumComparisons;
            if (x.Value == y.Value)
            {
                return 0;
            }
            if (_dag.ExistsDirectedPath(x.Value, y.Value))
            {
                return -1;
            }
            if (_dag.ExistsDirectedPath(y.Value, x.Value))
            {
                return 1;
            }
            if (_dag.CountDescendants(x.Value) < _dag.CountDescendants(y.Value))
            {
                _dag.AddEdge(y.Value, x.Value);
                return 1;
            }
            _dag.AddEdge(x.Value, y.Value);
            return -1;
        }
    }
}
