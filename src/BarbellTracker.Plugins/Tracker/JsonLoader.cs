using BarbellTracker.Adapter;
using BarbellTracker.Adapter.Interface;
using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;
using BarbellTracker.ApplicationCode.EventModel;
using BarbellTracker.DomainCode;
using System;
using System.Collections.Generic;
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

        public JsonLoader(IEventSystem eventSystem, PluginManager pluginManager)
        {
            this.eventSystem = eventSystem;
            pluginManager.AddPlugin(this);

            EventDelegate<StartExtractVideoInfo> StartExtractVideoInfoDelegate = StartTracking;
            eventSystem.Subscribe(StartExtractVideoInfoDelegate);

            Name = "JsonLoader";
        }
        public async void LoadFile(FileSelected fileSelected)
        {
            try
            {
                var conent = File.ReadAllText(fileSelected.FilePath);
                var trackedInformation = JsonSerializer.Deserialize<TrackedInformation>(conent);

                eventSystem.Fire(new ExtracedVideoInfo()
                {
                    trackedInformation = trackedInformation,
                });
            }
            catch(Exception ex)
            {

            }
            
        }

        public void StartTracking(StartExtractVideoInfo extracedVideoInfo)
        {

            var StartExtractionInformation = extracedVideoInfo.StartExtractionInformation;
            if (StartExtractionInformation.PluginName != this.Name)
            {
                return;
            }

            EventDelegate<FileSelected> FileSelectedDelegate = LoadFile;
            eventSystem.Subscribe(FileSelectedDelegate);
            eventSystem.Fire(new SelectFile() { FileExtensionRestriction = new string [] { ".json"} });
        }
    }
}
