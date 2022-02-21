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

        }

        public ObservableCollection<CSVVelocityModel> CSVVelocityModels
        {
            get { return _csvVelocityModels; }
            set
            {
                _csvVelocityModels = value;
                OnPropertyChanged("CSVVelocityModels");
            }
        }
    }
}
