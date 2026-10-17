using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;
using BarbellTracker.Services.Implementation;
using BarbellTracker.WPF_HelperClasses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    internal class SettingsControlViewModel : ViewModelBase
    {
        private readonly AnalysisSettings analysisSettings;
        private readonly UserSettings userSettings;
        private readonly JsonSettingsStore<UserSettings> store;
        private readonly FileManager fileManager;
        private readonly IEventSystem eventSystem;
        private string saveError = string.Empty;

        public SettingsControlViewModel()
        {
            var provider = DependencyInjectionHelper.provider;
            analysisSettings = provider.GetRequiredService<AnalysisSettings>();
            userSettings = provider.GetRequiredService<UserSettings>();
            store = provider.GetRequiredService<JsonSettingsStore<UserSettings>>();
            fileManager = provider.GetRequiredService<FileManager>();
            eventSystem = provider.GetRequiredService<IEventSystem>();

            ChooseOutputFolderCommand = new RelayCommand(() => true, ChooseOutputFolder);
            ResetOutputFolderCommand = new RelayCommand(() => true, () => OutputFolder = string.Empty);
            OpenLogFolderCommand = new RelayCommand(() => true, () => OpenFolder(AppPaths.LogsFolder));
            OpenDataFolderCommand = new RelayCommand(() => true, () => OpenFolder(AppPaths.DataFolder));
        }

        public ICommand ChooseOutputFolderCommand { get; }
        public ICommand ResetOutputFolderCommand { get; }
        public ICommand OpenLogFolderCommand { get; }
        public ICommand OpenDataFolderCommand { get; }

        public string VersionText => $"BarbellTracker {AppLogging.Version}";

        private void OpenFolder(string folder)
        {
            try
            {
                Directory.CreateDirectory(folder);
                Process.Start(new ProcessStartInfo("explorer.exe", $"\"{folder}\"") { UseShellExecute = true });
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is System.ComponentModel.Win32Exception)
            {
                SaveError = $"Der Ordner konnte nicht geöffnet werden: {ex.Message}";
            }
        }

        /// <summary>The allowed windows of the position smoothing, 0 = off</summary>
        public int[] SmoothingWindows { get; } = { 0, 5, 7, 9 };

        public int SmoothingWindow
        {
            get => analysisSettings.SmoothingWindow;
            set
            {
                if (analysisSettings.SmoothingWindow != value)
                {
                    analysisSettings.SmoothingWindow = value;
                    OnPropertyChanged();
                    SaveAndNotify();
                }
            }
        }

        /// <summary>Minimal vertical movement that counts as repetition, shown in cm</summary>
        public double MinRangeOfMotionCm
        {
            get => analysisSettings.MinRangeOfMotion * 100;
            set
            {
                // ignore values that make no sense instead of breaking the analysis
                if (value < 1 || value > 500 || Math.Abs(analysisSettings.MinRangeOfMotion * 100 - value) < 1e-9)
                {
                    return;
                }
                analysisSettings.MinRangeOfMotion = value / 100;
                OnPropertyChanged();
                SaveAndNotify();
            }
        }

        /// <summary>Empty = the default folder</summary>
        public string OutputFolder
        {
            get => userSettings.OutputFolder;
            set
            {
                value ??= string.Empty;
                if (userSettings.OutputFolder == value)
                {
                    return;
                }
                userSettings.OutputFolder = value;
                fileManager.FolderPath = string.IsNullOrWhiteSpace(value) ? AppPaths.DefaultExportFolder : value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EffectiveOutputFolder));
                SaveAndNotify(analysisChanged: false);
            }
        }

        public string EffectiveOutputFolder => fileManager.FolderPath;

        public string SaveError
        {
            get => saveError;
            private set => SetProperty(ref saveError, value);
        }

        private void ChooseOutputFolder()
        {
            var dialog = new OpenFolderDialog { Title = "Ausgabeordner für die CSV-Dateien" };
            if (dialog.ShowDialog() == true)
            {
                OutputFolder = dialog.FolderName;
            }
        }

        private void SaveAndNotify(bool analysisChanged = true)
        {
            userSettings.SmoothingWindow = analysisSettings.SmoothingWindow;
            userSettings.MinRangeOfMotion = analysisSettings.MinRangeOfMotion;
            SaveError = store.Save(userSettings) ? string.Empty : "Die Einstellungen konnten nicht gespeichert werden.";

            if (analysisChanged)
            {
                eventSystem.Fire(new AnalysisSettingsChanged());
            }
        }
    }
}
