using BarbellTracker.WPF_HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    abstract class ViewModelBase :  PropertyChangedNotifier
    {
        private string _name;

        public ViewModelBase(string name)
        {
            Name = name;
        }

        public ViewModelBase()
        {

        }

        public string Name 
        { 
            get { return _name; }
            set 
            { 
                _name = value; 
                OnPropertyChanged();
            }

        }
    }
}
