using System.IO;
using System.Xml.Serialization;

namespace BizportalService
{
    /// <summary>
    /// Contains the basic settings required to start or stop the batch process
    /// to execute a java file. The settings file should be located in the
    /// same folder as the service - known as the 'base folder'. This is
    /// a convention we are using, i.e.
    ///
    /// basefolder\
    ///     bizportalservice.exe
    ///     settings.xml
    ///     log.txt
    ///
    /// The batch file and jar file can be located anywhere.
    /// </summary>
    public class Settings
    {
        // the fully qualified path to the base folder.
        public static readonly string BaseFolder;

        // the fully qualified path to the settings file.
        public static readonly string SettingsFile;

        // the fully qualified path the the log file.
        public static readonly string LogFile;

        static Settings()
        {
            BaseFolder = Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);

            Failure.RaiseIf(!Directory.Exists(BaseFolder),
                new DirectoryNotFoundException($"Base Folder not found: {BaseFolder}"));

            LogFile = Path.Combine(BaseFolder, "log.txt");

            SettingsFile = Path.Combine(BaseFolder, "settings.xml");

            Failure.RaiseIf(!File.Exists(SettingsFile),
                new FileNotFoundException($"Settings file not found: {SettingsFile}"));
        }

        /// <summary>
        /// Initializes a new instance of the Settings type.
        /// </summary>
        public Settings()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Settings type from another instance.
        /// </summary>
        /// <param name="other">Another instance of Settings</param>
        public Settings(Settings other)
        {
            JarFile = other.JarFile;
            BatchFile = other.BatchFile;
            MonitorChanges = other.MonitorChanges;
        }

        /// <summary>
        /// The fully qualified filename of the batch file to execute.
        /// </summary>
        public string BatchFile { get; set; }

        /// <summary>
        /// The fully qualified filename of the jar to execute.
        /// </summary>
        public string JarFile { get; set; }

        /// <summary>
        /// Determines whether to monitor deployment changes.
        /// </summary>
        public bool MonitorChanges { get; set; } = true;

        /// <summary>
        /// Number of seconds to wait for initial process to complete.
        /// </summary>
        public int WaitTimeInSecs { get; set; } = 45;

        /// <summary>
        /// Saves the settings to file.
        /// </summary>
        public void Save()
        {
            if (!Directory.Exists(BaseFolder)) return;

            var serializer = new XmlSerializer(typeof(Settings));

            using (var writer = new StreamWriter(SettingsFile))
            {
                serializer.Serialize(writer, this);
            }
        }

        /// <summary>
        /// Loads the settings from the specified filename.
        /// </summary>
        /// <returns>An instance of settings initialized from the specified file</returns>
        public static Settings Load()
        {
            Settings settings;

            var serializer = new XmlSerializer(typeof(Settings));
            
            using (var fs = new FileStream(SettingsFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                settings = (Settings)serializer.Deserialize(fs);
            }

            if (!File.Exists(settings.BatchFile))
            {
                Failure.Raise<FileNotFoundException>(
                    $"Batch file does not exist: {settings.BatchFile}");
            }

            if (!File.Exists(settings.JarFile))
            {
                Failure.Raise<FileNotFoundException>(
                    $"Jar file does not exist: {settings.JarFile}");
            }

            return settings;
        }
    }
}
