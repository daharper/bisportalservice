using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BizportalService
{
    /// <summary>
    /// Basic logger, logs to 'log.txt' in the base folder.
    /// In debug mode, also displays to the console.
    /// </summary>
    public static class Log
    {
        // the types of logging categories supported.
        private enum LogCategory
        {
            Debug,
            Info,
            Warn,
            Error
        };

        // maps log categories to their labels.
        private static readonly Dictionary<LogCategory, string> LogLabels = new Dictionary<LogCategory, string>
        {
            {LogCategory.Debug, "DEBUG"},
            {LogCategory.Info,  "INFO " },
            {LogCategory.Warn,  "WARN " },
            {LogCategory.Error, "ERROR"}
        };
        
        // used for writing the log file.
        private static readonly StreamWriter Writer;

        // initialization
        static Log()
        {
            if (File.Exists(Settings.LogFile))
            {
                File.Delete(Settings.LogFile);
            }

            Writer = new StreamWriter(Settings.LogFile) {AutoFlush = true};
            Info("log file has been initialized.");

            // we can now register to handle exceptions
            Failure.OnException = Exception;
        }

        // write the specified message to the log file, and in debug show it on the console.
        private static void WriteLine(LogCategory category, string text)
        {
            var timestamp = DateTime.Now.ToString("G");
            var line = $"{timestamp}: {LogLabels[category]} - {text}";

            Writer.WriteLine(line);
//#if DEBUG
            Console.WriteLine(line);
//#endif
        }

        /// <summary>
        /// Logs the specified debug text.
        /// </summary>
        public static void Debug(string text)
        {
            WriteLine(LogCategory.Debug, text);
        }

        /// <summary>
        /// Logs the specified information text.
        /// </summary>
        public static void Info(string text)
        {
            WriteLine(LogCategory.Info, text);
        }

        /// <summary>
        /// Logs the specified warning text.
        /// </summary>
        public static void Warn(string text)
        {
            WriteLine(LogCategory.Warn, text);
        }

        /// <summary>
        /// Logs the specified error text.
        /// </summary>
        public static void Error(string text)
        {
            WriteLine(LogCategory.Error, text);
        }

        /// <summary>
        /// Logs the specified exception and optional text.
        /// </summary>
        public static void Exception(Exception e, string text = "")
        {
            Error(e?.ToString() ?? "<<null exception>>");

            if (!string.IsNullOrEmpty(text))
            {
                Error(text);
            }
        }

        /// <summary>
        /// Logs environment information.
        /// </summary>
        /// <param name="settings"></param>
        public static void Environment(Settings settings)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Environment:");
            sb.AppendLine($"\tbase folder:   {Settings.BaseFolder}");
            sb.AppendLine($"\tsettings file: {Settings.SettingsFile}");
            sb.AppendLine($"\tbatch file:    {settings.BatchFile}");
            sb.AppendLine($"\tjar file:      {settings.JarFile}");
            sb.Append($"\tmonitoring:    {settings.MonitorChanges}");
            Info(sb.ToString());
        }

        /// <summary>
        /// Closes the log file, and disposes of it.
        /// </summary>
        public static void Close()
        {
            Writer.Flush();
            Writer.Dispose();
        }
    }
}
