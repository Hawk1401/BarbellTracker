using System;
using System.IO;

namespace BarbellTracker.ApplicationCode
{
    /// <summary>The folders where the application stores its data</summary>
    public static class AppPaths
    {
        public static string DataFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BarbellTracker");
        public static string SessionsFolder => Path.Combine(DataFolder, "sessions");
        public static string SettingsFile => Path.Combine(DataFolder, "settings.json");
        public static string LogsFolder => Path.Combine(DataFolder, "logs");
        public static string DefaultExportFolder => Path.Combine(DataFolder, "exports");
    }
}
