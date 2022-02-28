﻿using BarbellTracker.Adapter;
using BarbellTracker.Adapter.Model;
using BarbellTracker.ApplicationCode;
using BarbellTracker.WPF_DesktopClient.DataStructures;
using BarbellTracker.WPF_DesktopClient.View;
using BarbellTracker.WPF_HelperClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    internal class AdapterControlViewModel : ViewModelBase
    {
        // Bsp: UIVideo; CSVTabelle;
        // dynamic selection of view(Video, Tabelle, ...)
        private ObservableCollection<AdapterModel> _adapterViews = new();
        private ObservableCollection<ViewModelBase> _tabItemsViewModel = new();


        public AdapterControlViewModel()
        {
            EventSystem.Subscribe(Event.AdapterAdded, HandleAddedAdapter);

            // some test data
            AdapterViews.Add(new AdapterModel("Test", "Content1"));
            AdapterViews.Add(new AdapterModel("Test2", "Content1"));
            AdapterViews.Add(new AdapterModel("Test3", "Content1"));

            TabsItemViewModels.Add(new AdapterVelocityTableViewModel("VelocityTable"));
            TabsItemViewModels.Add(new AdapterVideoPlayerViewModel("VideoPlayer"));
        }

        public ObservableCollection<AdapterModel> AdapterViews 
        { 
            get { return _adapterViews; }
            set { 
                _adapterViews = value;
                OnPropertyChanged();
            } 
        }

        public ObservableCollection<PropertyChangedNotifier> TabsItemViewModels
        { 
            get { return _tabItemsViewModel; } 
            set
            {
                _tabItemsViewModel = value;
                OnPropertyChanged();
            }
        }

        // TODO: remove datacontext from views
        // TODO: add some viewModels to list
        // TODO: 

        public async Task HandleAddedAdapter(EventContext eventContext)
        {
            string name = eventContext.Arg as string;
            var success = UIAdapterManager.Instance.TryGetUIAdapterByName(name, out Adapter.Interface.IUIAdapter adapter);
            if (success)
            {
               if(adapter is UICSVVelocityAdapter velocityAdapter)
                {
                    // to somthing
                    AdapterViews.Add(new AdapterModel(velocityAdapter.Name, "Content99"));

                    return;
                }
            }

        }
    }
}
