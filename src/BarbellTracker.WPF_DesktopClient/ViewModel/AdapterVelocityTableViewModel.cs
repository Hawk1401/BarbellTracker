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
using static BarbellTracker.Adapter.Model.VectorCSVModel;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    internal class AdapterVelocityTableViewModel : ViewModelBase
    {
        private ObservableCollection<VectorCSVItem> _csvVelocityModels = new ObservableCollection<VectorCSVItem>();

        public AdapterVelocityTableViewModel(string name) : base(name)
        {
            EventSystem.Subscribe(Event.AdapterAdded, HandleAddedAdapter);
            
            // some test data
            var testTable = new VectorCSVModel();
            testTable.AddItem("456", 5, "38");
            testTable.AddItem("483", 2, "88");
            testTable.AddItem("789", 9, "55");
            foreach (var item in testTable.GetTable())
            {
                CSVVelocityModels.Add(item);
            }
               
        }

        public ObservableCollection<VectorCSVItem> CSVVelocityModels
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
