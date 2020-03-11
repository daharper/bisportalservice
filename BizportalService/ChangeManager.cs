using System;
using System.IO;

namespace BizportalService
{
    /// <summary>
    /// Responsible for reacting to change in runtime configuration.
    ///
    /// There are two types of change:
    ///
    /// 1. User drops a new .jar file into the base folder (SIT/UAT)
    /// 2. User updates the Settings.xml file (PROD)
    ///
    /// In the first scenario, used mainly in SIT/UAT, new releases
    /// are dropped in the same folder. Since the process is running
    /// the file cannot be replaced, a new filename is provided by
    /// the user, i.e. MyApplication(2).jar, which will trigger the
    /// this class to stop the existing process, update the
    /// Settings.xml with the new file name, then execute the new file.
    ///
    /// In the second scenario, used in PROD, new releases are put in
    /// their own subfolder (i.e. RELEASE1, RELEASE2 etc). This enables
    /// easy rollback if there is a problem. In this case, the user
    /// will update the Settings.xml directly with the new file path,
    /// which will trigger this class to stop the existing process
    /// and start a new one.
    ///
    /// See Overview.txt for more information
    /// </summary>
    public class ChangeManager : IDisposable
    {
        private Settings _settings;
        private readonly FileSystemWatcher _watcher;
        private readonly ProcessManager _processManager;

        /// <summary>
        /// Creates a new instance of the Change Manager type initialized with
        /// the specified settings. 
        /// </summary>
        public ChangeManager()
        {
            var filename = Settings.ResolveFilename();
            _settings = Settings.Load(filename);

            Log.WriteEnvironment(_settings);
            Log.WriteLine("* initiating process start up");

            // start the process up
            _processManager = new ProcessManager(_settings);
            _processManager.StartProcess(_settings);

            // set up our change watcher
            if (_settings.MonitorChanges)
            {
                _watcher = new FileSystemWatcher
                {
                    Path = _settings.Folder,
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
            _watcher?.Dispose();
            _processManager?.Dispose();
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
                ProcessNewFile(e.FullPath);
                return;
            }

            if (string.CompareOrdinal(e.FullPath, _settings.Filename) != 0) return;

            Log.WriteLine($"* settings update detected");
            var prevSettings = new Settings(_settings);

            Log.WriteLine("* loaded new settings");
            _settings = Settings.Load(_settings.Filename);

            // if neither JarFile nor BatchFile has changed, then there's nothing to do
            if (string.CompareOrdinal(prevSettings.JarFile, _settings.JarFile) == 0 &&
                string.CompareOrdinal(prevSettings.BatchFile, _settings.BatchFile) == 0)
            {
                Log.WriteLine("* no file changes detected, nothing to do.");
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
            Log.WriteLine($"* new jar found detected: {filename}");

            if (string.CompareOrdinal(filename, _settings.JarFile) != 0)
            {
                Log.WriteLine("* updating settings file");
                _settings.JarFile = filename;
                _settings.Save();
            }

            _processManager.RestartProcess(_settings);
        }
    }
}
