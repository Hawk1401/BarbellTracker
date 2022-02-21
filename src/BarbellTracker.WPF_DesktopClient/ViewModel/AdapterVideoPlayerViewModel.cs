using BarbellTracker.WPF_HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    internal class AdapterVideoPlayerViewModel : PropertyChangedNotifier
    {
        private string _url;
        public AdapterVideoPlayerViewModel()
        {
            // some test data
            URL = @"C:\Users\jmetzger\source\repos\BarbellTracker\src\BarbellTracker.WPF_DesktopClient\file_example_MP4_640_3MG.mp4";
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
