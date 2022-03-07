using BarbellTracker.Adapter;
using BarbellTracker.Adapter.Model;
using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;
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
        private ObservableCollection<ViewModelBase> _tabItemsViewModel = new();
        private IEventSystem _eventSystem;
        private UIAdapterManager _adapterManager;

        public AdapterControlViewModel(IEventSystem eventSystem, UIAdapterManager uIAdapterManager)
        {
            this._eventSystem = eventSystem;
            this._adapterManager = uIAdapterManager;

            IEventSystem.EventDelegate<AdapterAdded> eventDelegate = HandleAddedAdapter;

            eventSystem.Subscribe(eventDelegate);
            //EventSystem.Subscribe(Event.AdapterAdded, HandleAddedAdapter);

            /*
            // some test data
            TabsItemViewModels.Add(new AdapterVelocityTableViewModel("VelocityTable"));
            TabsItemViewModels.Add(new AdapterVideoPlayerViewModel("VideoPlayer"));
            */
        }

        public ObservableCollection<ViewModelBase> TabsItemViewModels
        { 
            get { return _tabItemsViewModel; } 
            set
            {
                _tabItemsViewModel = value;
                OnPropertyChanged();
            }
        }

        public void HandleAddedAdapter(AdapterAdded AdapterAdded)
        {
            string name = AdapterAdded.AdapterName;
            var success = _adapterManager.TryGetUIAdapterByName(name, out Adapter.Interface.IUIAdapter adapter);
            if (success)
            {
                if(adapter is UICSVVectorAdapter vectorAdapter)
                {
                    TabsItemViewModels.Add(new AdapterVelocityTableViewModel(vectorAdapter.Name));
                    return;
                }
                if(adapter is UIVideoAdapter videoAdapter)
                {
                    TabsItemViewModels.Add(new AdapterVideoPlayerViewModel(videoAdapter.Name));
                    return;
                }
            }

        }
    }
}
