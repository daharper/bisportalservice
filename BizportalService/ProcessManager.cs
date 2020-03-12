using System;
using System.Diagnostics;
using System.IO;
using System.Management;

namespace BizportalService
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
            Log.Info($"stopping any java (jarfile) processes");

            StopChildJavaProcesses();

            if (_process == null) return;

            Log.Info("cleaning up the batch process");

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

            Log.Info($"starting process to execute: {_settings.JarFile}");

            /*
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
            */

            var startInfo = new ProcessStartInfo
            {
                FileName = "java",
                Arguments = $"-jar {_settings.JarFile}"
            };

            _process = new Process { StartInfo = startInfo };
            _process.Start();
            //_process.WaitForExit(60000);
        }

        /// <summary>
        /// Restarts the process.
        /// </summary>
        /// <param name="settings"></param>
        public void RestartProcess(Settings settings)
        {
            _settings = settings;

            Log.Info("restarting with new configuration:");
            Log.Environment(_settings);
            
            StartProcess(_settings);
        }

        /// <summary>
        /// Performs any resource cleanup.
        /// </summary>
        public void Dispose()
        {
            StopProcess();
        }

        /// <summary>
        /// Kills the jar file process, whether started by this service or not.
        /// </summary>
        private void StopChildJavaProcesses()
        {
            const string wmiQuery = "select * from Win32_Process where name = \"java.exe\"";

            var searcher = new ManagementObjectSearcher(wmiQuery);

            foreach (var obj in searcher.Get())
            {
                var commandLine = Convert.ToString(obj["commandline"]).ToLowerInvariant();
                var jarFile = Path.GetFileName(_settings.JarFile).ToLowerInvariant();
                var baseFolder = Settings.BaseFolder.ToLowerInvariant();

                Log.Info($"detected java process: {commandLine}");

                if (!commandLine.Contains(jarFile) && 
                    !commandLine.Contains(baseFolder)) continue;

                var pid = Convert.ToInt32(obj["ProcessID"]);
                var process = Process.GetProcessById(pid);

                Log.Info($"=> stopped [{process.Id}]");

                if (!process.HasExited) process.Kill();
                process.Dispose();
            }
        }
    }
}
