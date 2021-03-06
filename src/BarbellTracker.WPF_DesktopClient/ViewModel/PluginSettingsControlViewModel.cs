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
using Microsoft.Extensions.DependencyInjection;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    internal class PluginSettingsControlViewModel : ViewModelBase
    {
        private ObservableCollection<PluginStatus> _pluginsWithStatus = new ObservableCollection<PluginStatus>();
        private IEventSystem eventSystem;
        private PluginManager pluginManager;
        public PluginSettingsControlViewModel()
        {
            this.eventSystem = DependencyInjectionHelper.provider.GetRequiredService<IEventSystem>(); 
            this.pluginManager = DependencyInjectionHelper.provider.GetRequiredService<PluginManager>();
            GetPluginInstancesOfProcessingPlugins();
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
                PluginsWithStatus.Add(new PluginStatus(eventSystem ,plugin.Name, plugin.IsActiv()));
            }
        }


    }
}
