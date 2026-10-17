using System.Windows;
using System.Windows.Input;
using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;
using BarbellTracker.WPF_HelperClasses;
using Microsoft.Extensions.DependencyInjection;
using static BarbellTracker.ApplicationCode.IEventSystem;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private enum Page { Video, History, Plugins, Settings }

        private Page page = Page.Video;

        public MainWindowViewModel()
        {
            ClickCommandVideoMenu = new RelayCommand(() => true, () => Show(Page.Video));
            ClickCommandHistoryMenu = new RelayCommand(() => true, () => Show(Page.History));
            ClickCommandStartMenu = new RelayCommand(() => true, () => Show(Page.Plugins));
            ClickCommandSettingsMenu = new RelayCommand(() => true, () => Show(Page.Settings));

            // an opened session is shown on the video page
            var eventSystem = DependencyInjectionHelper.provider.GetRequiredService<IEventSystem>();
            EventDelegate<SessionOpened> openedDelegate = _ => RunOnUiThread(() => Show(Page.Video));
            eventSystem.Subscribe(openedDelegate);
        }

        public ICommand ClickCommandVideoMenu { get; }
        public ICommand ClickCommandHistoryMenu { get; }
        public ICommand ClickCommandStartMenu { get; }
        public ICommand ClickCommandSettingsMenu { get; }

        public bool IsVisibleVideoPage => page == Page.Video;
        public bool IsVisibleHistoryPage => page == Page.History;
        public bool IsVisiblePluginPage => page == Page.Plugins;
        public bool IsVisibleSettingsControl => page == Page.Settings;

        private void Show(Page newPage)
        {
            page = newPage;
            OnPropertyChanged(nameof(IsVisibleVideoPage));
            OnPropertyChanged(nameof(IsVisibleHistoryPage));
            OnPropertyChanged(nameof(IsVisiblePluginPage));
            OnPropertyChanged(nameof(IsVisibleSettingsControl));
        }

        private static void RunOnUiThread(System.Action action)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null || dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.Invoke(action);
            }
        }
    }
}
