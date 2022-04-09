using BarbellTracker.Adapter;
using BarbellTracker.Adapter.Model;
using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    internal class AdapterVelocityTableViewModel : ViewModelBase
    {
        private ObservableCollection<VectorCSVItem> _csvVelocityModels = new ObservableCollection<VectorCSVItem>();

        public AdapterVelocityTableViewModel(string name) : base(name)
        {
            var uiAdapterManager = DependencyInjectionHelper.provider.GetRequiredService<UIAdapterManager>();

            var success = uiAdapterManager.TryGetUIAdapterByName(name, out Adapter.Interface.IUIAdapter adapter);
            if (success)
            {
                if (adapter is UICSVVectorAdapter velocityAdapter)
                {

                    foreach (var item in velocityAdapter.CSV.GetTable())
                    {
                        CSVVelocityModels.Add(item);
                    }
                }

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


    }
}
