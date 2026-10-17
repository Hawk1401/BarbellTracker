using System;
using BarbellTracker.AbstractionCode;
using Xunit;

namespace BarbellTracker.AbstractionCodeTests
{
    public class SignalSmootherTester
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void SavitzkyGolay_WithoutWindow_ReturnsCopy(int window)
        {
            var data = new double[] { 1, 5, 2, 8 };

            var result = SignalSmoother.SavitzkyGolay(data, window);

            Assert.Equal(data, result);
            Assert.NotSame(data, result);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(9)]
        public void SavitzkyGolay_OfQuadraticSignal_KeepsSignal(int window)
        {
            var data = new double[30];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 0.5 * i * i - 3 * i + 2;
            }

            var result = SignalSmoother.SavitzkyGolay(data, window);

            for (int i = 0; i < data.Length; i++)
            {
                Assert.Equal(data[i], result[i], 6);
            }
        }

        [Fact]
        public void SavitzkyGolay_OfSpike_ReducesSpike()
        {
            var data = new double[] { 0, 0, 0, 0, 10, 0, 0, 0, 0 };

            var result = SignalSmoother.SavitzkyGolay(data, 5);

            Assert.True(result[4] < data[4]);
        }

        [Fact]
        public void SavitzkyGolay_UnsupportedWindow_Throws()
        {
            Assert.Throws<ArgumentException>(() => SignalSmoother.SavitzkyGolay(new double[10], 4));
        }

        [Fact]
        public void SavitzkyGolay_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => SignalSmoother.SavitzkyGolay(null, 5));
        }

        [Fact]
        public void SavitzkyGolay_ShorterThanWindow_ReturnsCopy()
        {
            var data = new double[] { 1, 2, 3 };

            Assert.Equal(data, SignalSmoother.SavitzkyGolay(data, 5));
        }
    }
}
