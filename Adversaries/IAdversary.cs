using System.Collections.Generic;

namespace AdversaryExperiments.Adversaries
{
    public interface IAdversary : IComparer<WrappedInt>
    {
        string Name { get; }
        long NumComparisons { get; }
        List<WrappedInt> CurrentData { get; }
    }
}