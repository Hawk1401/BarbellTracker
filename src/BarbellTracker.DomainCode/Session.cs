using System;

namespace BarbellTracker.DomainCode
{
    /// <summary>
    /// A saved set: the tracked positions plus what the user entered about it and a short summary of the result.
    /// The positions are stored, not the calculated metrics, so a session can always be analysed again with new settings.
    /// </summary>
    public class Session
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; }
        public string Exercise { get; set; }
        public double WeightKg { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string VideoPath { get; set; }

        // summary for the history list (calculated when the session was saved)
        public int RepetitionCount { get; set; }
        public double BestMeanVelocity { get; set; }
        public double VelocityLossPercent { get; set; }

        public TrackedInformation Tracked { get; set; }
    }
}
