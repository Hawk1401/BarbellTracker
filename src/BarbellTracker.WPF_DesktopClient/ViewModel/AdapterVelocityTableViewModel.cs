using BarbellTracker.Adapter.Model;
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
        private ObservableCollection<CSVVelocityModel> _csvVelocityModels = new ObservableCollection<CSVVelocityModel>();

        public AdapterVelocityTableViewModel()
        {
            // some test data
            CSVVelocityModels.Add(new CSVVelocityModel("10:12Uhr", 1, "12, 8, 65"));
            CSVVelocityModels.Add(new CSVVelocityModel("10:13Uhr", 2, "16, 4, 5"));
            CSVVelocityModels.Add(new CSVVelocityModel("10:14Uhr", 99, "22, 43, 68"));
        }

        public ObservableCollection<CSVVelocityModel> CSVVelocityModels
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
