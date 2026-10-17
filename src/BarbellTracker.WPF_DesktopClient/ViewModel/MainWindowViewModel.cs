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
        private bool _isVisiblePluginPage;
        private bool _isVisibleVideoPage = true;

        public MainWindowViewModel()
        {
            ClickCommandVideoMenu = new RelayCommand(() => true, VideoMenu);
            ClickCommandStartMenu = new RelayCommand(() => true, StartMenu);
            ClickCommandSettingsMenu = new RelayCommand(() => true, SettingsMenu);
        }

        public ICommand ClickCommandVideoMenu { get; }
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

        public bool IsVisibleVideoPage
        {
            get { return _isVisibleVideoPage; }
            set { SetProperty(ref _isVisibleVideoPage, value); }
        }

        public void VideoMenu()
        {
            IsVisibleVideoPage = true;
            IsVisiblePluginPage = false;
            IsVisibleSettingsControl = false;
        }

        public void StartMenu()
        {
            IsVisibleVideoPage = false;
            IsVisiblePluginPage = true;
            IsVisibleSettingsControl = false;
        }

        public void SettingsMenu()
        {
            IsVisibleVideoPage = false;
            IsVisibleSettingsControl = true;
            IsVisiblePluginPage = false;
        }

    }
}
