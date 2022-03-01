using BarbellTracker.Adapter;
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
        private ObservableCollection<ViewModelBase> _tabItemsViewModel = new();


        public AdapterControlViewModel()
        {
            EventSystem.Subscribe(Event.AdapterAdded, HandleAddedAdapter);
            
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

        public async Task HandleAddedAdapter(EventContext eventContext)
        {
            string name = eventContext.Arg as string;
            //var success = UIAdapterManager.Instance.TryGetUIAdapterByName(name, out Adapter.Interface.IUIAdapter adapter);
            //if (success)
            //{
            //    if(adapter is UICSVVelocityAdapter velocityAdapter)
            //    {
            //        TabsItemViewModels.Add(new AdapterVelocityTableViewModel(velocityAdapter.Name));
            //        return;
            //    }
            //    if(adapter is UIVideoAdapter videoAdapter)
            //    {
            //        TabsItemViewModels.Add(new AdapterVideoPlayerViewModel(videoAdapter.Name));
            //        return;
            //    }
            //}

        }
    }
}
