using System;
using System.Diagnostics;
using System.IO;
using System.Management;

namespace BizportalService.Core
{
    /// <summary>
    /// Responsible for starting and stopping a batch process
    /// </summary>
    public class ProcessManager : IDisposable
    {
        private Settings _settings;
        private Process _process;

        /// <summary>
        /// Initializes a new instance of the ProcessManager type from the specified settings.
        /// </summary>
        /// <param name="settings"></param>
        public ProcessManager(Settings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Stops the process.
        /// </summary>
        public void StopProcess()
        {
            Log.WriteLine($"* stopping any child processes:");
      
            KillChildProcesses();

            if (_process == null) return;

            if (!_process.HasExited)
            {
                _process.Kill();
            }

            _process.Dispose();
            _process = null;
        }

        /// <summary>
        /// Starts the batch process with the specified filename.
        /// </summary>
        /// <param name="settings">The environment settings</param>
        public void StartProcess(Settings settings)
        {
            StopProcess();

            _settings = settings;

            Log.WriteLine($"* starting process to execute: {_settings.JarFile}");

            if (!File.Exists(_settings.JarFile))
            {
                Log.WriteLine($"* settings file not found error: {_settings.JarFile}");
                return;
            }

            if (!File.Exists(_settings.BatchFile))
            {
                Log.WriteLine($"batch file not found error: {_settings.BatchFile}");
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C {_settings.BatchFile} \"{_settings.JarFile}\"",
                CreateNoWindow = true,
                ErrorDialog = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            _process = new Process { StartInfo = startInfo };
            _process.Start();
        }

        /// <summary>
        /// Restarts the process.
        /// </summary>
        /// <param name="settings"></param>
        public void RestartProcess(Settings settings)
        {
            _settings = settings;

            Log.WriteEnvironment(_settings);
            Log.WriteLine("* restarting spring with new configuration:");
            StartProcess(_settings);
        }

        /// <summary>
        /// Performs any resource cleanup.
        /// </summary>
        public void Dispose()
        {
            StopProcess();
        }

        private void KillChildProcesses()
        {
            var searcher = new ManagementObjectSearcher(
                $"select * from Win32_Process where name = \"java.exe\"");

            foreach (var obj in searcher.Get())
            {
                var commandLine = Convert.ToString(obj["commandline"]).ToLowerInvariant();
                var jarFile = Path.GetFileName(_settings.JarFile).ToLowerInvariant();
                var baseFolder = _settings.Folder.ToLowerInvariant();

                Log.WriteLine($"\t* detected java process: {commandLine}");

                if (commandLine.Contains(jarFile) || commandLine.Contains(baseFolder))
                {
                    var pid = Convert.ToInt32(obj["ProcessID"]);
                    var process = Process.GetProcessById(pid);

                    Log.WriteLine($"\t  => killed [{process.Id}]");

                    if (!process.HasExited)
                    {
                        process.Kill();
                    }

                    process.Dispose();
                }
            }
        }
    }
}
