using System;
using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;
using BarbellTracker.Services.Implementation;
using BarbellTracker.WPF_HelperClasses;
using Microsoft.Extensions.DependencyInjection;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    internal class SettingsControlViewModel : ViewModelBase
    {
        private readonly AnalysisSettings settings;
        private readonly IEventSystem eventSystem;

        public SettingsControlViewModel()
        {
            var provider = DependencyInjectionHelper.provider;
            settings = provider.GetRequiredService<AnalysisSettings>();
            eventSystem = provider.GetRequiredService<IEventSystem>();
        }

        /// <summary>The allowed windows of the position smoothing, 0 = off</summary>
        public int[] SmoothingWindows { get; } = { 0, 5, 7, 9 };

        public int SmoothingWindow
        {
            get => settings.SmoothingWindow;
            set
            {
                if (settings.SmoothingWindow != value)
                {
                    settings.SmoothingWindow = value;
                    OnPropertyChanged();
                    eventSystem.Fire(new AnalysisSettingsChanged());
                }
            }
        }

        /// <summary>Minimal vertical movement that counts as repetition, shown in cm</summary>
        public double MinRangeOfMotionCm
        {
            get => settings.MinRangeOfMotion * 100;
            set
            {
                // ignore values that make no sense instead of breaking the analysis
                if (value < 1 || value > 500 || Math.Abs(settings.MinRangeOfMotion * 100 - value) < 1e-9)
                {
                    return;
                }
                settings.MinRangeOfMotion = value / 100;
                OnPropertyChanged();
                eventSystem.Fire(new AnalysisSettingsChanged());
            }
        }
    }
}
