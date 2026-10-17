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

    /// <summary>Basic properties of a video file</summary>
    public record VideoInfo(int Width, int Height, int FrameRate, int FrameCount);

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

        public static VideoInfo GetInfo(string videoPath)
        {
            using var capture = OpenVideo(videoPath);
            return new VideoInfo(capture.FrameWidth, capture.FrameHeight, (int)Math.Round(capture.Fps), capture.FrameCount);
        }

        /// <summary>Reads a single frame of the video as PNG, e.g. to show it in the UI</summary>
        public static byte[] ReadFramePng(string videoPath, int frameIndex)
        {
            using var capture = OpenVideo(videoPath);
            capture.PosFrames = Math.Max(0, frameIndex);
            using var frame = new Mat();
            if (!capture.Read(frame) || frame.Empty())
            {
                throw new ArgumentOutOfRangeException(nameof(frameIndex), $"The video has no frame {frameIndex}");
            }
            Cv2.ImEncode(".png", frame, out var png);
            return png;
        }

        private static VideoCapture OpenVideo(string videoPath)
        {
            if (!File.Exists(videoPath)) throw new FileNotFoundException("The video does not exist", videoPath);
            var capture = new VideoCapture(videoPath);
            if (!capture.IsOpened())
            {
                capture.Dispose();
                throw new InvalidDataException($"The video '{videoPath}' could not be opened");
            }
            return capture;
        }

        /// <param name="startFrame">The frame that contains the region, tracking starts there (time 0)</param>
        public TrackingResult Track(string videoPath, TrackingRegion region, double pixelPerCm, string name,
            IProgress<double> progress = null, CancellationToken cancellationToken = default, int startFrame = 0)
        {
            if (!File.Exists(videoPath)) throw new FileNotFoundException("The video does not exist", videoPath);
            if (pixelPerCm <= 0) throw new ArgumentException("PixelPerCm has to be positive", nameof(pixelPerCm));
            if (region == null || region.Width <= 0 || region.Height <= 0) throw new ArgumentException("The region has to have a size", nameof(region));

            if (startFrame < 0) throw new ArgumentOutOfRangeException(nameof(startFrame));

            using var capture = OpenVideo(videoPath);
            if (startFrame > 0) capture.PosFrames = startFrame;

            var fps = (int)Math.Round(capture.Fps);
            if (fps <= 0) throw new InvalidDataException("The video has no valid frame rate");
            var frameCount = capture.FrameCount - startFrame;

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
