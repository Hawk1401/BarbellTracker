using System;
using System.Collections.Generic;
using BarbellTracker.AbstractionCode;
using BarbellTracker.Adapter.Model;
using BarbellTracker.DomainCode;
using BarbellTracker.Services;
using BarbellTracker.Services.Implementation;
using Xunit;

namespace BarbellTracker.ServicesTests
{
    public class SetAnalyzerTester
    {
        private const int Fps = 30;

        // default scale: 1 px = 1 cm, so 100 px = 1 m
        private static TrackedInformation Track(string id, IEnumerable<Vector2D> positions, int fps = Fps, double pixelPerCm = 1)
        {
            return new TrackedInformation()
            {
                Id = id,
                Name = id,
                FrameRate = fps,
                PixelPerCm = pixelPerCm,
                Positions = new List<Vector2D>(positions).ToArray(),
            };
        }

        /// <summary>Cosine shaped movement between two image y values (top of the image has the smaller y)</summary>
        private static void AddMove(List<Vector2D> positions, int frames, double fromY, double toY, double x = 0)
        {
            for (int i = 1; i <= frames; i++)
            {
                var t = (double)i / frames;
                positions.Add(new Vector2D(x, fromY + (toY - fromY) * (1 - Math.Cos(Math.PI * t)) / 2));
            }
        }

        /// <summary>A set that starts at the top, every repetition goes down 1 m in 1.5 s and up in the given frames</summary>
        private static List<Vector2D> Set(params int[] concentricFrames)
        {
            var positions = new List<Vector2D> { new Vector2D(0, 0) };
            foreach (var up in concentricFrames)
            {
                AddMove(positions, 45, 0, 100);
                AddMove(positions, up, 100, 0);
            }
            return positions;
        }

        private static PhysicalMotionCalculator MotionCalculator(AnalysisSettings settings = null)
        {
            return new PhysicalMotionCalculator(new ServiceCache<PhysicalMotion>(), settings ?? new AnalysisSettings());
        }

        private static SetAnalyzer Analyzer(AnalysisSettings settings = null)
        {
            settings ??= new AnalysisSettings();
            return new SetAnalyzer(new ServiceCache<SetAnalysis>(), MotionCalculator(settings), settings);
        }

        // ---- PhysicalMotionCalculator ----

        [Fact]
        public void Motion_ConstantUpwardMovement_HasPositiveVelocityInMetersPerSecond()
        {
            // 10 px per frame upwards (image y decreases) at 10 fps with 1 px = 1 cm => 1 m/s
            var positions = new List<Vector2D>();
            for (int i = 0; i < 20; i++)
            {
                positions.Add(new Vector2D(0, 500 - 10 * i));
            }

            var motion = MotionCalculator().Calculate(Track("up", positions, fps: 10));

            foreach (var velocity in motion.Velocity)
            {
                Assert.Equal(1.0, velocity.Y, 6);
                Assert.Equal(0.0, velocity.X, 6);
            }
            foreach (var acceleration in motion.Acceleration)
            {
                Assert.Equal(0.0, acceleration.Y, 6);
            }
        }

        [Fact]
        public void Motion_PixelPerCm_ScalesDistances()
        {
            var positions = new List<Vector2D>();
            for (int i = 0; i < 10; i++)
            {
                positions.Add(new Vector2D(20 * i, 0));
            }

            // 20 px per frame, 10 px = 1 cm => 2 cm per frame, 10 fps => 0.2 m/s
            var motion = MotionCalculator().Calculate(Track("x", positions, fps: 10, pixelPerCm: 10));

            Assert.Equal(0.2, motion.Velocity[5].X, 6);
            Assert.Equal(0.18, motion.Position[9].X, 6);
        }

