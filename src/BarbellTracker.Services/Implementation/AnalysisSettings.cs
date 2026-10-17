namespace BarbellTracker.Services.Implementation
{
    public class AnalysisSettings
    {
        /// <summary>Savitzky-Golay window for the positions: 0 (off), 5, 7 or 9</summary>
        public int SmoothingWindow { get; init; } = 5;

        /// <summary>
        /// Minimal vertical distance in meters that counts as a movement phase.
        /// Smaller movements (re-grip, bar bounce, tracking noise) are not detected as repetition.
        /// </summary>
        public double MinRangeOfMotion { get; init; } = 0.2;
    }
}
