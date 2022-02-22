using System;
using System.Collections.Generic;
using BarbellTracker.AbstractionCode;
using BarbellTracker.DomainCode;
using BarbellTracker.Services.Implementation;
using Xunit;

namespace BarbellTracker.ServicesTests
{
    public class VelocityCalculatorTester
    {

        VelocityCalculator _sut = VelocityCalculator.Instance;

        [Fact]
        public void Test1()
        {

            var vectors = new AbstractionCode.Vector2D[]
            {
                new AbstractionCode.Vector2D(0,0),
                new AbstractionCode.Vector2D(1,0),
                new AbstractionCode.Vector2D(0,0),
                new AbstractionCode.Vector2D(0,1),
                new AbstractionCode.Vector2D(0,0),
                new AbstractionCode.Vector2D(-1,0),
                new AbstractionCode.Vector2D(0,0),
                new AbstractionCode.Vector2D(0,-1),
                new AbstractionCode.Vector2D(0,0),
            };
            //TrackedInformation trackedInfos = new TrackedInformation()
            //{
            //    FrameRate = 30,
            //    Id = "MyTestId",
            //    PixelPerCm = 300,
            //    Name = "myTestName",
            //    Positions = 
            //}
            //_sut.GetVelocity();

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


            var velocity = _sut.GetVelocity(trackedInfos);


            Assert.Equal(velocity.Vectors.Length, Velocity.Length);
            for (int i = 0; i < velocity.Vectors.Length; i++)
            {
                Assert.Equal(velocity.Vectors[i], Velocity[i]);
            }
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
    }
}
