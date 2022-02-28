using BarbellTracker.Adapter;
using BarbellTracker.Adapter.Model;
using BarbellTracker.ApplicationCode;
using BarbellTracker.WPF_HelperClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    internal class AdapterVelocityTableViewModel : PropertyChangedNotifier
    {
        private ObservableCollection<VectorCSVModel> _csvVelocityModels = new ObservableCollection<VectorCSVModel>();

        public AdapterVelocityTableViewModel()
        {
            EventSystem.Subscribe(Event.AdapterAdded, HandleAddedAdapter);

            // some test data
            //CSVVelocityModels.Add(new CSVVelocityModel("123", 5, "99"));
            //CSVVelocityModels.Add(new CSVVelocityModel("456", 5, "88"));
            //CSVVelocityModels.Add(new CSVVelocityModel("789", 5, "55"));

        }

        public ObservableCollection<VectorCSVModel> CSVVelocityModels
        {
            get { return _csvVelocityModels; }
            set
            {
                _csvVelocityModels = value;
                OnPropertyChanged();
            }
        }

        public async Task HandleAddedAdapter(EventContext eventContext)
        {
            string name = eventContext.Arg as string;
            var success = UIAdapterManager.Instance.TryGetUIAdapterByName(name, out Adapter.Interface.IUIAdapter adapter);
            if (success)
            {
                if (adapter is UICSVVelocityAdapter velocityAdapter)
                {
                    // to somthing
                    //foreach (CSVVelocityModel cSVVelocityModel in velocityAdapter.Table)
                    //{
                    //    CSVVelocityModels.Add(cSVVelocityModel);
                    //}

                    return;
                }
            }

        }
    }
}
