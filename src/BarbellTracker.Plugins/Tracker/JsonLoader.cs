using BarbellTracker.Adapter;
using BarbellTracker.Adapter.Interface;
using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;
using BarbellTracker.ApplicationCode.EventModel;
using BarbellTracker.DomainCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static BarbellTracker.ApplicationCode.IEventSystem;

namespace BarbellTracker.Plugins.Tracker
{
    public class JsonLoader : IPlugin, ITrackerPlugin
    {
        public string Name { get; set; }
        public string Description { get; set; }

        private IEventSystem eventSystem;
        private PluginManager pluginManager;
        private string currentExtractionId;

        public JsonLoader(IEventSystem eventSystem, PluginManager pluginManager)
        {
            this.eventSystem = eventSystem;
            this.pluginManager = pluginManager;

            pluginManager.AddPlugin(this);

            EventDelegate<StartExtractVideoInfo> StartExtractVideoInfoDelegate = StartTracking;
            eventSystem.Subscribe(StartExtractVideoInfoDelegate);

            Name = "JsonLoader";
        }
        public void LoadFile(FileSelected fileSelected)
        {
            // This tracker only reacts to the file it asked for
            EventDelegate<FileSelected> FileSelectedDelegate = LoadFile;
            eventSystem.Unsubscribe(FileSelectedDelegate);

            try
            {
                var content = File.ReadAllText(fileSelected.FilePath);
                var loaded = JsonSerializer.Deserialize<TrackedInformation>(content);

                if (loaded == null || loaded.Positions == null)
                {
                    Trace.TraceError($"{Name}: '{fileSelected.FilePath}' does not contain tracked positions");
                    return;
                }

                // The Id is the cache key of the services, so it has to be unique per extraction
                // and not only per file content (e.g. two files with the same Id in the json).
                var trackedInformation = new TrackedInformation()
                {
                    Id = $"{loaded.Id}-{currentExtractionId}",
                    Name = loaded.Name,
                    Positions = loaded.Positions,
                    FrameRate = loaded.FrameRate,
                    PixelPerCm = loaded.PixelPerCm,
                };

                eventSystem.Fire(new ExtracedVideoInfo()
                {
                    trackedInformation = trackedInformation,
                });
            }
            catch (Exception ex) when (ex is IOException || ex is JsonException || ex is UnauthorizedAccessException || ex is ArgumentException)
            {
                Trace.TraceError($"{Name}: could not load '{fileSelected.FilePath}': {ex.Message}");
            }
        }

        public void StartTracking(StartExtractVideoInfo extracedVideoInfo)
        {
            var StartExtractionInformation = extracedVideoInfo.StartExtractionInformation;
            if (StartExtractionInformation.PluginName != this.Name)
            {
                return;
            }

            currentExtractionId = StartExtractionInformation.Id;

            EventDelegate<FileSelected> FileSelectedDelegate = LoadFile;
            eventSystem.Subscribe(FileSelectedDelegate);
            eventSystem.Fire(new SelectFile() { FileExtensionRestriction = new string [] { ".json"} });
        }

    }
}
