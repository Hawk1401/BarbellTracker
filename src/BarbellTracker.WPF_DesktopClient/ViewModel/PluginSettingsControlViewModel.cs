﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.Adapter;
using BarbellTracker.WPF_DesktopClient.DataStructures;
using BarbellTracker.WPF_HelperClasses;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    internal class PluginSettingsControlViewModel : ViewModelBase
    {
        private ObservableCollection<PluginStatus> _pluginListWithEnableStatus = new ObservableCollection<PluginStatus>();
        public PluginSettingsControlViewModel()
        {
            GetPluginInstancesOfProcessingPlugins();

            // Example Data for _pluginsListWithEnableStatus
            PluginWithEnableStatus.Add(new PluginStatus("PluginOne", false));
            PluginWithEnableStatus.Add(new PluginStatus("PluginTwo", false));
            PluginWithEnableStatus.Add(new PluginStatus("PluginThree", false));
            PluginWithEnableStatus.Add(new PluginStatus("PluginFour", false));
        }

        public ObservableCollection<PluginStatus> PluginWithEnableStatus
        {
            get { return _pluginListWithEnableStatus; }
            set
            {
                _pluginListWithEnableStatus = value;
                OnPropertyChanged("_pluginListWithEnableStatus");
            }
        }


        public void GetPluginInstancesOfProcessingPlugins()
        {
            List<Adapter.Interface.IProcessingPlugin> plugins = PluginManager.Instance.GetProcessingPlugins();
            foreach (Adapter.Interface.IProcessingPlugin plugin in plugins)
            {
                _pluginListWithEnableStatus.Add(new PluginStatus(plugin.Name, plugin.IsActiv()));
            }
        }


    }
}
