using System;
using System.IO;

namespace BizportalService
{
    /// <summary>
    /// Responsible for reacting to change at runtime.
    ///
    /// See Overview.txt for more information.
    /// </summary>
    public class Kernel : IDisposable
    {
        private Settings _settings;
        private readonly FileSystemWatcher _watcher;
        private readonly ProcessManager _processManager;

        /// <summary>
        /// Creates a new instance of the Change Manager type initialized with
        /// the specified settings. 
        /// </summary>
        public Kernel()
        {
            _settings = Settings.Load();

            Log.Environment(_settings);
            Log.Info("initiating process start up");

            // start the process up
            _processManager = new ProcessManager(_settings);
            _processManager.StartProcess(_settings);

            // set up our change watcher
            if (_settings.MonitorChanges)
            {
                Log.Info("starting change monitor");

                _watcher = new FileSystemWatcher
                {
                    Path = Settings.BaseFolder,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                    EnableRaisingEvents = true
                };

                _watcher.Changed += OnChanged;
                _watcher.Created += OnCreated;
            }
        }

        /// <summary>
        /// Clean up any resources.
        /// </summary>
        public void Dispose()
        {
            Log.Info("cleaning up resources");

            _watcher?.Dispose();
            _processManager?.Dispose();

            Log.Close();
        }

        /// <summary>
        /// If a new jar file has been dropped into the folder, then
        /// update the settings file and restart.
        /// </summary>
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (!EventFilter.CanAccept(EventType.Created)) return;
            if (!e.FullPath.EndsWith(".jar")) return;
            if (_settings.JarFile == e.FullPath) return;

            ProcessNewFile(e.FullPath);
        }

        /// <summary>
        /// If the configuration file has changed, restart with a new
        /// process, if indeed the batch file name has changed.
        /// </summary>
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (!EventFilter.CanAccept(EventType.Any)) return;

            // this is triggered by a file overwrite
            if (e.FullPath.EndsWith(".jar"))
            {
                Log.Info("executing process file has been overwritten");
                ProcessNewFile(e.FullPath);
                return;
            }

            if (string.CompareOrdinal(e.FullPath, Settings.SettingsFile) != 0) return;

            Log.Info($"settings update detected");
            var prevSettings = new Settings(_settings);

            Log.Info("loaded new settings");
            _settings = Settings.Load();

            // if neither JarFile nor BatchFile has changed, then there's nothing to do
            if (string.CompareOrdinal(prevSettings.JarFile, _settings.JarFile) == 0 &&
                string.CompareOrdinal(prevSettings.BatchFile, _settings.BatchFile) == 0)
            {
                Log.Info("no file changes detected, nothing to do");
                return;
            }

            _processManager.RestartProcess(_settings);
        }

        /// <summary>
        /// Processes a new file, if it is not an overwrite of the running file
        /// then the settings file is first updated.
        /// </summary>
        /// <param name="filename">A new file dropped into the settings folder</param>
        private void ProcessNewFile(string filename)
        {
            Log.Info($"new jar found detected: {filename}");

            if (string.CompareOrdinal(filename, _settings.JarFile) != 0)
            {
                Log.Info("updating settings file");
                _settings.JarFile = filename;
                _settings.Save();
            }

            _processManager.RestartProcess(_settings);
        }
    }
}
