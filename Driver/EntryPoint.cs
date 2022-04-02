using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using AdversaryExperiments.Adversaries;
using System.Diagnostics;

namespace AdversaryExperiments.Driver
{
    class EntryPoint
    {
        static void Main(string[] args)
        {
            var cmdLine = new CommandLine();
            if (!cmdLine.TryParse(args, Console.Out))
            {
                cmdLine.PrintUsage(Console.Out);
                return;
            }

            for (int i = 0; i <= cmdLine.NumIncrements; ++i)
            {
                int dataSize = cmdLine.StartSize + i * cmdLine.SizeIncrement;
                var adversaries = new IAdversary[] { new BrodalAdversary(dataSize), new DAGAdversary(dataSize), new McIlroyKiller(dataSize) };
                var sw = new Stopwatch();
                foreach (var adv in adversaries)
                {
                    sw.Restart();
                    adv.CurrentData.Sort(adv.Compare);
                    //TreeSort(adv.CurrentData, adv);
                    Console.WriteLine($"{dataSize}, {adv.Name}, {adv.NumComparisons / (dataSize * Math.Log2(dataSize)):F5}, {adv.NumComparisons / (double) (dataSize * dataSize):F5}, {sw.Elapsed}");
                }
            }
        }

        private static void TreeSort(IReadOnlyList<WrappedInt> data, IComparer<WrappedInt> comparer)
        {
            var tree = new SortedDictionary<WrappedInt, bool>(comparer);
            foreach (var d in data)
            {
                tree.Add(d, true);
            }
        }
    }
}
