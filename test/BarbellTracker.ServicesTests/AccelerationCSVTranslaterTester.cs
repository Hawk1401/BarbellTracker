using Autofac.Extras.Moq;
using BarbellTracker.AbstractionCode;
using BarbellTracker.Adapter.Model;
using BarbellTracker.Services;
using BarbellTracker.Services.Implementation;
using BarbellTracker.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BarbellTracker.ServicesTests
{
    public class AccelerationCSVTranslaterTester
    {


        [Fact]
        public void RequestCSVWithTheGetCSV_FromATrackedInformationObject_WillReturnTheRigthCSVObject()
        {
            using (var mockAuto = AutoMock.GetLoose())
            {
                //Arrange
                var tracked = new DomainCode.TrackedInformation() { Id = "myTestID" };
                var FirstVector = new Vector2D() { X = 0, Y = 1 };
                var SecondVector = new Vector2D() { X = 2, Y = 3 };
                var ThirdVector = new Vector2D() { X = 4, Y = 5 };
                var FPS = 1;

                var acceleration = CreateVelocityObject(FPS, FirstVector, SecondVector, ThirdVector);


                var expected = new VectorCSVModel();
                expected.AddItem("00:00:", FirstVector.Length(), FirstVector.ToString());
                expected.AddItem("00:01:", SecondVector.Length(), SecondVector.ToString());
                expected.AddItem("00:02:", ThirdVector.Length(), ThirdVector.ToString());



                mockAuto.Mock<ICalculator<Acceleration>>()
                    .Setup(x => x.GetCalculatedValue(tracked))
                    .Returns(acceleration);

                var VelocityCalculatorMock = mockAuto.Create<ICalculator<Acceleration>>();
                var sut = new AccelerationCSVTranslater(VelocityCalculatorMock, new ServiceCache<AccelerationCSVModel>());

                //Act
                var CSV = sut.GetCSV(tracked);

                //Assert
                Assert.Equal(expected, CSV);
            }

        }


        [Fact]
        public void Request_CachedAcceleration_WillReturnTheCachedObjcet()
        {
            using (var mockAuto = AutoMock.GetLoose())
            {
                //Arrange
                var tracked = new DomainCode.TrackedInformation() { Id = "myTestID" };
                var FirstVector = new Vector2D() { X = 0, Y = 1 };
                var SecondVector = new Vector2D() { X = 2, Y = 3 };
                var ThirdVector = new Vector2D() { X = 4, Y = 5 };
                var FPS = 1;

                var acceleration = CreateVelocityObject(FPS, FirstVector, SecondVector, ThirdVector);




                mockAuto.Mock<ICalculator<Acceleration>>()
                    .Setup(x => x.GetCalculatedValue(tracked))
                    .Returns(acceleration);

                var VelocityCalculatorMock = mockAuto.Create<ICalculator<Acceleration>>();
                var sut = new AccelerationCSVTranslater(VelocityCalculatorMock, new ServiceCache<AccelerationCSVModel>());

                //Act
                var expected = sut.GetCSV(tracked);
                var CachedVersion = sut.GetCSV(tracked);

                //Assert
                Assert.StrictEqual(expected, CachedVersion);
            }

        }

        [Fact]
        public void RequestCSVWithTheCreateCSV_FromAVelocityObject_WillReturnTheRigthCSVObject()
        {
            //Arrange
            var sut = CreateSTU();
            var FirstVector = new Vector2D() { X = 0, Y = 1 };
            var SecondVector = new Vector2D() { X = 2, Y = 3 };
            var ThirdVector = new Vector2D() { X = 4, Y = 5 };
            var FPS = 1;

            var acceleration = CreateVelocityObject(FPS, FirstVector, SecondVector, ThirdVector);

            var expected = new VectorCSVModel();
            expected.AddItem("00:00:", FirstVector.Length(), FirstVector.ToString());
            expected.AddItem("00:01:", SecondVector.Length(), SecondVector.ToString());
            expected.AddItem("00:02:", ThirdVector.Length(), ThirdVector.ToString());


            //Act
            var CSV = sut.CreateCSV(acceleration);


            //Assert
            Assert.Equal(expected, CSV);
        }


        public Acceleration CreateVelocityObject(int FPS, params Vector2D[] vector2Ds)
        {

            return new Acceleration()
            {
                FPS = FPS,
                Vectors = vector2Ds
            };
        }

        public AccelerationCSVTranslater CreateSTU()
        {
            using IHost host = Host.CreateDefaultBuilder(new string[0])
                .ConfigureServices((_, services) =>
                    services.AddSingleton<ServiceCache<Velocity>>()
                    .AddSingleton<ServiceCache<Acceleration>>()
                    .AddSingleton<ServiceCache<AccelerationCSVModel>>()
                    .AddTransient<ICalculator<Velocity>, VelocityCalculator>()
                    .AddTransient<ICalculator<Acceleration>, AccelerationCalculator>()
                    .AddTransient<AccelerationCSVTranslater>()
                    )
                .Build();

            var servieces = host.Services;
            var Scope = servieces.CreateScope();
            var provider = Scope.ServiceProvider;

            return provider.GetRequiredService<AccelerationCSVTranslater>();
        }
    }
}

