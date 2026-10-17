using System;
using System.IO;
using System.Threading;
using BarbellTracker.AbstractionCode;
using BarbellTracker.DomainCode;
using OpenCvSharp;
using OpenCvSharp.Tracking;

namespace BarbellTracker.Tracking
{
    /// <summary>Rectangle in pixels of the first frame that contains the object to track (e.g. the barbell plate).</summary>
    public record TrackingRegion(int X, int Y, int Width, int Height);

    /// <summary>
    /// The result of a tracking run. If the tracker lost the object, the positions contain everything
    /// up to the last good frame and LostAtFrame is the index of the first frame without a position.
    /// </summary>
    public record TrackingResult(TrackedInformation Information, int? LostAtFrame)
    {
        public bool IsComplete => LostAtFrame == null;
    }

    /// <summary>Tracks an object through a video with the OpenCV CSRT tracker.</summary>
    public class VideoTracker
    {
        /// <summary>
        /// Calculates the scale from a known real world length, e.g. the diameter of a 45 cm plate.
        /// </summary>
        public static double CalculatePixelPerCm(double pixelLength, double realLengthInCm)
        {
            if (pixelLength <= 0) throw new ArgumentException("The length in pixels has to be positive", nameof(pixelLength));
            if (realLengthInCm <= 0) throw new ArgumentException("The real length has to be positive", nameof(realLengthInCm));
            return pixelLength / realLengthInCm;
        }

        public TrackingResult Track(string videoPath, TrackingRegion region, double pixelPerCm, string name,
            IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(videoPath)) throw new FileNotFoundException("The video does not exist", videoPath);
            if (pixelPerCm <= 0) throw new ArgumentException("PixelPerCm has to be positive", nameof(pixelPerCm));
            if (region == null || region.Width <= 0 || region.Height <= 0) throw new ArgumentException("The region has to have a size", nameof(region));

            using var capture = new VideoCapture(videoPath);
            if (!capture.IsOpened()) throw new InvalidDataException($"The video '{videoPath}' could not be opened");

            var fps = (int)Math.Round(capture.Fps);
            if (fps <= 0) throw new InvalidDataException("The video has no valid frame rate");
            var frameCount = capture.FrameCount;

            using var frame = new Mat();
            if (!capture.Read(frame) || frame.Empty()) throw new InvalidDataException("The video has no frames");

            var bounds = new Rect(0, 0, frame.Width, frame.Height);
            var roi = new Rect(region.X, region.Y, region.Width, region.Height);
            if (roi != (roi & bounds)) throw new ArgumentException("The region has to be inside the video frame", nameof(region));

            using var tracker = TrackerCSRT.Create();
            tracker.Init(frame, roi);

            var positions = new System.Collections.Generic.List<Vector2D> { Center(roi) };
            int? lostAt = null;
            var index = 1;

            while (capture.Read(frame) && !frame.Empty())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var box = roi;
                if (!tracker.Update(frame, ref box))
                {
                    lostAt = index;
                    break;
                }
                roi = box;
                positions.Add(Center(roi));

                if (frameCount > 0) progress?.Report(Math.Min(1d, (double)index / frameCount));
                index++;
            }

            var info = new TrackedInformation()
            {
                Id = $"{Path.GetFileNameWithoutExtension(videoPath)}-{DateTime.Now:yyyyMMddHHmmssfff}",
                Name = name,
                Positions = positions.ToArray(),
                FrameRate = fps,
                PixelPerCm = pixelPerCm,
            };
            return new TrackingResult(info, lostAt);
        }

        private static Vector2D Center(Rect r) => new Vector2D(r.X + r.Width / 2d, r.Y + r.Height / 2d);
    }
}
