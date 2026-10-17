using System;
using BarbellTracker.AbstractionCode;
using BarbellTracker.Adapter.Model;
using BarbellTracker.DomainCode;
using BarbellTracker.Services.Interface;

namespace BarbellTracker.Services.Implementation
{
    /// <summary>
    /// Converts the tracked pixel positions into meters, m/s and m/s² with an upward pointing y axis.
    /// </summary>
    public class PhysicalMotionCalculator : IService, ICalculator<PhysicalMotion>
    {
        public string Name => "Physical Motion Calculator";
        public string Description => "This service converts the tracked pixel positions into meters, m/s and m/s²";

        private readonly ServiceCache<PhysicalMotion> cache;
        private readonly AnalysisSettings settings;
        private readonly object locker = new object();

        public PhysicalMotionCalculator(ServiceCache<PhysicalMotion> cache, AnalysisSettings settings)
        {
            this.cache = cache;
            this.settings = settings;
        }

        public PhysicalMotion GetCalculatedValue(TrackedInformation trackedInformation)
        {
            lock (locker)
            {
                if (cache.TryGetCachedItem(trackedInformation, out var cached))
                {
                    return cached;
                }

                return cache.AddItemToCache(trackedInformation, Calculate(trackedInformation));
            }
        }

        public PhysicalMotion Calculate(TrackedInformation info)
        {
            if (info.Positions == null || info.Positions.Length < 2)
            {
                throw new ArgumentException("At least two positions are needed to calculate a motion");
            }
            if (info.FrameRate <= 0)
            {
                throw new ArgumentException($"The frame rate has to be positive but was {info.FrameRate}");
            }
            if (info.PixelPerCm <= 0)
            {
                throw new ArgumentException($"PixelPerCm has to be positive but was {info.PixelPerCm}");
            }

            int n = info.Positions.Length;
            double dt = 1d / info.FrameRate;
            double metersPerPixel = 1d / info.PixelPerCm / 100d;

            var x = new double[n];
            var y = new double[n];
            var time = new double[n];
            for (int i = 0; i < n; i++)
            {
                x[i] = info.Positions[i].X * metersPerPixel;
                y[i] = -info.Positions[i].Y * metersPerPixel; // image y points down
                time[i] = i * dt;
            }

            x = SignalSmoother.SavitzkyGolay(x, settings.SmoothingWindow);
            y = SignalSmoother.SavitzkyGolay(y, settings.SmoothingWindow);

            var vx = Gradient(x, dt);
            var vy = Gradient(y, dt);
            var ax = Gradient(vx, dt);
            var ay = Gradient(vy, dt);

            var position = new Vector2D[n];
            var velocity = new Vector2D[n];
            var acceleration = new Vector2D[n];
            for (int i = 0; i < n; i++)
            {
                position[i] = new Vector2D(x[i], y[i]);
                velocity[i] = new Vector2D(vx[i], vy[i]);
                acceleration[i] = new Vector2D(ax[i], ay[i]);
            }

            return new PhysicalMotion()
            {
                Time = time,
                Position = position,
                Velocity = velocity,
                Acceleration = acceleration,
            };
        }

        /// <summary>Derivative with central differences, one sided at the edges</summary>
        private static double[] Gradient(double[] values, double dt)
        {
            int n = values.Length;
            var result = new double[n];
            result[0] = (values[1] - values[0]) / dt;
            result[n - 1] = (values[n - 1] - values[n - 2]) / dt;
            for (int i = 1; i < n - 1; i++)
            {
                result[i] = (values[i + 1] - values[i - 1]) / (2 * dt);
            }
            return result;
        }
    }
}
