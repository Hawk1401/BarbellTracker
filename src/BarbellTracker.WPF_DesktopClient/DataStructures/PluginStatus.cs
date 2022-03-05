using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.WPF_DesktopClient.DataStructures
{
    class PluginStatus : WPF_HelperClasses.PropertyChangedNotifier
    {
        private string _pluginName;
        private bool _status;
        private IEventSystem eventSystem;
        public PluginStatus(IEventSystem eventSystem, string pluginName, bool status)
        {
            this.PluginName = pluginName;
            this.Status = status;
            this.eventSystem = eventSystem;

        }

        public string PluginName 
        { 
            get { return _pluginName; }
            set 
            {
                _pluginName = value;
                OnPropertyChanged("PluginName");
            }
        }

        public bool Status 
        {
            get { return _status; }
            set 
            {
                _status = value;
                OnPropertyChanged("Status");
                if(Status)
                {
                    eventSystem.Fire(new ActivatePlugin() { PluginName = PluginName });
                    return;
                }
                eventSystem.Fire(new DeactivatePlugin() { PluginName = PluginName });


            }
        }
        // kein ToString override, da nicht gebraucht ;)
    }
}
