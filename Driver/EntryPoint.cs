using System;
using System.Collections.Generic;
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
                var killer = new DAGAdversary(dataSize);
                var sw = new Stopwatch();
                sw.Start();
                killer.CurrentData.Sort(killer.Compare);
                Console.WriteLine("{0} {1} {2}", dataSize, killer.NumComparisons, sw.Elapsed);
            }
        }
    }
}
