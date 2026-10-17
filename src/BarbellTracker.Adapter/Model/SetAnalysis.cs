using System;
using System.Collections.Generic;

namespace BarbellTracker.Adapter.Model
{
    public class SetAnalysis
    {
        public IReadOnlyList<Repetition> Repetitions { get; init; } = Array.Empty<Repetition>();

        /// <summary>Velocity loss of the last repetition compared to the fastest one in percent</summary>
        public double VelocityLossPercent =>
            Repetitions.Count == 0 ? 0 : Repetitions[Repetitions.Count - 1].VelocityLossPercent;
    }
}
