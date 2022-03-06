using BarbellTracker.Adapter.Interface;
using BarbellTracker.ApplicationCode;
using BarbellTracker.DomainCode;
using BarbellTracker.Services.Implementation;
using BarbellTracker.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.ApplicationCode.Event;
using static BarbellTracker.ApplicationCode.IEventSystem;

namespace BarbellTracker.Plugins.Processing
{
    public class VelocityToCSVFile : IProcessingPlugin
    {
        public string Name { get; set; }
        public string Description { get; set; }
        private bool Activ;

        private VelocityCSVTranslater translater;
        private FileManager fileManager;
        private IEventSystem eventSystem;
        private PluginManager pluginManager;

        public VelocityToCSVFile(PluginManager pluginManager, VelocityCSVTranslater translater, FileManager fileManager, IEventSystem eventSystem)
        {
            Name = nameof(VelocityToCSVFile);


            this.translater = translater;
            this.fileManager = fileManager;
            this.eventSystem = eventSystem;
            this.pluginManager = pluginManager;

            pluginManager.AddPlugin(this);
            EventDelegate<ActivatePlugin> ActivatePluginDelegate = Activate;
            EventDelegate<DeactivatePlugin> DeactivatePluginDelegate = Deactivate;

            eventSystem.Subscribe(ActivatePluginDelegate);
            eventSystem.Subscribe(DeactivatePluginDelegate);
            Activ = false;
        }

        public bool IsActiv()
        {
            return Activ;
        }



        public void ProcessData(ExtracedVideoInfo extracedVideoInfo)
        {
            var trackedInformation = extracedVideoInfo.trackedInformation;
            var CSV = translater.GetCSV(trackedInformation);
            fileManager.Write("Velocity.CSV", CSV.ToString());
        }

        public void Deactivate(DeactivatePlugin deactivatePlugin)
        {
            if (deactivatePlugin.PluginName != Name)
            {
                return;
            }

            EventDelegate<ExtracedVideoInfo> eventDelegate = ProcessData;
            eventSystem.Unsubscribe(eventDelegate);
            Activ = false;
        }

        public void Activate(ActivatePlugin activatePlugin)
        {
            if(activatePlugin.PluginName != Name)
            {
                return;
            }

            EventDelegate<ExtracedVideoInfo> eventDelegate = ProcessData;
            eventSystem.Subscribe(eventDelegate);
            Activ = true;
        }
    }
}
