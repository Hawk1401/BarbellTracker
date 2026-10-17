using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BarbellTracker.Adapter.Model;
using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;
using BarbellTracker.DomainCode;
using BarbellTracker.Services.Interface;
using BarbellTracker.Tracking;
using BarbellTracker.WPF_HelperClasses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using OxyPlot;
using static BarbellTracker.ApplicationCode.IEventSystem;

namespace BarbellTracker.WPF_DesktopClient.ViewModel
{
    /// <summary>
    /// Open a video, mark the barbell plate and a known length, track it and show the analysis of the set.
    /// The page also shows the analysis of data that was loaded by other trackers (e.g. the json loader),
    /// because it listens to the ExtracedVideoInfo event.
    /// </summary>
    internal class VideoAnalysisViewModel : ViewModelBase
    {
        private const string TrackerName = "VideoTracker";
        private const double DefaultPlateDiameterCm = 45;

        private readonly IEventSystem eventSystem;
        private readonly ICalculator<SetAnalysis> analyzer;
        private readonly ICalculator<PhysicalMotion> motionCalculator;

        private string videoPath;
        private VideoInfo videoInfo;
        private BitmapSource frameImage;
        private int currentFrame;
        private int frameLoadVersion;

        private bool isCalibrationTool;
        private bool isDragging;
        private Point dragStart;

        private bool hasRegion;
        private Rect region;
        private int regionFrame;
        private bool hasCalibration;
        private Point calibrationStart, calibrationEnd;
        private double calibrationCm = DefaultPlateDiameterCm;

        private bool isTracking;
        private double progress;
        private string status = "Öffne ein Video, um zu starten.";
        private CancellationTokenSource cancellation;

        private Vector2DPoint[] overlayPositions = Array.Empty<Vector2DPoint>();
        private int overlayStartFrame;
        private PointCollection pathPoints = new();
        private bool hasMarker;
        private double markerX, markerY;

        private bool hasResult;
        private string summary = string.Empty;
        private PlotModel velocityPlot;
        private PlotModel barPathPlot;
        private bool resultFromThisPage;
        private TrackedInformation shownInformation;
        private int recalculation;

        private readonly record struct Vector2DPoint(double X, double Y);

        public VideoAnalysisViewModel() : base("Video & Analyse")
        {
            var provider = DependencyInjectionHelper.provider;
            eventSystem = provider.GetRequiredService<IEventSystem>();
            analyzer = provider.GetRequiredService<ICalculator<SetAnalysis>>();
            motionCalculator = provider.GetRequiredService<ICalculator<PhysicalMotion>>();

            OpenVideoCommand = new RelayCommand(() => !IsTracking, OpenVideo);
            StartTrackingCommand = new RelayCommand(CanStartTracking, StartTracking);
            CancelTrackingCommand = new RelayCommand(() => IsTracking, () => cancellation?.Cancel());

            EventDelegate<ExtracedVideoInfo> resultDelegate = OnExtractedVideoInfo;
            eventSystem.Subscribe(resultDelegate);
            EventDelegate<AnalysisSettingsChanged> settingsDelegate = OnSettingsChanged;
            eventSystem.Subscribe(settingsDelegate);
        }

        public ICommand OpenVideoCommand { get; }
        public ICommand StartTrackingCommand { get; }
        public ICommand CancelTrackingCommand { get; }

        public ObservableCollection<Repetition> Repetitions { get; } = new();

        // ---------------- video + overlay ----------------

        public BitmapSource FrameImage
        {
            get => frameImage;
            private set => SetProperty(ref frameImage, value);
        }

        public double FrameWidth => videoInfo?.Width ?? 640;
        public double FrameHeight => videoInfo?.Height ?? 360;
        public bool HasVideo => videoInfo != null;
        public int MaxFrame => Math.Max(0, (videoInfo?.FrameCount ?? 1) - 1);

        /// <summary>Line width of the overlay, scaled with the video resolution so it is visible in a shrunken view</summary>
        public double OverlayThickness => Math.Max(2, FrameWidth / 250);

        public int CurrentFrame
        {
            get => currentFrame;
            set
            {
                if (SetProperty(ref currentFrame, value))
                {
                    OnPropertyChanged(nameof(FrameLabel));
                    UpdateMarker();
                    _ = LoadFrameAsync(value);
                }
            }
        }

        public string FrameLabel => videoInfo == null ? string.Empty : $"Frame {currentFrame} / {MaxFrame}  ({currentFrame / (double)videoInfo.FrameRate:F2} s)";

        public bool IsRegionTool
        {
            get => !isCalibrationTool;
            set { if (value) SetTool(false); }
        }

        public bool IsCalibrationTool
        {
            get => isCalibrationTool;
            set { if (value) SetTool(true); }
        }

