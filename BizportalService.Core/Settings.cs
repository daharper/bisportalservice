using System;
using System.IO;
using System.Xml.Serialization;

namespace BizportalService.Core
{
    /// <summary>
    /// Contains the basic settings to start or stop a batch process.
    /// The settings file should be placed in the root folder.
    /// </summary>
    public class Settings
    {
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
            Filename = other.Filename;
            JarFile = other.JarFile;
            BatchFile = other.BatchFile;
            MonitorChanges = other.MonitorChanges;
        }

        /// <summary>
        /// The file name this settings class was initialized from.
        /// </summary>
        public string Filename { get; set; } = "";

        /// <summary>
        /// Gets the path to the folder of the settings filename.
        /// </summary>
        public string Folder => Path.GetDirectoryName(Filename);

        /// <summary>
        /// The fully qualified filename of the batch file to execute.
        /// </summary>
        public string BatchFile { get; set; } = @"d:\bizportalReskin\api\StartReskinApi.bat";

        /// <summary>
        /// The fully qualified filename of the jar to execute.
        /// </summary>
        public string JarFile { get; set; } = @"d:\bizportalReskin\api\bizportal-api-1.0.0-SNAPSHOT.jar";

        /// <summary>
        /// Determines whether to monitor deployment changes.
        /// </summary>
        public bool MonitorChanges { get; set; } = true;

        /// <summary>
        /// Saves the settings to file.
        /// </summary>
        public void Save()
        {
            if (!Directory.Exists(Folder)) return;

            var serializer = new XmlSerializer(typeof(Settings));

            using (var writer = new StreamWriter(Filename))
            {
                serializer.Serialize(writer, this);
            }
        }

        /// <summary>
        /// Loads the settings from the specified filename.
        /// </summary>
        /// <param name="filename">A fully qualified filename</param>
        /// <returns>An instance of settings initialized from the specified file</returns>
        public static Settings Load(string filename)
        {
            Settings settings;

            if (!File.Exists(filename))
            {
                settings = new Settings {Filename = filename};
                settings.Save();
                return settings;
            }

            var serializer = new XmlSerializer(typeof(Settings));
            
            using (var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                settings = (Settings)serializer.Deserialize(fileStream);
            }

            settings.Filename = filename;
            return settings;
        }

        public static string ResolveFilename()
        {
            const string defaultFilename = @"d:\bizportalReskin\api\Settings.xml";

            string filename = null;

            try
            {
                filename = Environment.GetEnvironmentVariable("BIZPORTAL_SETTINGS");
            }
            catch { /* do nothing - permission problem etc. */ }

            if (string.IsNullOrEmpty(filename))
            {
                filename = defaultFilename;
            }

            return filename;
        }
    }
}
