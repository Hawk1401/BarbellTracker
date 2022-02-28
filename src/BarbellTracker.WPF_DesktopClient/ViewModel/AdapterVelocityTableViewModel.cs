using BarbellTracker.Adapter;
using BarbellTracker.Adapter.Model;
using BarbellTracker.ApplicationCode;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    internal class AdapterVelocityTableViewModel : ViewModelBase
    {
        private ObservableCollection<VectorCSVModel> _csvVelocityModels = new ObservableCollection<VectorCSVModel>();

        public AdapterVelocityTableViewModel(string name) : base(name)
        {
            EventSystem.Subscribe(Event.ExtracedVideoInfo, HandleTrakingInformation);

            // some test data
            /*
            var testTable = new VectorCSVModel();
            testTable.AddItem("456", 5, "38");
            testTable.AddItem("483", 2, "88");
            testTable.AddItem("789", 9, "55");
            foreach (var item in testTable.GetTable())
            {
                CSVVelocityModels.Add(item);
            }
            */
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

        public async Task HandleTrakingInformation(EventContext eventContext)
        {
            var name = eventContext.Arg as string;
            var success = UIAdapterManager.Instance.TryGetUIAdapterByName(name, out Adapter.Interface.IUIAdapter adapter);
            if (success)
            {
                if(adapter is UICSVVelocityAdapter velocityAdapter)
                {
                    var velocityMetaData = velocityAdapter.Table;
                    foreach (var item in velocityMetaData)
                    {
                        CSVVelocityModels.Add(item);
                    }
                }
            }
        }


    }
}
