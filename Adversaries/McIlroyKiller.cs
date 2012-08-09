using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdversaryExperiments.Adversaries
{
    public class McIlroyKiller
    {
        private readonly int length;
        private readonly int gas;        
        
        private int numSolid;
        private WrappedInt candidatePivot;

        public int NumComparisons { get; private set; }
        public List<WrappedInt> CurrentData { get; private set; }

        public McIlroyKiller(int length)
        {
            this.length = length;
            gas = length;

            Reset();
        }
        
        public void Reset()
        {           
            IEnumerable<WrappedInt> allGas = Enumerable.Repeat<int>(gas, length).Select(i => new WrappedInt() { Value = i });
            CurrentData = new List<WrappedInt>(allGas);
            numSolid = 0;
            NumComparisons = 0;
            return;
        }
        
        public int Compare(WrappedInt x, WrappedInt y)
        {
            ++NumComparisons;
            MaybeDoFreezing(x, y);
            if (IsGas(x))
            {
                candidatePivot = x;
            }
            else if (IsGas(y))
            {
                candidatePivot = y;
            }
            return x.Value - y.Value;
        }

        private void MaybeDoFreezing(WrappedInt x, WrappedInt y)
        {
            if (IsGas(x) && IsGas(y))
            {
                if (candidatePivot == x)
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
            wi.Value = numSolid;
            ++numSolid;
        }

        private bool IsGas(WrappedInt wi)
        {
            return wi.Value == gas;
        }
    }
}
