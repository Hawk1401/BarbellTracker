using System;
using System.Collections.Generic;
using System.Linq;
using BarbellTracker.Adapter.Model;
using BarbellTracker.DomainCode;
using BarbellTracker.Services.Interface;

namespace BarbellTracker.Services.Implementation
{
    /// <summary>
    /// Detects the repetitions of a set and calculates the metrics of every repetition.
    /// </summary>
    public class SetAnalyzer : IService, ICalculator<SetAnalysis>
    {
        private const double Gravity = 9.81;

        public string Name => "Set Analyzer";
        public string Description => "This service detects the repetitions of a set and calculates velocity metrics for each of them";

        private readonly ServiceCache<SetAnalysis> cache;
        private readonly ICalculator<PhysicalMotion> motionCalculator;
        private readonly AnalysisSettings settings;
        private readonly object locker = new object();

        public SetAnalyzer(ServiceCache<SetAnalysis> cache, ICalculator<PhysicalMotion> motionCalculator, AnalysisSettings settings)
        {
            this.cache = cache;
            this.motionCalculator = motionCalculator;
            this.settings = settings;
        }

        public SetAnalysis GetCalculatedValue(TrackedInformation trackedInformation)
        {
            lock (locker)
            {
                if (cache.TryGetCachedItem(trackedInformation, out var cached))
                {
                    return cached;
                }

                var motion = motionCalculator.GetCalculatedValue(trackedInformation);
                return cache.AddItemToCache(trackedInformation, Analyze(motion));
            }
        }

        public SetAnalysis Analyze(PhysicalMotion motion)
        {
            var phases = FindConcentricPhases(motion);
            var reps = phases.Select((phase, i) => CalculateMetrics(motion, phase, i + 1)).ToList();

            if (reps.Count == 0)
            {
                return new SetAnalysis();
            }

            var best = reps.Max(r => r.MeanVelocity);
            var withLoss = reps.Select(r => new Repetition()
            {
                Number = r.Number,
                StartIndex = r.StartIndex,
                EndIndex = r.EndIndex,
                ConcentricDuration = r.ConcentricDuration,
                EccentricDuration = r.EccentricDuration,
                RangeOfMotion = r.RangeOfMotion,
                MeanVelocity = r.MeanVelocity,
                PeakVelocity = r.PeakVelocity,
                MeanPropulsiveVelocity = r.MeanPropulsiveVelocity,
                BarPathDeviation = r.BarPathDeviation,
                VelocityLossPercent = best > 0 ? (best - r.MeanVelocity) / best * 100d : 0,
            }).ToList();

            return new SetAnalysis() { Repetitions = withLoss };
        }

        private record ConcentricPhase(int EccentricStart, int Start, int End);

        /// <summary>
        /// Zigzag turning point detection: a turning point is only accepted after the bar moved
        /// at least MinRangeOfMotion away from it, so noise does not create repetitions.
        /// Every valley => peak segment is the concentric phase of one repetition.
        /// </summary>
        private List<ConcentricPhase> FindConcentricPhases(PhysicalMotion motion)
        {
            var y = motion.Position.Select(p => p.Y).ToArray();
            var th = settings.MinRangeOfMotion;
            var pivots = new List<(int Index, bool IsPeak)>();

            int dir = 0; // 1 = moving up, -1 = moving down
            int hi = 0, lo = 0;
            for (int i = 1; i < y.Length; i++)
            {
                if (y[i] > y[hi]) hi = i;
                if (y[i] < y[lo]) lo = i;

                if (dir == 0)
                {
                    if (y[i] - y[lo] >= th)
                    {
                        pivots.Add((lo, false));
                        dir = 1;
                        hi = i;
                    }
                    else if (y[hi] - y[i] >= th)
                    {
                        pivots.Add((hi, true));
                        dir = -1;
                        lo = i;
                    }
                }
                else if (dir == 1 && y[hi] - y[i] >= th)
                {
                    pivots.Add((hi, true));
                    dir = -1;
                    lo = i;
                }
                else if (dir == -1 && y[i] - y[lo] >= th)
                {
                    pivots.Add((lo, false));
                    dir = 1;
                    hi = i;
                }
            }

            if (dir == 1) pivots.Add((hi, true));
            if (dir == -1) pivots.Add((lo, false));

            var phases = new List<ConcentricPhase>();
            for (int k = 0; k + 1 < pivots.Count; k++)
            {
                if (!pivots[k].IsPeak && pivots[k + 1].IsPeak)
                {
                    var eccentricStart = k > 0 ? pivots[k - 1].Index : -1;
                    phases.Add(new ConcentricPhase(eccentricStart, pivots[k].Index, pivots[k + 1].Index));
                }
            }
            return phases;
        }

        private static Repetition CalculateMetrics(PhysicalMotion motion, ConcentricPhase phase, int number)
        {
            var (eccentricStart, start, end) = phase;
            var duration = motion.Time[end] - motion.Time[start];
            var rom = motion.Position[end].Y - motion.Position[start].Y;

            int peakIndex = start;
            double peak = double.MinValue;
            double deviation = 0;
            for (int i = start; i <= end; i++)
            {
                if (motion.Velocity[i].Y > peak)
                {
                    peak = motion.Velocity[i].Y;
                    peakIndex = i;
                }
                deviation = Math.Max(deviation, Math.Abs(motion.Position[i].X - motion.Position[start].X));
            }

            // propulsive phase: until the deceleration is stronger than gravity
            int propulsiveEnd = end;
            for (int i = peakIndex; i <= end; i++)
            {
                if (motion.Acceleration[i].Y <= -Gravity)
                {
                    propulsiveEnd = i;
                    break;
                }
            }
            double mpv = 0;
            for (int i = start; i <= propulsiveEnd; i++)
            {
                mpv += motion.Velocity[i].Y;
            }
            mpv /= propulsiveEnd - start + 1;

            return new Repetition()
            {
                Number = number,
                StartIndex = start,
                EndIndex = end,
                ConcentricDuration = duration,
                EccentricDuration = eccentricStart >= 0 ? motion.Time[start] - motion.Time[eccentricStart] : 0,
                RangeOfMotion = rom,
                MeanVelocity = duration > 0 ? rom / duration : 0,
                PeakVelocity = peak,
                MeanPropulsiveVelocity = mpv,
                BarPathDeviation = deviation,
            };
        }
    }
}
