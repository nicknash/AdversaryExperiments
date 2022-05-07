using System.Collections.Generic;
using System.Linq;

namespace AdversaryExperiments.Adversaries.Closure
{
    public class ClosureAdversary : IAdversary
    {
        private readonly ItalianoDAG _dag;

        public string Name {get; }

        public long NumComparisons { get; private set; }

        public List<WrappedInt> CurrentData { get; }

        public ClosureAdversary(int size)
        {
            Name = "Closure";
            CurrentData = new List<WrappedInt>(Enumerable.Range(0, size).Select(i => new WrappedInt { Value = i }));
            _dag = new ItalianoDAG(size);
        }

        public int Compare(WrappedInt x, WrappedInt y)
        {
            ++NumComparisons;
            if(x == y)
            {
                return 0;
            }

            if(_dag.ExistsDirectedPath(x.Value, y.Value))
            {
                return -1;
            }
            else if(_dag.ExistsDirectedPath(y.Value, x.Value))
            {
                return 1;
            }
            if(_dag.CountClosureEdges(x.Value, y.Value) < _dag.CountClosureEdges(y.Value, x.Value))
            {
                _dag.AddEdge(x.Value, y.Value);
                return -1;
            }
            else
            {
                _dag.AddEdge(y.Value, x.Value);
                return 1;
            }
        }
    }
}