using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdversaryExperiments.Adversaries
{
    public class McIlroyKiller : IAdversary
    {
        private readonly int _gas;        
        
        private int _numSolid;
        private WrappedInt _candidatePivot;

        public string Name { get; }
        public long NumComparisons { get; private set; }
        public List<WrappedInt> CurrentData { get; }

        public McIlroyKiller(int length)
        {
            Name = "McIlroy";
            _gas = length;
            IEnumerable<WrappedInt> allGas = Enumerable.Repeat(_gas, length).Select(i => new WrappedInt() { Value = i });
            CurrentData = new List<WrappedInt>(allGas);
            _numSolid = 0;
            NumComparisons = 0;
        }
        
        public int Compare(WrappedInt x, WrappedInt y)
        {
            ++NumComparisons;
            MaybeDoFreezing(x, y);
            if (IsGas(x))
            {
                _candidatePivot = x;
            }
            else if (IsGas(y))
            {
                _candidatePivot = y;
            }
            return x.Value - y.Value;
        }

        private void MaybeDoFreezing(WrappedInt x, WrappedInt y)
        {
            if (IsGas(x) && IsGas(y))
            {
                if (_candidatePivot == x)
                {
                    Freeze(x);
                }
                else
                {
                    Freeze(y);
                }
            }
        }

        private void Freeze(WrappedInt wi)
        {
            wi.Value = _numSolid;
            ++_numSolid;
        }

        private bool IsGas(WrappedInt wi)
        {
            return wi.Value == _gas;
        }
    }
}
