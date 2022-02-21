using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BarbellTracker.WPF_DesktopClient.DataStructures
{
    internal class AdapterModel : WPF_HelperClasses.PropertyChangedNotifier
    {
        private string _adapterName;
        private UserControl _userControl; 

        public  AdapterModel(string name, UserControl userControl)
        {
            this.AdapterName = name;
            this.UserControl = userControl;
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
        public UserControl UserControl
        {
            get { return _userControl; }
            set 
            { 
                this._userControl = value; 
                OnPropertyChanged("UserControl");
            }
        }
    }
}
