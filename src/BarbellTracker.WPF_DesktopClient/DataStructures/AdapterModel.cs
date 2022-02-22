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
        private string _content;


        public AdapterModel(string name, string content)
        {
            this.AdapterName = name;
            this.Content = content;
        }

        public string AdapterName 
        { 
            get { return _adapterName; } 
            set 
            { 
                this._adapterName = value;
                OnPropertyChanged();
            } 
        }

        public string Content 
        { 
            get { return _content; }
            set { _content = value; OnPropertyChanged(); }
        }
    }
}