        [Fact]
        public void Motion_ConstantAcceleration_IsCalculated()
        {
            // y(t) = 0.5 * a * t^2 with a = 2 m/s^2 at 20 fps (positions in cm because 1 px = 1 cm)
            var positions = new List<Vector2D>();
            for (int i = 0; i < 30; i++)
            {
                var t = i / 20d;
                positions.Add(new Vector2D(0, -0.5 * 2 * t * t * 100));
            }

            var motion = MotionCalculator().Calculate(Track("acc", positions, fps: 20));

            Assert.Equal(2.0, motion.Acceleration[15].Y, 4);
        }

        [Fact]
        public void Motion_TimeStartsAtZeroAndUsesFrameRate()
        {
            var motion = MotionCalculator().Calculate(Track("t", new[] { new Vector2D(0, 0), new Vector2D(0, 0), new Vector2D(0, 0) }, fps: 4));

            Assert.Equal(new[] { 0d, 0.25, 0.5 }, motion.Time);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(-5, 1)]
        [InlineData(30, 0)]
        [InlineData(30, -1)]
        public void Motion_InvalidFrameRateOrScale_Throws(int fps, double pixelPerCm)
        {
            var info = Track("bad", new[] { new Vector2D(0, 0), new Vector2D(1, 1) }, fps, pixelPerCm);

            Assert.Throws<ArgumentException>(() => MotionCalculator().Calculate(info));
        }

        [Fact]
        public void Motion_FewerThanTwoPositions_Throws()
        {
            var info = Track("short", new[] { new Vector2D(0, 0) });

            Assert.Throws<ArgumentException>(() => MotionCalculator().Calculate(info));
        }

        [Fact]
        public void Motion_SameTrackedInformation_IsCached()
        {
            var calculator = MotionCalculator();
            var info = Track("cached", Set(30));

            Assert.Same(calculator.GetCalculatedValue(info), calculator.GetCalculatedValue(info));
        }

        // ---- SetAnalyzer ----

        [Fact]
        public void Analyze_ThreeRepetitions_FindsThreeRepetitions()
        {
            var result = Analyzer().GetCalculatedValue(Track("three", Set(24, 28, 34)));

            Assert.Equal(3, result.Repetitions.Count);
            Assert.Equal(1, result.Repetitions[0].Number);
            Assert.Equal(3, result.Repetitions[2].Number);
        }

        [Fact]
        public void Analyze_Repetition_HasExpectedMetrics()
        {
            // 24 frames (0.8 s) to lift 1 m with a cosine profile => mean 1.25 m/s, peak pi/2 * 1.25 m/s
            var rep = Analyzer().GetCalculatedValue(Track("metrics", Set(24))).Repetitions[0];

            Assert.Equal(0.8, rep.ConcentricDuration, 3);
            Assert.Equal(1.5, rep.EccentricDuration, 3);
            Assert.Equal(1.0, rep.RangeOfMotion, 3);
            Assert.Equal(1.25, rep.MeanVelocity, 2);
            Assert.Equal(Math.PI / 2 * 1.25, rep.PeakVelocity, 1);
            Assert.InRange(rep.MeanPropulsiveVelocity, 1.0, rep.PeakVelocity);
            Assert.Equal(0.0, rep.BarPathDeviation, 6);
            Assert.Equal(0.0, rep.VelocityLossPercent, 6);
        }

        [Fact]
        public void Analyze_SlowerRepetitions_HaveGrowingVelocityLoss()
        {
            var result = Analyzer().GetCalculatedValue(Track("loss", Set(24, 28, 34)));

            Assert.Equal(0.0, result.Repetitions[0].VelocityLossPercent, 6);
            Assert.True(result.Repetitions[1].VelocityLossPercent > 0);
            Assert.True(result.Repetitions[2].VelocityLossPercent > result.Repetitions[1].VelocityLossPercent);
            Assert.Equal(result.Repetitions[2].VelocityLossPercent, result.VelocityLossPercent);
        }

