using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;
using BarbellTracker.DomainCode;
using BarbellTracker.Services.Implementation;
using BarbellTracker.WPF_HelperClasses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Serilog;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using static BarbellTracker.ApplicationCode.IEventSystem;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    /// <summary>The saved sessions: open, delete, export and the progress of the velocity per exercise</summary>
    internal class HistoryViewModel : ViewModelBase
    {
        private const string AllExercises = "Alle Übungen";

        private readonly ISessionRepository repository;
        private readonly IEventSystem eventSystem;
        private readonly SetCSVTranslater csvTranslater;

        private IReadOnlyList<Session> allSessions = Array.Empty<Session>();
        private string selectedExercise = AllExercises;
        private Session selectedSession;
        private PlotModel trendPlot = new PlotModel();
        private string status = string.Empty;

        public HistoryViewModel() : base("Verlauf")
        {
            var provider = DependencyInjectionHelper.provider;
            repository = provider.GetRequiredService<ISessionRepository>();
            eventSystem = provider.GetRequiredService<IEventSystem>();
            csvTranslater = provider.GetRequiredService<SetCSVTranslater>();

            RefreshCommand = new RelayCommand(() => true, Refresh);
            OpenCommand = new RelayCommand(() => SelectedSession != null, Open);
            DeleteCommand = new RelayCommand(() => SelectedSession != null, Delete);
            ExportCsvCommand = new RelayCommand(() => SelectedSession != null, ExportCsv);
            ExportJsonCommand = new RelayCommand(() => SelectedSession != null, ExportJson);

            EventDelegate<SessionSaved> savedDelegate = _ =>
            {
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher != null && !dispatcher.CheckAccess())
                {
                    dispatcher.Invoke(Refresh);
                }
                else
                {
                    Refresh();
                }
            };
            eventSystem.Subscribe(savedDelegate);

            Refresh();
        }

        public ICommand RefreshCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ExportCsvCommand { get; }
        public ICommand ExportJsonCommand { get; }

        public ObservableCollection<Session> Sessions { get; } = new();
        public ObservableCollection<string> Exercises { get; } = new() { AllExercises };

        public string SelectedExercise
        {
            get => selectedExercise;
            set
            {
                if (SetProperty(ref selectedExercise, value ?? AllExercises))
                {
                    ApplyFilter();
                }
            }
        }

        public Session SelectedSession
        {
            get => selectedSession;
            set
            {
                if (SetProperty(ref selectedSession, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public PlotModel TrendPlot
        {
            get => trendPlot;
            private set => SetProperty(ref trendPlot, value);
        }

        public string Status
        {
            get => status;
            private set => SetProperty(ref status, value);
        }

        public void Refresh()
        {
            try
            {
                allSessions = repository.GetAll();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                allSessions = Array.Empty<Session>();
                Status = $"Die Sessions konnten nicht gelesen werden: {ex.Message}";
                ApplyFilter();
                return;
            }

            var previous = selectedExercise;
            Exercises.Clear();
            Exercises.Add(AllExercises);
            foreach (var exercise in allSessions.Select(s => s.Exercise).Where(e => !string.IsNullOrWhiteSpace(e)).Distinct().OrderBy(e => e))
            {
                Exercises.Add(exercise);
            }

            selectedExercise = Exercises.Contains(previous) ? previous : AllExercises;
            OnPropertyChanged(nameof(SelectedExercise));
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var selectedId = SelectedSession?.Id;
            var filtered = selectedExercise == AllExercises
                ? allSessions
                : allSessions.Where(s => s.Exercise == selectedExercise).ToList();

            Sessions.Clear();
            foreach (var session in filtered)
            {
                Sessions.Add(session);
            }
            SelectedSession = Sessions.FirstOrDefault(s => s.Id == selectedId);

            TrendPlot = CreateTrendPlot(filtered);
            Status = Sessions.Count == 0 ? "Noch keine gespeicherten Sessions." : $"{Sessions.Count} Session(s)";
        }

        /// <summary>The best mean velocity of every session over time, one line per exercise</summary>
        private static PlotModel CreateTrendPlot(IEnumerable<Session> sessions)
        {
            var model = new PlotModel { Title = "Beste mittlere Geschwindigkeit je Session" };
            model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Title = "Datum", StringFormat = "dd.MM.yy" });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "m/s", Minimum = 0 });
            model.Legends.Add(new OxyPlot.Legends.Legend { LegendPosition = OxyPlot.Legends.LegendPosition.BottomRight });

            foreach (var group in sessions.Where(s => s.RepetitionCount > 0).GroupBy(s => string.IsNullOrWhiteSpace(s.Exercise) ? "(ohne Übung)" : s.Exercise))
            {
                var series = new LineSeries
                {
                    Title = group.Key,
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 4,
                    StrokeThickness = 2,
                    TrackerFormatString = "{0}\n{2:dd.MM.yyyy HH:mm}\n{4:0.00} m/s",
                };
                foreach (var session in group.OrderBy(s => s.CreatedAt))
                {
                    series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(session.CreatedAt), session.BestMeanVelocity));
                }
                model.Series.Add(series);
            }
            return model;
        }

        private void Open()
        {
            if (selectedSession == null)
            {
                return;
            }

            // read it again, the list only has what was there at the last refresh
            if (!repository.TryGet(selectedSession.Id, out var session))
            {
                Status = "Die Session existiert nicht mehr.";
                Refresh();
                return;
            }
            eventSystem.Fire(new SessionOpened() { Session = session });
        }

        private void Delete()
        {
            if (selectedSession == null)
            {
                return;
            }

            var answer = MessageBox.Show($"Session '{selectedSession.Name}' wirklich löschen?", "Session löschen",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (answer != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                repository.Delete(selectedSession.Id);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                Log.Error(ex, "Deleting session {Id} failed", selectedSession.Id);
                Status = $"Löschen fehlgeschlagen: {ex.Message}";
                return;
            }
            Refresh();
        }

        private void ExportCsv()
        {
            Export("csv", "CSV-Datei|*.csv", session =>
                csvTranslater.GetCSV(session.Tracked.WithNewId()).ToString());
        }

        private void ExportJson()
        {
            // the format the json loader reads, so the data can be imported again
            Export("json", "JSON-Datei|*.json", session =>
                JsonSerializer.Serialize(session.Tracked, new JsonSerializerOptions { WriteIndented = true }));
        }

        private void Export(string extension, string filter, Func<Session, string> createContent)
        {
            if (selectedSession == null)
            {
                return;
            }

            var dialog = new SaveFileDialog
            {
                Title = "Exportieren",
                Filter = filter,
                DefaultExt = extension,
                FileName = MakeFileName(selectedSession.Name),
            };
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                File.WriteAllText(dialog.FileName, createContent(selectedSession));
                Log.Information("Session {Id} exported to {File}", selectedSession.Id, dialog.FileName);
                Status = $"Exportiert nach {dialog.FileName}";
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException)
            {
                Log.Error(ex, "Export of session {Id} failed", selectedSession.Id);
                Status = $"Export fehlgeschlagen: {ex.Message}";
            }
        }

        private static string MakeFileName(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var clean = new string((name ?? "session").Select(c => invalid.Contains(c) ? '_' : c).ToArray());
            return string.IsNullOrWhiteSpace(clean) ? "session" : clean;
        }
    }
}
