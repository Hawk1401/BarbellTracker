using BarbellTracker.DomainCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BarbellTracker.DomainCodeTests
{
    public class TrackedInformationTester
    {

        [Fact]
        public void Equals_OfTrackedInformationAndNull_ReturnFalse()
        {
            //Arrange
            TrackedInformation trackedInformation = new TrackedInformation()
            {
                Id = "MyId"
            };

            //Act
            var isEqual = trackedInformation.Equals(null);

            //Assert
            Assert.False(isEqual);
        }

        [Fact]
        public void Equals_OfTrackedInformationAndObject_ReturnFalse()
        {
            //Arrange
            TrackedInformation trackedInformation = new TrackedInformation()
            {
                Id = "MyId"
            };

            //Act
            var isEqual = trackedInformation.Equals(new object());

            //Assert
            Assert.False(isEqual);
        }


        [Fact]
        public void Equals_OfTwoDiffrentsTrackedInformation_ReturnFalse()
        {
            //Arrange
            TrackedInformation trackedInformation = new TrackedInformation()
            {
                Id = "FristID"
            };
            TrackedInformation SecondtrackedInformation = new TrackedInformation()
            {
                Id = "SecondID"
            };

            //Act
            var isEqual = trackedInformation.Equals(SecondtrackedInformation);

            //Assert
            Assert.False(isEqual);
        }


        [Fact]
        public void Equals_OfTwoDiffrentsTrackedInformationWithSameId_ReturnTrue()
        {
            //Arrange
            TrackedInformation trackedInformation = new TrackedInformation()
            {
                Id = "SameId"
            };
            TrackedInformation SecondtrackedInformation = new TrackedInformation()
            {
                Id = "SameId"
            };

            //Act
            var isEqual = trackedInformation.Equals(SecondtrackedInformation);

            //Assert
            Assert.True(isEqual);
        }
    }
}