        [Fact]
        public void Analyze_HorizontalDrift_IsReportedAsBarPathDeviation()
        {
            var positions = new List<Vector2D> { new Vector2D(0, 100) };
            AddMove(positions, 30, 100, 0);
            // drift sideways by 5 cm (= 0.05 m) while lifting
            var drift = new List<Vector2D>();
            for (int i = 0; i < positions.Count; i++)
            {
                drift.Add(new Vector2D(5 * (double)i / (positions.Count - 1), positions[i].Y));
            }

            var rep = Analyzer().GetCalculatedValue(Track("drift", drift)).Repetitions[0];

            Assert.Equal(0.05, rep.BarPathDeviation, 2);
        }

        [Fact]
        public void Analyze_StartingAtTheBottom_FindsRepetitionWithoutEccentricPhase()
        {
            var positions = new List<Vector2D> { new Vector2D(0, 100) };
            AddMove(positions, 30, 100, 0);

            var result = Analyzer().GetCalculatedValue(Track("deadlift", positions));

            Assert.Single(result.Repetitions);
            Assert.Equal(0.0, result.Repetitions[0].EccentricDuration);
        }

        [Fact]
        public void Analyze_NoiseBelowMinimalRangeOfMotion_IsNoRepetition()
        {
            var rnd = new Random(42);
            var positions = new List<Vector2D>();
            for (int i = 0; i < 200; i++)
            {
                positions.Add(new Vector2D(0, 50 + rnd.NextDouble() * 3)); // 3 cm jitter
            }

            var result = Analyzer().GetCalculatedValue(Track("noise", positions));

            Assert.Empty(result.Repetitions);
            Assert.Equal(0.0, result.VelocityLossPercent);
        }

        [Fact]
        public void Analyze_NoisyRepetitions_AreStillDetected()
        {
            var rnd = new Random(7);
            var noisy = new List<Vector2D>();
            foreach (var p in Set(24, 28, 34))
            {
                noisy.Add(new Vector2D(p.X, p.Y + (rnd.NextDouble() - 0.5) * 2)); // +-1 cm jitter
            }

            var result = Analyzer().GetCalculatedValue(Track("noisyset", noisy));

            Assert.Equal(3, result.Repetitions.Count);
        }

        [Fact]
        public void Analyze_MinRangeOfMotion_DecidesWhatIsARepetition()
        {
            var positions = new List<Vector2D> { new Vector2D(0, 30) };
            AddMove(positions, 20, 30, 0); // lifts 0.3 m
            var strict = new AnalysisSettings() { MinRangeOfMotion = 0.5 };
            var lenient = new AnalysisSettings() { MinRangeOfMotion = 0.1 };

            Assert.Empty(Analyzer(strict).GetCalculatedValue(Track("strict", positions)).Repetitions);
            Assert.Single(Analyzer(lenient).GetCalculatedValue(Track("lenient", positions)).Repetitions);
        }

        [Fact]
        public void Analyze_SameTrackedInformation_IsCached()
        {
            var analyzer = Analyzer();
            var info = Track("cachedset", Set(24, 28));

            Assert.Same(analyzer.GetCalculatedValue(info), analyzer.GetCalculatedValue(info));
        }

        // ---- SetCSVTranslater ----

        [Fact]
        public void Csv_ContainsHeaderAndOneRowPerRepetition()
        {
            var translater = new SetCSVTranslater(Analyzer(), new ServiceCache<SetCSVModel>());

            var csv = translater.GetCSV(Track("csv", Set(24, 28, 34))).ToString();
            var lines = csv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Assert.Equal(SetCSVModel.GetHeader(), lines[0]);
            Assert.Equal(4, lines.Length);
            Assert.StartsWith("1;", lines[1]);
            Assert.DoesNotContain(",", lines[1]); // invariant culture, no decimal comma
        }

        [Fact]
        public void Csv_SameTrackedInformation_IsCached()
        {
            var translater = new SetCSVTranslater(Analyzer(), new ServiceCache<SetCSVModel>());
            var info = Track("csvcache", Set(24));

            Assert.Same(translater.GetCSV(info), translater.GetCSV(info));
        }
    }
}
