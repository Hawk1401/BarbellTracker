using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using BarbellTracker.Adapter.Model;
using BarbellTracker.Services;
using BarbellTracker.Services.Implementation;
using BarbellTracker.Tracking;
using OpenCvSharp;
using Xunit;

namespace BarbellTracker.TrackingTests
{
    public class VideoTrackerTester : IDisposable
    {
        private const int Width = 640;
        private const int Height = 480;
        private const int Fps = 30;
        private const int Radius = 25;

        private readonly string folder = Path.Combine(Path.GetTempPath(), "BarbellTrackerTrackingTests-" + Guid.NewGuid());

        public VideoTrackerTester()
        {
            Directory.CreateDirectory(folder);
        }

        public void Dispose()
        {
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
        }

        /// <summary>Writes a video with a textured disc ("barbell plate") that moves along the given centers</summary>
        private string CreateVideo(IReadOnlyList<Point> centers)
        {
            var path = Path.Combine(folder, Guid.NewGuid() + ".avi");
            using var writer = new VideoWriter(path, FourCC.MJPG, Fps, new Size(Width, Height));
            Assert.True(writer.IsOpened(), "The test video could not be created");

            var rng = new Random(1);
            using var background = new Mat(Height, Width, MatType.CV_8UC3, new Scalar(90, 90, 90));
            for (int i = 0; i < 400; i++)
            {
                var c = new Scalar(60 + rng.Next(60), 60 + rng.Next(60), 60 + rng.Next(60));
                Cv2.Circle(background, new Point(rng.Next(Width), rng.Next(Height)), 2 + rng.Next(3), c, -1);
            }

            foreach (var center in centers)
            {
                using var frame = background.Clone();
                Cv2.Circle(frame, center, Radius, new Scalar(30, 30, 220), -1);
                Cv2.Circle(frame, center, Radius / 2, new Scalar(240, 240, 240), 3);
                Cv2.Line(frame, new Point(center.X - Radius, center.Y), new Point(center.X + Radius, center.Y), new Scalar(0, 255, 255), 3);
                Cv2.Line(frame, new Point(center.X, center.Y - Radius), new Point(center.X, center.Y + Radius), new Scalar(0, 255, 0), 3);
                writer.Write(frame);
            }
            return path;
        }

        /// <summary>Cosine shaped vertical movement of 300 px, one repetition per call: down then up</summary>
        private static void AddRepetition(List<Point> centers, int downFrames, int upFrames, int top, int bottom, int x)
        {
            for (int i = 1; i <= downFrames; i++)
            {
                centers.Add(new Point(x, (int)Math.Round(top + (bottom - top) * (1 - Math.Cos(Math.PI * i / downFrames)) / 2)));
            }
            for (int i = 1; i <= upFrames; i++)
            {
                centers.Add(new Point(x, (int)Math.Round(bottom + (top - bottom) * (1 - Math.Cos(Math.PI * i / upFrames)) / 2)));
            }
        }

        private static List<Point> TwoRepetitions()
        {
            var centers = new List<Point> { new Point(320, 80) };
            AddRepetition(centers, 40, 25, 80, 380, 320);
            AddRepetition(centers, 40, 30, 80, 380, 320);
            return centers;
        }

        private static TrackingRegion RegionAround(Point center) =>
            new TrackingRegion(center.X - Radius - 5, center.Y - Radius - 5, 2 * Radius + 10, 2 * Radius + 10);

        [Fact]
        public void Track_MovingDisc_FollowsTheDisc()
        {
            var centers = TwoRepetitions();
            var video = CreateVideo(centers);

            var result = new VideoTracker().Track(video, RegionAround(centers[0]), pixelPerCm: 5, name: "synthetic");

            Assert.True(result.IsComplete);
            Assert.Equal(Fps, result.Information.FrameRate);
            Assert.Equal(5, result.Information.PixelPerCm);
            Assert.Equal("synthetic", result.Information.Name);
            Assert.Equal(centers.Count, result.Information.Positions.Length);
            for (int i = 0; i < centers.Count; i++)
            {
                Assert.InRange(Math.Abs(result.Information.Positions[i].X - centers[i].X), 0, 6);
                Assert.InRange(Math.Abs(result.Information.Positions[i].Y - centers[i].Y), 0, 6);
            }
        }

        [Fact]
        public void Track_ThenAnalyze_FindsBothRepetitionsWithPlausibleRangeOfMotion()
        {
            var centers = TwoRepetitions();
            var video = CreateVideo(centers);
            var info = new VideoTracker().Track(video, RegionAround(centers[0]), pixelPerCm: 5, name: "set").Information;

            var settings = new AnalysisSettings();
            var analyzer = new SetAnalyzer(new ServiceCache<SetAnalysis>(),
                new PhysicalMotionCalculator(new ServiceCache<PhysicalMotion>(), settings), settings);
            var result = analyzer.GetCalculatedValue(info);

            Assert.Equal(2, result.Repetitions.Count);
            foreach (var rep in result.Repetitions)
            {
                Assert.InRange(rep.RangeOfMotion, 0.55, 0.65); // 300 px at 5 px/cm = 0.6 m
            }
            Assert.True(result.Repetitions[1].MeanVelocity < result.Repetitions[0].MeanVelocity); // second rep is slower
        }

        [Fact]
        public void Track_ReportsProgress()
        {
            var centers = TwoRepetitions();
            var video = CreateVideo(centers);
            var values = new List<double>();

            new VideoTracker().Track(video, RegionAround(centers[0]), 5, "p", new Progress<double>(values.Add));

            // Progress<T> posts asynchronously, give it a moment
            SpinWait.SpinUntil(() => values.Count > 0, 2000);
            Assert.NotEmpty(values);
            Assert.All(values, v => Assert.InRange(v, 0d, 1d));
        }

        [Fact]
        public void Track_Cancelled_ThrowsOperationCanceledException()
        {
            var centers = TwoRepetitions();
            var video = CreateVideo(centers);
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.Throws<OperationCanceledException>(() =>
                new VideoTracker().Track(video, RegionAround(centers[0]), 5, "c", cancellationToken: cts.Token));
        }

        [Fact]
        public void Track_MissingVideo_Throws()
        {
            Assert.Throws<FileNotFoundException>(() =>
                new VideoTracker().Track(Path.Combine(folder, "nope.avi"), new TrackingRegion(0, 0, 10, 10), 5, "x"));
        }

        [Fact]
        public void Track_RegionOutsideOfFrame_Throws()
        {
            var video = CreateVideo(TwoRepetitions());

            Assert.Throws<ArgumentException>(() =>
                new VideoTracker().Track(video, new TrackingRegion(Width - 5, 0, 50, 50), 5, "x"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Track_InvalidScale_Throws(double pixelPerCm)
        {
            var video = CreateVideo(TwoRepetitions());

            Assert.Throws<ArgumentException>(() =>
                new VideoTracker().Track(video, new TrackingRegion(0, 0, 50, 50), pixelPerCm, "x"));
        }

        [Fact]
        public void CalculatePixelPerCm_DividesPixelsByRealLength()
        {
            // 45 cm plate that is 180 px wide in the video
            Assert.Equal(4.0, VideoTracker.CalculatePixelPerCm(180, 45));
        }

        [Theory]
        [InlineData(0, 45)]
        [InlineData(180, 0)]
        [InlineData(-1, 45)]
        public void CalculatePixelPerCm_InvalidInput_Throws(double pixels, double cm)
        {
            Assert.Throws<ArgumentException>(() => VideoTracker.CalculatePixelPerCm(pixels, cm));
        }
    }
}
