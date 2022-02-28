using BarbellTracker.Adapter.Interface;
using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.EventModel;
using BarbellTracker.DomainCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BarbellTracker.Plugins.Tracker
{
    public class JsonLoader : IPlugin, ITrackerPlugin
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public JsonLoader()
        {
            EventSystem.Subscribe(Event.StartExtractVideoInfo, StartTacking);
            Name = "JsonLoader";
        }
        public async Task StartTacking(EventContext eventContext)
        {
            var StartExtractionInformation = eventContext.Arg as StartExtractionInformation;
            if(StartExtractionInformation.PluginName != this.Name)
            {
                return;
            }

            EventSystem.Subscribe(Event.FileSelected, LoadFile);
            EventSystem.Fire(this, Event.SelectFile, "");
        }
        public async Task LoadFile(EventContext eventContext)
        {
            try
            {
                var conent = File.ReadAllText((eventContext.Arg as FilePath).Value);
                var trackedInformation = JsonSerializer.Deserialize<TrackedInformation>(conent);

                EventSystem.Fire(this, Event.ExtracedVideoInfo, trackedInformation);
            }
            catch(Exception ex)
            {

            }
            
        }
    }
}