        public string Instruction => !HasVideo
            ? "Öffne ein Video."
            : isCalibrationTool
                ? "Ziehe eine Linie über eine Strecke bekannter Länge (z. B. Durchmesser der Hantelscheibe) und gib die Länge in cm ein."
                : "Wähle mit dem Slider einen Frame, in dem die Stange startet, und ziehe ein Rechteck um die Hantelscheibe.";

        public bool HasRegion => hasRegion;
        public double RegionX => region.X;
        public double RegionY => region.Y;
        public double RegionWidth => region.Width;
        public double RegionHeight => region.Height;

        public bool HasCalibration => hasCalibration;
        public double CalibrationX1 => calibrationStart.X;
        public double CalibrationY1 => calibrationStart.Y;
        public double CalibrationX2 => calibrationEnd.X;
        public double CalibrationY2 => calibrationEnd.Y;

        public double CalibrationCm
        {
            get => calibrationCm;
            set
            {
                if (SetProperty(ref calibrationCm, value))
                {
                    OnPropertyChanged(nameof(ScaleText));
                }
            }
        }

        public double CalibrationLengthPixel => hasCalibration ? (calibrationEnd - calibrationStart).Length : 0;

        public double? PixelPerCm =>
            hasCalibration && CalibrationLengthPixel > 0 && calibrationCm > 0
                ? VideoTracker.CalculatePixelPerCm(CalibrationLengthPixel, calibrationCm)
                : null;

        public string ScaleText => PixelPerCm is double scale
            ? $"Maßstab: {scale:F2} px/cm ({CalibrationLengthPixel:F0} px = {calibrationCm:F1} cm)"
            : "Maßstab: noch nicht festgelegt";

        public PointCollection PathPoints
        {
            get => pathPoints;
            private set => SetProperty(ref pathPoints, value);
        }

        public bool HasPath => pathPoints.Count > 1;
        public bool HasMarker => hasMarker;
        public double MarkerX => markerX;
        public double MarkerY => markerY;

        // ---------------- tracking state ----------------

