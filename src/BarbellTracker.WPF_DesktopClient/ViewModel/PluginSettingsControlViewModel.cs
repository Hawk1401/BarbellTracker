using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.Adapter;
using BarbellTracker.ApplicationCode;
using BarbellTracker.WPF_DesktopClient.DataStructures;
using BarbellTracker.WPF_HelperClasses;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    internal class PluginSettingsControlViewModel : ViewModelBase
    {
        private ObservableCollection<PluginStatus> _pluginsWithStatus = new ObservableCollection<PluginStatus>();
        private IEventSystem eventSystem;
        private PluginManager pluginManager;
        public PluginSettingsControlViewModel(IEventSystem eventSystem, PluginManager pluginManager)
        {
            this.eventSystem = eventSystem;
            this.pluginManager = pluginManager;
            GetPluginInstancesOfProcessingPlugins();

            // Example Data for PluginsWithStatus
            PluginsWithStatus.Add(new PluginStatus(eventSystem, "PluginOne", false));
            PluginsWithStatus.Add(new PluginStatus(eventSystem, "PluginTwo", false));
            PluginsWithStatus.Add(new PluginStatus(eventSystem, "PluginThree", false));
            PluginsWithStatus.Add(new PluginStatus(eventSystem, "PluginFour", false));
        }

        public ObservableCollection<PluginStatus> PluginsWithStatus
        {
            get { return _pluginsWithStatus; }
            set
            {
                _pluginsWithStatus = value;
                OnPropertyChanged("_pluginsWithStatus");
            }
        }


        public void GetPluginInstancesOfProcessingPlugins()
        {
            List<Adapter.Interface.IProcessingPlugin> plugins = pluginManager.GetProcessingPlugins();
            foreach (Adapter.Interface.IProcessingPlugin plugin in plugins)
            {
                PluginsWithStatus.Add(new PluginStatus(eventSystem, plugin.Name, plugin.IsActiv()));
            }
        }


    }
}
