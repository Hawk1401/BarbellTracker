using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.WPF_DesktopClient.DataStructures
{
    internal class AdapterModel : WPF_HelperClasses.PropertyChangedNotifier
    {
        private string _adapterName;


        public AdapterModel(string name)
        {
            this.AdapterName = name;
        }

        public string AdapterName 
        { 
            get { return _adapterName; } 
            set 
            { 
                this._adapterName = value;
                OnPropertyChanged("AdapterName");
            } 
        }
    }
}
