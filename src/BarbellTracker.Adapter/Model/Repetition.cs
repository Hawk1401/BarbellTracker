namespace BarbellTracker.Adapter.Model
{
    /// <summary>
    /// The metrics of a single repetition. A repetition is measured on its concentric (upward) phase.
    /// </summary>
    public class Repetition
    {
        public int Number { get; init; }
        /// <summary>Frame index where the concentric phase starts</summary>
        public int StartIndex { get; init; }
        /// <summary>Frame index where the concentric phase ends</summary>
        public int EndIndex { get; init; }
        /// <summary>Duration of the concentric phase in seconds</summary>
        public double ConcentricDuration { get; init; }
        /// <summary>Duration of the preceding eccentric phase in seconds, 0 if the set starts with the concentric phase</summary>
        public double EccentricDuration { get; init; }
        /// <summary>Range of motion (vertical distance of the concentric phase) in meters</summary>
        public double RangeOfMotion { get; init; }
        /// <summary>Mean velocity of the concentric phase in m/s</summary>
        public double MeanVelocity { get; init; }
        public double PeakVelocity { get; init; }
        /// <summary>Mean velocity of the propulsive phase (until the bar is decelerated stronger than gravity) in m/s</summary>
        public double MeanPropulsiveVelocity { get; init; }
        /// <summary>Largest horizontal distance to the start position in meters</summary>
        public double BarPathDeviation { get; init; }
        /// <summary>Mean velocity loss compared to the fastest repetition of the set in percent</summary>
        public double VelocityLossPercent { get; init; }
    }
}
