using System;
using System.Diagnostics;

namespace BizportalService.Core
{
    public static class Log
    {
        [Conditional("DEBUG")]
        public static void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        [Conditional("DEBUG")]
        public static void WriteEnvironment(Settings settings)
        {
            WriteLine("* environment");
            WriteLine($"\tbase folder:   {settings.Folder}");
            WriteLine($"\tsettings file: {settings.Filename}");
            WriteLine($"\tbatch file:    {settings.BatchFile}");
            WriteLine($"\tjar file:      {settings.JarFile}");
            WriteLine($"\tmonitoring:    {settings.MonitorChanges}");
            WriteLine("");
        }
    }
}
