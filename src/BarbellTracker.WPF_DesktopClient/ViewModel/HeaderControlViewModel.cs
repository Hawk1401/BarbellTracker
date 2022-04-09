using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using BarbellTracker.Adapter;
using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;
using BarbellTracker.DomainCode;
using BarbellTracker.WPF_HelperClasses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using static BarbellTracker.ApplicationCode.IEventSystem;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    internal class HeaderControlViewModel : ViewModelBase
    {
        private bool _startEnabled = true;
        private bool _abortEnabled;

        private IEventSystem eventSystem;
        private PluginManager pluginManager;

        public HeaderControlViewModel()
        {
            ClickCommandStartButton = new RelayCommand(IsStartEnabled, StartAnalysis);
            ClickCommandAbortButton = new RelayCommand(IsAbortEnabled, AbortAnalysis);

            this.eventSystem = DependencyInjectionHelper.provider.GetRequiredService<IEventSystem>();
            this.pluginManager = DependencyInjectionHelper.provider.GetRequiredService<PluginManager>();
        }

        public ICommand ClickCommandStartButton { get; }
        public ICommand ClickCommandAbortButton { get; }

        private void StartAnalysis()
        {
            _startEnabled = false;
            _abortEnabled = true;


            var plugin = pluginManager.GetTrackerPlugins().First();
            StartExtractionInformation startExtractionInformation = new StartExtractionInformation()
            {
                ExtractionName = "FristExtration",
                PluginName = plugin.Name
            };

            EventDelegate<SelectFile> handelFileDelegate = handelFile;
            eventSystem.Subscribe(handelFileDelegate);

            eventSystem.Fire(new StartExtractVideoInfo() { StartExtractionInformation = startExtractionInformation });

            // Nimm Processing Plugin in liste
            // und File Dialog -> wähle JSON File (Whatsapp von Flo)
        }

        public void handelFile(SelectFile SelectFile)
        {
            OpenFileDialog oFD = new OpenFileDialog();
            bool? result = oFD.ShowDialog();

            if(result == true)
            {
                eventSystem.Fire(new FileSelected() { FilePath = oFD.FileName });
            }
            
        }

        private bool IsStartEnabled()
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
