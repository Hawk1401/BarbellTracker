using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.Adapter.Interface;
using BarbellTracker.ApplicationCode;
using BarbellTracker.DomainCode;
using BarbellTracker.Services.Implementation;

namespace BarbellTracker.Plugins.Processing
{
    public class AccelerationToTable : IProcessingPlugin
    {
        public string Name { get; set; }
        public string Description { get; set; }
        private bool Activ;

        private AccelerationCSVTranslater translater;
        private FileManager fileManager;
        public AccelerationToTable(AccelerationCSVTranslater translater, FileManager fileManager)
        {
            this.translater = translater;
            this.fileManager = fileManager;

            EventSystem.Subscribe(Event.ActivatePlugin, Activate);
            EventSystem.Subscribe(Event.DeactivatePlugin, Deactivate);
            Activ = false;
        }

        public bool IsActiv()
        {
            return Activ;
        }

        public async Task ProcessData(EventContext eventContext)
        {
            var trackedInformation = eventContext.Arg as TrackedInformation;
            var CSV = translater.GetCSV(trackedInformation);
            fileManager.Write("Acceleration.CSV", CSV.ToString());
        }
        public async Task Activate(EventContext eventContext)
        {
            EventSystem.Subscribe(Event.ExtracedVideoInfo, ProcessData);
            Activ = true;
        }
        public async Task Deactivate(EventContext eventContext)
        {
            EventSystem.Unsubscribe(Event.ExtracedVideoInfo, ProcessData);
            Activ = false;
        }
    }
}
