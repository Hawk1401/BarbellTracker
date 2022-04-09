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
            TrackedInformation  trackedInformation = new TrackedInformation()
            {
                Id = "MyId"
            };

            var isEqual = trackedInformation.Equals(null);
            Assert.False(isEqual);
        }

        [Fact]
        public void Equals_OfTrackedInformationAndObject_ReturnFalse()
        {
            TrackedInformation trackedInformation = new TrackedInformation()
            {
                Id = "MyId"
            };

            var isEqual = trackedInformation.Equals(new object());
            Assert.False(isEqual);
        }


        [Fact]
        public void Equals_OfTwoDiffrentsTrackedInformation_ReturnFalse()
        {
            TrackedInformation trackedInformation = new TrackedInformation()
            {
                Id = "FristID"
            };
            TrackedInformation SecondtrackedInformation = new TrackedInformation()
            {
                Id = "SecondID"
            };

            var isEqual = trackedInformation.Equals(SecondtrackedInformation);

            Assert.False(isEqual);
        }


        [Fact]
        public void Equals_OfTwoDiffrentsTrackedInformationWithSameId_ReturnTrue()
        {
            TrackedInformation trackedInformation = new TrackedInformation()
            {
                Id = "SameId"
            };
            TrackedInformation SecondtrackedInformation = new TrackedInformation()
            {
                Id = "SameId"
            };

            var isEqual = trackedInformation.Equals(SecondtrackedInformation);

            Assert.True(isEqual);
        }
    }
}
