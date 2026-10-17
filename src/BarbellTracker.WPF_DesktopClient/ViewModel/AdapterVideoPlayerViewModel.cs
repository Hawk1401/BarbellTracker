using BarbellTracker.WPF_HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    internal class AdapterVideoPlayerViewModel : ViewModelBase
    {
        private string _url;
        public AdapterVideoPlayerViewModel(string name) : base(name)
        {
        }

        public string URL 
        { 
            get { return _url; }
            set 
            { 
                _url = value;
                OnPropertyChanged();
            }
        }
    }
}
