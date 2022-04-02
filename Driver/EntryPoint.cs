using System;
using System.Collections.Generic;
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

            var sorts = new (Action<IAdversary>, string)[]
            {
                (adv => adv.CurrentData.Sort(adv.Compare), "List.Sort"), 
                (TreeSort, "TreeSort")
            };
            for (int i = 0; i <= cmdLine.NumIncrements; ++i)
            {
                int dataSize = cmdLine.StartSize + i * cmdLine.SizeIncrement;
                var sw = new Stopwatch();
                foreach (var s in sorts)
                {
                    var adversaries = new IAdversary[] { new RandomAdversary(dataSize), new BrodalAdversary(dataSize), new DAGAdversary(dataSize), new McIlroyKiller(dataSize) };
                    foreach (var adv in adversaries)
                    {
                        sw.Restart();
                        s.Item1(adv);
                        Console.WriteLine($"{dataSize}, {s.Item2}, {adv.Name}, {adv.NumComparisons / (dataSize * Math.Log2(dataSize)):F5}, {adv.NumComparisons / (double)(dataSize * dataSize):F5}, {sw.Elapsed}");
                    }
                }
            }
        }

        private static void TreeSort(IAdversary adversary)
        {
            var tree = new SortedSet<WrappedInt>(adversary);
            foreach (var d in adversary.CurrentData)
            {
                tree.Add(d);
            }
        }
    }
}
