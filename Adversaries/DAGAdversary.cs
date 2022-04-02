﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdversaryExperiments.Adversaries
{
    public class DAGAdversary : IAdversary
    {
        private readonly IDAG _dag;

        public string Name { get; }
        public List<WrappedInt> CurrentData { get; }
        public long NumComparisons { get; private set; }

        public DAGAdversary(int length) : this(new CachedDAG(length))
        {
        }

        public DAGAdversary(IDAG initial)
        {
            var length = initial.NumVerts;            
            _dag = initial;
            Name = "DAG";
            CurrentData = new List<WrappedInt>(Enumerable.Range(0, length).Select(i => new WrappedInt { Value = i }));
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
