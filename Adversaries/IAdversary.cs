using System.Collections.Generic;

namespace AdversaryExperiments.Adversaries
{
    public interface IAdversary : IComparer<WrappedInt>
    {
        string Name { get; }
        long NumComparisons { get; }
        IReadOnlyList<WrappedInt> CurrentData { get; }
    }
}