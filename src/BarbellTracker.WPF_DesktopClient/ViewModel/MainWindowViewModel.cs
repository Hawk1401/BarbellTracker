using BarbellTracker.WPF_HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private bool _isVisibleSettingsControl;
        private bool _isVisiblePluginPage = true;

        public MainWindowViewModel()
        {
            ClickCommandStartMenu = new RelayCommand(() => true, StartMenu);
            ClickCommandSettingsMenu = new RelayCommand(() => true, SettingsMenu);
        }

        public ICommand ClickCommandStartMenu { get; }
        public ICommand ClickCommandSettingsMenu { get; }


        public bool IsVisibleSettingsControl
        {
            get
            {
                return _isVisibleSettingsControl;
            }
            set
            {
                if(SetProperty(ref _isVisibleSettingsControl, value))
                {
                    _isVisibleSettingsControl = value;
                }
                
            }
        }
        public bool IsVisiblePluginPage
        {
            get
            {
                return _isVisiblePluginPage;
            }
            set
            {
                if(SetProperty(ref _isVisiblePluginPage, value))
                {
                    _isVisiblePluginPage = value;
                }
            }
        }

        public void StartMenu()
        {
            IsVisiblePluginPage = true;
            IsVisibleSettingsControl = false;
        }
        public void SettingsMenu()
        {
            IsVisibleSettingsControl = true;
            IsVisiblePluginPage = false;
        }

    }
}
