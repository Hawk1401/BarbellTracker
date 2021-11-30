using System.Windows.Input;
using WPF_UI_HelperClasses;

namespace BarbellTracker.Plugin.WPF_MVVM_UI.ViewModel
{
    class HeaderControlViewModel
    {
        private bool _startEnabled = true;
        private bool _abortEnabled;

        public HeaderControlViewModel()
        {
            ClickCommandStartButton = new RelayCommand(IsStartEnabeld, StartAnalysis);
            ClickCommandAbortButton = new RelayCommand(IsAbortEnabled, AbortAnalysis);
        }

        public ICommand ClickCommandStartButton { get; }
        public ICommand ClickCommandAbortButton { get; }

        private void StartAnalysis()
        {
            _startEnabled = false;
            _abortEnabled = true;
        }

        private bool IsStartEnabeld()
        {
            return _startEnabled;
        }

        private void AbortAnalysis()
        {
            _abortEnabled = false;
            _startEnabled = true;
        }

        private bool IsAbortEnabled()
        {
            return _abortEnabled;
        }


    }
}
