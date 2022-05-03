using System.Collections.Generic;

namespace AdversaryExperiments.Adversaries.Closure
{
    class ClosureAdversary : IAdversary
    {
        private ItalianoDAG _dag;

        public string Name => throw new System.NotImplementedException();

        public long NumComparisons => throw new System.NotImplementedException();

        public List<WrappedInt> CurrentData => throw new System.NotImplementedException();

        public int Compare(WrappedInt x, WrappedInt y)
        {
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
            if(_dag.CountClosureEdges(x.Value, y.Value) < _dag.CountClosureEdges(x.Value, y.Value))
            {
                _dag.AddEdge(x.Value, y.Value);
                return -1;
            }
            else
            {
                _dag.AddEdge(x.Value, y.Value);
                return 1;
            }
        }
    }
}