        public bool IsTracking
        {
            get => isTracking;
            private set
            {
                if (SetProperty(ref isTracking, value))
                {
                    OnPropertyChanged(nameof(IsNotTracking));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsNotTracking => !isTracking;

        public double Progress
        {
            get => progress;
            private set => SetProperty(ref progress, value);
        }

        public string Status
        {
            get => status;
            private set => SetProperty(ref status, value);
        }

        // ---------------- results ----------------

        public bool HasResult
        {
            get => hasResult;
            private set => SetProperty(ref hasResult, value);
        }

        public string Summary
        {
            get => summary;
            private set => SetProperty(ref summary, value);
        }

        public PlotModel VelocityPlot
        {
            get => velocityPlot;
            private set => SetProperty(ref velocityPlot, value);
        }

        public PlotModel BarPathPlot
        {
            get => barPathPlot;
            private set => SetProperty(ref barPathPlot, value);
        }

        // ---------------- mouse interaction (called by the view) ----------------

        public void BeginDrag(Point position)
        {
            if (!HasVideo || IsTracking)
            {
                return;
            }

            isDragging = true;
            dragStart = Clamp(position);
            UpdateDrag(position);
        }

        public void UpdateDrag(Point position)
        {
            if (!isDragging)
            {
                return;
            }

            var end = Clamp(position);
            if (isCalibrationTool)
            {
                calibrationStart = dragStart;
                calibrationEnd = end;
                hasCalibration = true;
                OnCalibrationChanged();
            }
            else
            {
                region = new Rect(dragStart, end);
                hasRegion = true;
                regionFrame = currentFrame;
                OnRegionChanged();
            }
        }

        public void EndDrag()
        {
            isDragging = false;
            if (!isCalibrationTool && hasRegion && (region.Width < 4 || region.Height < 4))
            {
                hasRegion = false; // a click is no region
                OnRegionChanged();
            }
            if (isCalibrationTool && hasCalibration && CalibrationLengthPixel < 4)
            {
                hasCalibration = false;
                OnCalibrationChanged();
            }
            CommandManager.InvalidateRequerySuggested();
        }

        private Point Clamp(Point p)
        {
            return new Point(Math.Clamp(p.X, 0, FrameWidth), Math.Clamp(p.Y, 0, FrameHeight));
        }

        private void SetTool(bool calibration)
        {
            isCalibrationTool = calibration;
            OnPropertyChanged(nameof(IsRegionTool));
            OnPropertyChanged(nameof(IsCalibrationTool));
            OnPropertyChanged(nameof(Instruction));
        }

        private void OnRegionChanged()
        {
            OnPropertyChanged(nameof(HasRegion));
            OnPropertyChanged(nameof(RegionX));
            OnPropertyChanged(nameof(RegionY));
            OnPropertyChanged(nameof(RegionWidth));
            OnPropertyChanged(nameof(RegionHeight));
        }

        private void OnCalibrationChanged()
        {
            OnPropertyChanged(nameof(HasCalibration));
            OnPropertyChanged(nameof(CalibrationX1));
            OnPropertyChanged(nameof(CalibrationY1));
            OnPropertyChanged(nameof(CalibrationX2));
            OnPropertyChanged(nameof(CalibrationY2));
            OnPropertyChanged(nameof(ScaleText));
        }

        // ---------------- video ----------------

        private void OpenVideo()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Video auswählen",
                Filter = "Videos|*.mp4;*.mov;*.avi;*.mkv;*.m4v|Alle Dateien|*.*",
            };
            if (dialog.ShowDialog() == true)
            {
                LoadVideo(dialog.FileName);
            }
        }

        public void LoadVideo(string path)
        {
            try
            {
                videoInfo = VideoTracker.GetInfo(path);
                if (videoInfo.FrameCount <= 0 || videoInfo.FrameRate <= 0)
                {
                    throw new InvalidDataException("Das Video enthält keine lesbaren Frames.");
                }
                videoPath = path;
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidDataException || ex is UnauthorizedAccessException || ex is OpenCvSharp.OpenCVException)
            {
                videoInfo = null;
                Status = $"Das Video konnte nicht geöffnet werden: {ex.Message}";
                OnVideoChanged();
                return;
            }

            hasRegion = false;
            hasCalibration = false;
            overlayPositions = Array.Empty<Vector2DPoint>();
            PathPoints = new PointCollection();
            hasMarker = false;
            OnRegionChanged();
            OnCalibrationChanged();
            OnVideoChanged();
            OnPropertyChanged(nameof(HasPath));
            OnPropertyChanged(nameof(HasMarker));

            currentFrame = 0;
            OnPropertyChanged(nameof(CurrentFrame));
            OnPropertyChanged(nameof(FrameLabel));
            _ = LoadFrameAsync(0);
            Status = $"{Path.GetFileName(path)}: {videoInfo.Width}x{videoInfo.Height}, {videoInfo.FrameRate} fps, {videoInfo.FrameCount} Frames. Markiere die Hantelscheibe und den Maßstab.";
        }

        private void OnVideoChanged()
        {
            OnPropertyChanged(nameof(HasVideo));
            OnPropertyChanged(nameof(FrameWidth));
            OnPropertyChanged(nameof(FrameHeight));
            OnPropertyChanged(nameof(MaxFrame));
            OnPropertyChanged(nameof(OverlayThickness));
            OnPropertyChanged(nameof(Instruction));
            CommandManager.InvalidateRequerySuggested();
        }

        private async Task LoadFrameAsync(int index)
        {
            if (videoPath == null)
            {
                return;
            }

            var version = ++frameLoadVersion;
            var path = videoPath;
            try
            {
                var png = await Task.Run(() => VideoTracker.ReadFramePng(path, index));
                if (version != frameLoadVersion)
                {
                    return; // a newer frame was requested in the meantime
                }

                var bitmap = new BitmapImage();
                using (var stream = new MemoryStream(png))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                }
                bitmap.Freeze();
                FrameImage = bitmap;
            }
            catch (Exception ex) when (ex is IOException || ex is ArgumentOutOfRangeException || ex is OpenCvSharp.OpenCVException)
            {
                Status = $"Frame {index} konnte nicht gelesen werden: {ex.Message}";
            }
        }

        // ---------------- tracking ----------------

        private bool CanStartTracking()
        {
            return HasVideo && hasRegion && PixelPerCm != null && !IsTracking;
        }

        private async void StartTracking()
        {
            if (!CanStartTracking())
            {
                return;
            }

            var path = videoPath;
            var trackingRegion = new TrackingRegion((int)region.X, (int)region.Y, (int)region.Width, (int)region.Height);
            var scale = PixelPerCm.Value;
            var startFrame = regionFrame;
            var name = Path.GetFileNameWithoutExtension(path);

            cancellation = new CancellationTokenSource();
            var token = cancellation.Token;
            var progressReporter = new Progress<double>(p => Progress = p * 100);

            IsTracking = true;
            Progress = 0;
            HasResult = false;
            Status = "Tracking läuft …";

            try
            {
                var result = await Task.Run(() =>
                    new VideoTracker().Track(path, trackingRegion, scale, name, progressReporter, token, startFrame), token);

                ShowTrackingOverlay(result.Information, startFrame);
                Status = result.IsComplete
                    ? $"Tracking abgeschlossen ({result.Information.Positions.Length} Frames)."
                    : $"Die Hantel wurde bei Frame {startFrame + result.LostAtFrame} verloren. Es werden nur die Frames davor ausgewertet. Wähle einen früheren Frame oder ein größeres Rechteck.";

                Publish(result.Information, name);
            }
            catch (OperationCanceledException)
            {
                Status = "Tracking abgebrochen.";
            }
            catch (Exception ex) when (ex is IOException || ex is ArgumentException || ex is InvalidDataException || ex is OpenCvSharp.OpenCVException)
            {
                Status = $"Tracking fehlgeschlagen: {ex.Message}";
            }
            finally
            {
                cancellation.Dispose();
                cancellation = null;
                IsTracking = false;
            }
        }

