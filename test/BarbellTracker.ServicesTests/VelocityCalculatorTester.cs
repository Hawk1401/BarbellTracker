using System;
using System.Collections.Generic;
using BarbellTracker.AbstractionCode;
using BarbellTracker.DomainCode;
using BarbellTracker.Services.Implementation;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BarbellTracker.Services.Interface;
using BarbellTracker.Adapter.Model;
using BarbellTracker.Services;

namespace BarbellTracker.ServicesTests
{
    public class VelocityCalculatorTester
    {

        VelocityCalculator _sut = CreateSUT(); // System Under tests





        [Theory]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-42)]
        [InlineData(42)]
        public void TestFrameRateoutput(int frameRate)
        {
            TrackedInformation trackedInfos = new TrackedInformation()
            {
                FrameRate = frameRate,
                Id = $"MyTestIdWithFPS",
                PixelPerCm = 300,
                Name = "myTestName",
                Positions = new Vector2D[] { new Vector2D(0, 0), new Vector2D(1, 0) }
            };


            var velocity = _sut.GetCalculatedValue(trackedInfos);


            Assert.Equal(velocity.FPS, frameRate);
        }

        [Theory]
        [MemberData(nameof(TestDataForTestGetVelocity))]
        public void TestGetVelocity(Vector2D[] Posions, Vector2D[] Velocity)
        {
            TrackedInformation trackedInfos = new TrackedInformation()
            {
                FrameRate = 30,
                Id = "MyTestId",
                PixelPerCm = 300,
                Name = "myTestName",
                Positions = Posions
            };


            var velocity = _sut.GetCalculatedValue(trackedInfos);


            Assert.Equal(velocity.Vectors.Length, Velocity.Length);
            for (int i = 0; i < velocity.Vectors.Length; i++)
            {
                Assert.Equal(velocity.Vectors[i], Velocity[i]);
            }
        }

        [Theory]
        [MemberData(nameof(TestDataForTestGetVelocity))]
        public void Request_ACachedVelocity_WillReturnTheCachedVelocity(Vector2D[] Posions, Vector2D[] _)
        {
            TrackedInformation trackedInfos = new TrackedInformation()
            {
                FrameRate = 30,
                Id = "MyTestId",
                PixelPerCm = 300,
                Name = "myTestName",
                Positions = Posions
            };


            var expected = _sut.GetCalculatedValue(trackedInfos);

            var acuale = _sut.GetCalculatedValue(trackedInfos);
            Assert.StrictEqual(expected, acuale);
        }

        public static IEnumerable<object[]> TestDataForTestGetVelocity()
        {
            var VerticalVectors = new AbstractionCode.Vector2D[]
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

            var VerticalTestVelocity = new AbstractionCode.Vector2D[]
            {
                new AbstractionCode.Vector2D(0,1),
                new AbstractionCode.Vector2D(0,-1),
                new AbstractionCode.Vector2D(0,-1),
                new AbstractionCode.Vector2D(0,1),
                new AbstractionCode.Vector2D(0,1),
                new AbstractionCode.Vector2D(0,-2),
                new AbstractionCode.Vector2D(0,2),
            };

            yield return new object[] { VerticalVectors, VerticalTestVelocity };



            var HorizontalVectors = new AbstractionCode.Vector2D[]
            {
                new AbstractionCode.Vector2D(0,0),
                new AbstractionCode.Vector2D(1,0),
                new AbstractionCode.Vector2D(0,0),
                new AbstractionCode.Vector2D(-1,0),
                new AbstractionCode.Vector2D(0,0),
                new AbstractionCode.Vector2D(1,0),
                new AbstractionCode.Vector2D(-1,0),
                new AbstractionCode.Vector2D(1,0),

            };

            var HorizontalTestVelocity = new AbstractionCode.Vector2D[]
            {
                new AbstractionCode.Vector2D(1,0),
                new AbstractionCode.Vector2D(-1,0),
                new AbstractionCode.Vector2D(-1,0),
                new AbstractionCode.Vector2D(1,0),
                new AbstractionCode.Vector2D(1,0),
                new AbstractionCode.Vector2D(-2,0),
                new AbstractionCode.Vector2D(2,0),
            };

            yield return new object[] { HorizontalVectors, HorizontalTestVelocity };


            var DiagonalVectors = new AbstractionCode.Vector2D[]
            {
                new AbstractionCode.Vector2D(0,0),
                new AbstractionCode.Vector2D(1,1),
                new AbstractionCode.Vector2D(0,0),
                new AbstractionCode.Vector2D(-1,-1),
                new AbstractionCode.Vector2D(0,0),
                new AbstractionCode.Vector2D(1,-1),
                new AbstractionCode.Vector2D(0,0),
                new AbstractionCode.Vector2D(-1,1),
                new AbstractionCode.Vector2D(0,0),



            };

            var DiagonalTestVelocity = new AbstractionCode.Vector2D[]
            {
                new AbstractionCode.Vector2D(1,1),
                new AbstractionCode.Vector2D(-1,-1),
                new AbstractionCode.Vector2D(-1,-1),
                new AbstractionCode.Vector2D(1,1),
                new AbstractionCode.Vector2D(1,-1),
                new AbstractionCode.Vector2D(-1,1),
                new AbstractionCode.Vector2D(-1,1),
                new AbstractionCode.Vector2D(1,-1),
            };

            yield return new object[] { DiagonalVectors, DiagonalTestVelocity };
        }

        public static VelocityCalculator CreateSUT()
        {
            using IHost host = Host.CreateDefaultBuilder(new string[0])
                .ConfigureServices((_, services) =>
                    services.AddTransient<ServiceCache<Velocity>>()
                    .AddTransient<VelocityCalculator>())
                .Build();

            var servieces = host.Services;
            var Scope = servieces.CreateScope();
            var provider = Scope.ServiceProvider;
            return provider.GetRequiredService<VelocityCalculator>();
        }
    }
}
