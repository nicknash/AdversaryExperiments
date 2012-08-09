using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdversaryExperiments.Adversaries;

namespace AdversaryExperiments.Driver
{
    class EntryPoint
    {
        static void Main(string[] args)
        {
            var killer = new McIlroyKiller(10000);

            List<WrappedInt> data = killer.CurrentData;

            data.Sort(killer.Compare);

            Console.WriteLine("numComparisons = {0}", killer.NumComparisons);

            Console.ReadKey();
        }
    }
}
