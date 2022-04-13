using Autofac.Extras.Moq;
using BarbellTracker.AbstractionCode;
using BarbellTracker.Adapter.Model;
using BarbellTracker.Services;
using BarbellTracker.Services.Implementation;
using BarbellTracker.Services.Interface;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BarbellTracker.ServicesTests
{
    public class VelocityCSVTranslaterTester
    {


        [Fact]
        public void RequestCSVWithTheGetCSV_FromATrackedInformationObject_WillReturnTheRigthCSVObject()
        {
            //Arrange
            using (var mockAuto = AutoMock.GetLoose())
            {
                var tracked = new DomainCode.TrackedInformation() { Id = "myTestID" };
                var FirstVector = new Vector2D() { X = 0, Y = 1 };
                var SecondVector = new Vector2D() { X = 2, Y = 3 };
                var ThirdVector = new Vector2D() { X = 4, Y = 5 };
                var FPS = 1;

                var velocity = CreateVelocityObject(FPS, FirstVector, SecondVector, ThirdVector);


                var expected = new VectorCSVModel();
                expected.AddItem("00:00:", FirstVector.Length(), FirstVector.ToString());
                expected.AddItem("00:01:", SecondVector.Length(), SecondVector.ToString());
                expected.AddItem("00:02:", ThirdVector.Length(), ThirdVector.ToString());



                mockAuto.Mock<ICalculator<Velocity>>()
                    .Setup(x => x.GetCalculatedValue(tracked))
                    .Returns(velocity);

                var VelocityCalculatorMock = mockAuto.Create<ICalculator<Velocity>>();
                var sut = new VelocityCSVTranslater(VelocityCalculatorMock, new ServiceCache<VelocityCSVModel>());

                //Act
                var CSV = sut.GetCSV(tracked);

                //Assert
                Assert.Equal(expected, CSV);
            }

        }
        [Fact]
        public void GetCSV_WithSameKey_WillReturnTheSameIntance()
        {
            //Arrange
            using (var mockAuto = AutoMock.GetLoose())
            {
                var tracked = new DomainCode.TrackedInformation() { Id = "myTestID" };

                var FirstVector = new Vector2D() { X = 0, Y = 1 };
                var SecondVector = new Vector2D() { X = 2, Y = 3 };
                var ThirdVector = new Vector2D() { X = 4, Y = 5 };
                var FPS = 1;

                var velocity = CreateVelocityObject(FPS, FirstVector, SecondVector, ThirdVector);

                mockAuto.Mock<ICalculator<Velocity>>()
                    .Setup(x => x.GetCalculatedValue(tracked))
                    .Returns(velocity);

                var VelocityCalculatorMock = mockAuto.Create<ICalculator<Velocity>>();
                var sut = new VelocityCSVTranslater(VelocityCalculatorMock, new ServiceCache<VelocityCSVModel>());



                var expected = sut.GetCSV(tracked);
                var CSV = sut.GetCSV(tracked);


                //Act
                var saneReference = object.ReferenceEquals(expected, CSV);
                
                //Assert
                Assert.True(saneReference);
            }

        }
        [Fact]
        public void RequestCSVWithTheCreateCSV_FromAVelocityObject_WillReturnTheRigthCSVObject()
        {
            //Arrange
            var sut = new VelocityCSVTranslater(new VelocityCalculator(new ServiceCache<Velocity>()) , new ServiceCache<VelocityCSVModel>());
            var FirstVector = new Vector2D() { X = 0, Y = 1 };
            var SecondVector = new Vector2D() { X = 2, Y = 3 };
            var ThirdVector = new Vector2D() { X = 4, Y = 5 };
            var FPS = 30;

            var velocity = CreateVelocityObject(FPS, FirstVector, SecondVector, ThirdVector);

            var expected = new VectorCSVModel();
            expected.AddItem("00:00:", FirstVector.Length(), FirstVector.ToString());
            expected.AddItem("00:00:03", SecondVector.Length(), SecondVector.ToString());
            expected.AddItem("00:00:06", ThirdVector.Length(), ThirdVector.ToString());


            //Act
            var CSV = sut.CreateCSV(velocity);


            //Assert
            Assert.Equal(expected, CSV);

        }


        public Velocity CreateVelocityObject(int FPS, params Vector2D[] vector2Ds)
        {

            return new Velocity()
            {
                FPS = FPS,
                Vectors = vector2Ds
            };
        }
    }
}
