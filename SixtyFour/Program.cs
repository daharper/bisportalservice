using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SixtyFour
{
    class Program
    {
        enum Actions
        {
            Encode,
            Decode
        };

        private static readonly Dictionary<string, Actions> actions = 
            new Dictionary<string, Actions>(StringComparer.InvariantCultureIgnoreCase)
        {
            {"encode",Actions.Encode},
            {"decode",Actions.Decode}
        };

        static void Main(string[] args)
        {
            if (args.Length != 3) Usage();
            if (!actions.ContainsKey(args[0])) Usage();
            if (!File.Exists(args[1])) Usage();

            try
            {
                Execute(actions[args[0]], args[1], args[2]);
                Console.WriteLine("> ok.");
            }
            catch
            {
                Console.WriteLine("> error.");
            }
        }

        static void Execute(Actions action, string infile, string outfile)
        {
            if (action == Actions.Encode)
            {
                var bytes = File.ReadAllBytes(infile);
                var encoded = Convert.ToBase64String(bytes);
                File.WriteAllText(outfile, encoded);
            }
            else
            {
                var encoded = File.ReadAllText(infile);
                var bytes = Convert.FromBase64String(encoded);
                File.WriteAllBytes(outfile, bytes);
            }
        }

        static void Usage()
        {
            Console.WriteLine("usage: sixtyfour encode|decode example.exe example.dat");
            Environment.Exit(-1);
        }
    }
}