        /// <summary>Hands the result to the rest of the application, so the processing plugins (csv files, tables) run as well</summary>
        private void Publish(TrackedInformation info, string name)
        {
            resultFromThisPage = true;
            try
            {
                // the FileManager needs a started extraction to know where to write the files
                eventSystem.Fire(new StartExtractVideoInfo()
                {
                    StartExtractionInformation = new StartExtractionInformation() { PluginName = TrackerName, ExtractionName = name }
                });
                eventSystem.Fire(new ExtracedVideoInfo() { trackedInformation = info });
            }
            catch (Exception ex) when (ex is System.Reflection.TargetInvocationException || ex is IOException)
            {
                Status += $" Ein Plugin ist fehlgeschlagen: {ex.InnerException?.Message ?? ex.Message}";
            }
            finally
            {
                resultFromThisPage = false;
            }
        }

        private void ShowTrackingOverlay(TrackedInformation info, int startFrame)
        {
            overlayPositions = info.Positions.Select(p => new Vector2DPoint(p.X, p.Y)).ToArray();
            overlayStartFrame = startFrame;
            PathPoints = new PointCollection(overlayPositions.Select(p => new Point(p.X, p.Y)));
            OnPropertyChanged(nameof(HasPath));
            UpdateMarker();
        }

        private void UpdateMarker()
        {
            var index = currentFrame - overlayStartFrame;
            hasMarker = overlayPositions.Length > 0 && index >= 0 && index < overlayPositions.Length;
            if (hasMarker)
            {
                markerX = overlayPositions[index].X;
                markerY = overlayPositions[index].Y;
            }
            OnPropertyChanged(nameof(HasMarker));
            OnPropertyChanged(nameof(MarkerX));
            OnPropertyChanged(nameof(MarkerY));
        }

        // ---------------- results ----------------

        private void OnExtractedVideoInfo(ExtracedVideoInfo extracted)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(() => OnExtractedVideoInfo(extracted));
                return;
            }

            if (!resultFromThisPage)
            {
                // data from another tracker (json): the overlay of the video does not belong to it
                overlayPositions = Array.Empty<Vector2DPoint>();
                PathPoints = new PointCollection();
                OnPropertyChanged(nameof(HasPath));
                UpdateMarker();
            }

            ShowResult(extracted.trackedInformation);
        }

        private void OnSettingsChanged(AnalysisSettingsChanged changed)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(() => OnSettingsChanged(changed));
                return;
            }

            if (shownInformation == null)
            {
                return;
            }

            // the services cache by id, so the same data needs a new id to be calculated with the new settings
            ShowResult(new TrackedInformation()
            {
                Id = $"{shownInformation.Id}~{++recalculation}",
                Name = shownInformation.Name,
                Positions = shownInformation.Positions,
                FrameRate = shownInformation.FrameRate,
                PixelPerCm = shownInformation.PixelPerCm,
            }, remember: false);
        }

        public void ShowResult(TrackedInformation info, bool remember = true)
        {
            if (remember)
            {
                shownInformation = info;
            }

            try
            {
                var motion = motionCalculator.GetCalculatedValue(info);
                var analysis = analyzer.GetCalculatedValue(info);

                Repetitions.Clear();
                foreach (var rep in analysis.Repetitions)
                {
                    Repetitions.Add(rep);
                }

                VelocityPlot = PlotModelFactory.CreateVelocityPlot(motion, analysis);
                BarPathPlot = PlotModelFactory.CreateBarPathPlot(motion);
                Summary = analysis.Repetitions.Count == 0
                    ? $"{info.Name}: keine Wiederholung erkannt (siehe Einstellungen: Mindestbewegung)."
                    : $"{info.Name}: {analysis.Repetitions.Count} Wiederholungen, Geschwindigkeitsverlust {analysis.VelocityLossPercent:F1} %";
                HasResult = true;
            }
            catch (ArgumentException ex)
            {
                HasResult = false;
                Status = $"Die Daten konnten nicht ausgewertet werden: {ex.Message}";
            }
        }
    }
}
