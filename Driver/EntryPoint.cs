using System;
using System.Collections.Generic;
using AdversaryExperiments.Adversaries;
using System.Diagnostics;
using AdversaryExperiments.Adversaries.Brodal;
using AdversaryExperiments.Adversaries.Descendants;

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
                // (InsertionSort, "InsertionSort"), // Desc adversary insanely slow for this 
                (TreeSort, "TreeSort")
            };
            for (int i = 0; i <= cmdLine.NumIncrements; ++i)
            {
                int dataSize = cmdLine.StartSize + i * cmdLine.SizeIncrement;
                var sw = new Stopwatch();
                foreach (var s in sorts)
                {
                    var adversaries = new IAdversary[] { new RandomAdversary(dataSize), new BrodalAdversary(dataSize), new ZamirTernaryAdversary(dataSize), new DescendantsAdversary(dataSize), new McIlroyKiller(dataSize) };
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

        private static void InsertionSort(IAdversary adversary)
        {
            var minIdx = 0;
            var data = adversary.CurrentData;
            var compare = adversary.Compare;
            var n = adversary.CurrentData.Count;
            for(int i = 1; i < n; ++i)
            {
                if(compare(data[i], data[minIdx]) < 0)
                {
                    minIdx = i;
                }
            }
            var tmp = data[0];
            data[0] = data[minIdx];
            data[minIdx] = tmp;

            for(int j = 2; j < n; ++j)
            {
                var here = data[j];
                int k = j;
                while(compare(here, data[k - 1]) < 0)
                {
                    data[k] = data[k - 1];
                    --k;
                }
                data[k] = here;
            }
        }
    }
}
