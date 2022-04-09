using BarbellTracker.Adapter;
using BarbellTracker.Adapter.Model;
using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;
using BarbellTracker.WPF_DesktopClient.DataStructures;
using BarbellTracker.WPF_DesktopClient.View;
using BarbellTracker.WPF_HelperClasses;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static BarbellTracker.ApplicationCode.IEventSystem;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    internal class AdapterControlViewModel : ViewModelBase
    {
        private ObservableCollection<ViewModelBase> _tabItemsViewModel = new();
        private IEventSystem _eventSystem;
        private UIAdapterManager _adapterManager;

        private IEventSystem eventSystem;
        private UIAdapterManager uIAdapterManager;
        public AdapterControlViewModel()
        {
            eventSystem = DependencyInjectionHelper.provider.GetRequiredService<IEventSystem>();
            uIAdapterManager = DependencyInjectionHelper.provider.GetRequiredService<UIAdapterManager>();
            EventDelegate<AdapterAdded> del = HandleAddedAdapter;
            eventSystem.Subscribe(del);


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
            var success = uIAdapterManager.TryGetUIAdapterByName(name, out Adapter.Interface.IUIAdapter adapter);
            if (success)
            {
                if (adapter is UICSVVectorAdapter velocityAdapter)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate { TabsItemViewModels.Add(new AdapterVelocityTableViewModel(velocityAdapter.Name)); });
                    return;
                }
                if (adapter is UIVideoAdapter videoAdapter)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate { TabsItemViewModels.Add(new AdapterVideoPlayerViewModel(videoAdapter.Name)); });
                    return;
                }
            }

        }
    }
}
