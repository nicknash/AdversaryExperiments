using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;
using System.IO;

namespace AdversaryExperiments.Driver
{
    class CommandLine
    {
        OptionSet options;
        public int StartSize { get; private set; }
        public int SizeIncrement { get; private set; }
        public int NumIncrements { get; private set; }

        public CommandLine()
        {            
            options = new OptionSet()
            {
                { "startSize=", "The first input size", (int s) => StartSize = s },
                { "sizeIncrement=", "The input size increment", (int si) => SizeIncrement = si },
                { "numIncrements=", "The number of increments", (int ni) => NumIncrements = ni }
            };
        }

        public bool TryParse(IEnumerable<string> args, TextWriter errorOut)
        {
            bool parsedOK = true;
            try
            {
                options.Parse(args);
            }
            catch (OptionException e)
            {
                parsedOK = false;
                errorOut.WriteLine(e);
            }
            return parsedOK && CheckValid();
        }

        private bool CheckValid()
        {
            return StartSize > 0 && SizeIncrement > 0 && NumIncrements >= 0;
        }

        public void PrintUsage(TextWriter msgOut)
        {
            msgOut.WriteLine("Incorrect command line arguments.");
            msgOut.WriteLine("Usage: ");
            options.WriteOptionDescriptions(msgOut);
        }
    }
}
