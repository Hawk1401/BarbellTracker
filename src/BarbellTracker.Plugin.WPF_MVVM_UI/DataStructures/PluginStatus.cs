using BarbellTracker.ApplicationCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.Plugin.WPF_MVVM_UI.DataStructures
{
    class PluginStatus : WPF_UI_HelperClasses.PropertyChangedNotifier
    {
        private string _pluginName;
        private bool _status;

        public PluginStatus(string pluginName, bool status)
        {
            this.PluginName = pluginName;
            this.Status = status;
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
                    EventSystem.Fire(this, Event.ActivatePlugin, PluginName);
                    return;
                }
                EventSystem.Fire(this, Event.DeactivatePlugin, PluginName);

            }
        }
        // kein ToString override, da nicht gebraucht ;)
    }
}
