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
            var tracked = new DomainCode.TrackedInformation();
            var FirstVector = new Vector2D() { X = 0, Y = 1 };
            var SecondVector = new Vector2D() { X = 2, Y = 3 };
            var ThirdVector = new Vector2D() { X = 4, Y = 5 };
            var FPS = 1;

            var velocity = CreateVelocityObject(FPS, FirstVector, SecondVector, ThirdVector);


            var expected = new VectorCSVModel();
            expected.AddItem("00:00:", FirstVector.length(), FirstVector.ToString());
            expected.AddItem("00:01:", SecondVector.length(), SecondVector.ToString());
            expected.AddItem("00:02:", ThirdVector.length(), ThirdVector.ToString());


            using (var mockAuto = AutoMock.GetLoose())
            {
                mockAuto.Mock<ICalculator<Velocity>>()
                    .Setup(x => x.GetCalculatedValue(tracked))
                    .Returns(velocity);
                 
                var VelocityCalculatorMock = mockAuto.Create<ICalculator<Velocity>>();
                var sut = new VelocityCSVTranslater(VelocityCalculatorMock, new ServiceCache<VectorCSVModel>());

                var CSV = sut.GetCSV(tracked);
                Assert.Equal(expected, CSV);
            }

        }

        [Fact]
        public void RequestCSVWithTheCreateCSV_FromAVelocityObject_WillReturnTheRigthCSVObject()
        {
            var sut = new VelocityCSVTranslater(new VelocityCalculator(new ServiceCache<Velocity>()) , new ServiceCache<VectorCSVModel>());
            var FirstVector = new Vector2D() { X = 0, Y = 1 };
            var SecondVector = new Vector2D() { X = 2, Y = 3 };
            var ThirdVector = new Vector2D() { X = 4, Y = 5 };
            var FPS = 1;

            var velocity = CreateVelocityObject(FPS, FirstVector, SecondVector, ThirdVector);

            var expected = new VectorCSVModel();
            expected.AddItem("00:00:", FirstVector.length(), FirstVector.ToString());
            expected.AddItem("00:01:", SecondVector.length(), SecondVector.ToString());
            expected.AddItem("00:02:", ThirdVector.length(), ThirdVector.ToString());

            var CSV = sut.CreateCSV(velocity);

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
