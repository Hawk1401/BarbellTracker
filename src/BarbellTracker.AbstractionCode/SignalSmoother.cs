using System;
using System.Collections.Generic;

namespace BarbellTracker.AbstractionCode
{
    /// <summary>
    /// Noise reduction for tracked signals.
    /// Pixel tracking jitters by a few pixels, which becomes huge noise after differentiating twice,
    /// so the positions are smoothed before velocity and acceleration are derived.
    /// </summary>
    public static class SignalSmoother
    {
        // Quadratic Savitzky-Golay convolution coefficients: window => (coefficients, normalization)
        private static readonly Dictionary<int, (int[] Coefficients, int Norm)> Filters = new()
        {
            { 5, (new[] { -3, 12, 17, 12, -3 }, 35) },
            { 7, (new[] { -2, 3, 6, 7, 6, 3, -2 }, 21) },
            { 9, (new[] { -21, 14, 39, 54, 59, 54, 39, 14, -21 }, 231) },
        };

        /// <summary>
        /// Applies a Savitzky-Golay filter, which keeps peaks much better than a moving average.
        /// The first and last window/2 samples are not filtered (no complete window available).
        /// </summary>
        /// <param name="data">The raw signal</param>
        /// <param name="window">0 or 1 for no smoothing, otherwise 5, 7 or 9</param>
        /// <returns>A new array with the smoothed signal</returns>
        public static double[] SavitzkyGolay(double[] data, int window)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var result = (double[])data.Clone();

            if (window == 0 || window == 1)
            {
                return result;
            }

            if (!Filters.TryGetValue(window, out var filter))
            {
                throw new ArgumentException($"Window size {window} is not supported, use 0, 5, 7 or 9", nameof(window));
            }

            var half = window / 2;
            for (int i = half; i < data.Length - half; i++)
            {
                double sum = 0;
                for (int k = 0; k < window; k++)
                {
                    sum += filter.Coefficients[k] * data[i - half + k];
                }
                result[i] = sum / filter.Norm;
            }

            return result;
        }
    }
}
