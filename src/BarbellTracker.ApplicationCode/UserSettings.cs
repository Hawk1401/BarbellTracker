using System;
using System.IO;
using System.Text.Json;

namespace BarbellTracker.ApplicationCode
{
    /// <summary>The settings that are kept between two starts of the application</summary>
    public class UserSettings
    {
        /// <summary>Savitzky-Golay window of the position smoothing, 0 = off</summary>
        public int SmoothingWindow { get; set; } = 5;

        /// <summary>Minimal vertical movement in meters that counts as repetition</summary>
        public double MinRangeOfMotion { get; set; } = 0.2;

        /// <summary>Where the csv files of the processing plugins are written, empty = default folder</summary>
        public string OutputFolder { get; set; } = string.Empty;
    }

    /// <summary>Loads and saves a settings object as json. A missing or damaged file gives the defaults.</summary>
    public class JsonSettingsStore<T> where T : class, new()
    {
        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

        private readonly string file;

        public JsonSettingsStore(string file)
        {
            this.file = file ?? throw new ArgumentNullException(nameof(file));
        }

        public T Load()
        {
            try
            {
                if (File.Exists(file))
                {
                    return JsonSerializer.Deserialize<T>(File.ReadAllText(file)) ?? new T();
                }
            }
            catch (Exception ex) when (ex is JsonException || ex is IOException || ex is UnauthorizedAccessException)
            {
                // fall through: a broken settings file must not prevent the start
            }
            return new T();
        }

        /// <returns>false if the file could not be written</returns>
        public bool Save(T settings)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(file)));
                File.WriteAllText(file, JsonSerializer.Serialize(settings, Options));
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}
