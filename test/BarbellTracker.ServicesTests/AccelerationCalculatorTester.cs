using BarbellTracker.AbstractionCode;
using BarbellTracker.Adapter.Model;
using BarbellTracker.DomainCode;
using BarbellTracker.Services;
using BarbellTracker.Services.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BarbellTracker.Services.Interface;

namespace BarbellTracker.ServicesTests
{
    public class AccelerationCalculatorTester
    {
        AccelerationCalculator _sut = CreateSUT(); // System Under tests


        [Theory]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-42)]
        [InlineData(42)]
        public void TestFrameRateoutput(int frameRate)
        {
            //Arrange
            TrackedInformation trackedInfos = new TrackedInformation()
            {
                FrameRate = frameRate,
                Id = $"MyTestIdWithFPS",
                PixelPerCm = 300,
                Name = "myTestName",
                Positions = new Vector2D[] { new Vector2D(0, 0), new Vector2D(1, 0) }
            };


            //Act
            var velocity = _sut.GetCalculatedValue(trackedInfos);

            //Assert
            Assert.Equal(velocity.FPS, frameRate);
        }

        [Theory]
        [MemberData(nameof(VelocityWithAccelerationArrayEnumerable))]
        public void TestGetVelocity(Vector2D[] Posions , Vector2D[] Acceleration)
        {
            //Arrange
            TrackedInformation trackedInfos = new TrackedInformation()
            {
                FrameRate = 30,
                Id = "MyTestId",
                PixelPerCm = 300,
                Name = "myTestName",
                Positions = Posions
            };


            //Act
            var CaledAcceleration = _sut.GetCalculatedValue(trackedInfos);


            //Assert
            Assert.Equal(CaledAcceleration.Vectors.Length, Acceleration.Length);
            for (int i = 0; i < CaledAcceleration.Vectors.Length; i++)
            {
                Assert.Equal(CaledAcceleration.Vectors[i], Acceleration[i]);
            }
        }

        [Fact]
        public void Request_CachedVelocity_WillReturnTheCachedVelocity()
        {
            //Arrange
            TrackedInformation trackedInfos = new TrackedInformation()
            {
                FrameRate = 30,
                Id = "MyTestId",
                PixelPerCm = 300,
                Name = "myTestName",
                Positions = new Vector2D[]
                {
                    new AbstractionCode.Vector2D(0,0),
                    new AbstractionCode.Vector2D(0,1),
                    new AbstractionCode.Vector2D(0,3)
                }
            };


            //Act
            var Origanal = _sut.GetCalculatedValue(trackedInfos);
            var Cached = _sut.GetCalculatedValue(trackedInfos);

            //Assert
            Assert.StrictEqual(Origanal, Cached);

        }
        public static IEnumerable<object[]> VelocityWithAccelerationArrayEnumerable()
        {
            var VerticalVectors = new Vector2D[]
            {
                new AbstractionCode.Vector2D(0,0),
                new AbstractionCode.Vector2D(0,1),
                new AbstractionCode.Vector2D(0,0),
                new AbstractionCode.Vector2D(0,-1),
                new AbstractionCode.Vector2D(0,0),
                new AbstractionCode.Vector2D(0,1),
                new AbstractionCode.Vector2D(0,-1),
                new AbstractionCode.Vector2D(0,1),

            };

            var VerticalTestAcceleration = new Vector2D[]
            {
                new Vector2D(0,-2),
                new Vector2D(0,0),
                new Vector2D(0,2),
                new Vector2D(0,0),
                new Vector2D(0,-3),
                new Vector2D(0,4),
            };


            yield return new object[] { VerticalVectors, VerticalTestAcceleration };



            var HorizontalVectors = new Vector2D[]
            {
                new Vector2D(0,0),
                new Vector2D(1,0),
                new Vector2D(0,0),
                new Vector2D(-1,0),
                new Vector2D(0,0),
                new Vector2D(1,0),
                new Vector2D(-1,0),
                new Vector2D(1,0),

            };

            var HorizontalTestAcceleration = new Vector2D[]
            {
                new Vector2D(-2,0),
                new Vector2D(0,0),
                new Vector2D(2,0),
                new Vector2D(0,0),
                new Vector2D(-3,0),
                new Vector2D(4,0),
            };

            yield return new object[] { HorizontalVectors, HorizontalTestAcceleration };


            var DiagonalVectors = new Vector2D[]
            {
                new Vector2D(0,0),
                new Vector2D(1,1),
                new Vector2D(0,0),
                new Vector2D(-1,-1),
                new Vector2D(0,0),
                new Vector2D(1,-1),
                new Vector2D(0,0),
                new Vector2D(-1,1),
                new Vector2D(0,0),



            };

            //var DiagonalTestVelocity = new Vector2D[]
            //{
            //    new Vector2D(1,1),
            //    new Vector2D(-1,-1),
            //    new Vector2D(-1,-1),
            //    new Vector2D(1,1),
            //    new Vector2D(1,-1),
            //    new Vector2D(-1,1),
            //    new Vector2D(-1,1),
            //    new Vector2D(1,-1),
            //};

            var DiagonalTestAcceleration = new Vector2D[]
            {
                new Vector2D(-2,-2),
                new Vector2D(0,0),
                new Vector2D(2,2),
                new Vector2D(0,-2),
                new Vector2D(-2,2),
                new Vector2D(0,0),
                new Vector2D(2,-2),
            };

            yield return new object[] { DiagonalVectors, DiagonalTestAcceleration};
        }

        public static AccelerationCalculator CreateSUT()
        {
            using IHost host = Host.CreateDefaultBuilder(new string[0])
                .ConfigureServices((_, services) =>
                    services.AddTransient<ServiceCache<Velocity>>()
                    .AddTransient<ServiceCache<Acceleration>>()
                    .AddTransient<ICalculator<Velocity>, VelocityCalculator>()
                    .AddTransient<AccelerationCalculator>())
                .Build();

            var servieces = host.Services;
            var Scope = servieces.CreateScope();
            var provider = Scope.ServiceProvider;
            return provider.GetRequiredService<AccelerationCalculator>();
        }
    }
}
