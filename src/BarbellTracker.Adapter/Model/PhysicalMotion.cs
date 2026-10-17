using BarbellTracker.AbstractionCode;

namespace BarbellTracker.Adapter.Model
{
    /// <summary>
    /// The movement of the barbell in SI units. The y axis points UP (the image y axis is inverted).
    /// All arrays have the same length, one entry per tracked frame.
    /// </summary>
    public class PhysicalMotion
    {
        /// <summary>Time since the first frame in seconds</summary>
        public double[] Time { get; set; }
        /// <summary>Smoothed position in meters</summary>
        public Vector2D[] Position { get; set; }
        /// <summary>Velocity in m/s</summary>
        public Vector2D[] Velocity { get; set; }
        /// <summary>Acceleration in m/s²</summary>
        public Vector2D[] Acceleration { get; set; }

        public int Count => Time.Length;
    }
}
