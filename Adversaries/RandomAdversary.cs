using System;
using System.Collections.Generic;
using System.Linq;

namespace AdversaryExperiments.Adversaries
{
    public class RandomAdversary : IAdversary
    {
        public string Name { get; }
        public long NumComparisons { get; private set; }
        public List<WrappedInt> CurrentData { get; }

        public RandomAdversary(int size)
        {
            Name = "Random";
            var r = new Random();
            CurrentData = Enumerable.Range(0, size).Select(i => new WrappedInt { Value = i}).ToList();
            for (int i = 1; i < CurrentData.Count; ++i)
            {
                var tmp = CurrentData[i];
                var j = r.Next(i);
                CurrentData[i] = CurrentData[j];
                CurrentData[j] = tmp;
            }
        }

        public int Compare(WrappedInt x, WrappedInt y)
        {
            ++NumComparisons;
            return x.Value - y.Value;
        }
    }
}