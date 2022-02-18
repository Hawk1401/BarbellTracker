﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbellTracker.Plugin.WPF_MVVM_UI.DataStructures;
using WPF_UI_HelperClasses;

namespace BarbellTracker.Plugin.WPF_MVVM_UI.ViewModel
{
    internal class PluginSettingsControlViewModel : PropertyChangedNotifier
    {
        private ObservableCollection<PluginStatus> _pluginListWithEnableStatus = new ObservableCollection<PluginStatus>();
        public PluginSettingsControlViewModel()
        {
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

    }
}