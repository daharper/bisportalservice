using System;
using BizportalService;

namespace BizportalConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("> starting up...");
            Console.WriteLine();

            var changeManager = new ChangeManager();

            Console.WriteLine();
            Console.WriteLine("> running...press enter to stop");
            Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine("> shutting down...");
            Console.WriteLine();

            changeManager.Dispose();

            Console.WriteLine();
            Console.WriteLine("press enter to quit....");

            Console.ReadLine();
        }
    }
}
