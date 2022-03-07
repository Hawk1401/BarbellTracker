using BarbellTracker.Adapter;
using BarbellTracker.Adapter.Interface;
using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;
using BarbellTracker.Services.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BarbellTracker.ApplicationCode.IEventSystem;

namespace BarbellTracker.Plugins.Processing
{
    public class VelocityToAdapterTable : IProcessingPlugin
    {
        public string Name { get; set; }
        public string Description { get; set; }
        private bool Activ;

        private VelocityCSVTranslater translater;
        private UIAdapterManager adapterManager;
        private IEventSystem eventSystem;
        private PluginManager pluginManager;

        public VelocityToAdapterTable(PluginManager pluginManager, UIAdapterManager adapterManager, VelocityCSVTranslater translater, IEventSystem eventSystem)
        {

            Name = nameof(VelocityToAdapterTable);

            this.translater = translater;
            this.eventSystem = eventSystem;
            this.adapterManager = adapterManager;
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

            UICSVVectorAdapter adapter = new UICSVVectorAdapter()
            {
                Name = "Velocity " + extracedVideoInfo.trackedInformation.Name,
                CSV = CSV
            };

            adapterManager.AddNewAdapter(adapter);
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

            if (activatePlugin.PluginName != Name)
            {
                return;
            }

            EventDelegate<ExtracedVideoInfo> eventDelegate = ProcessData;
            eventSystem.Subscribe(eventDelegate);
            Activ = true;
        }
    }
}
