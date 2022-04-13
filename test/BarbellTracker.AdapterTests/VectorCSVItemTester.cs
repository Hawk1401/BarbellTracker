using BarbellTracker.AbstractionCode;
using BarbellTracker.Adapter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BarbellTracker.AdapterTests
{
    public class VectorCSVItemTester
    {

        [Theory]
        [MemberData(nameof(VectorCSVItems))]
        public void Copy_ofAVectorCSVItem_willReturnASimilarItem(VectorCSVItem vectorCSVItem)
        {
            
            //Act
            var copy = vectorCSVItem.Copy();

            //Assert
            Assert.Equal(vectorCSVItem, copy);
            Assert.NotSame(vectorCSVItem, copy);
        }

        [Fact]
        public void Equals_OfAVectorCSVItemAndNull_WillReturnFalse()
        { 
            //Act
            var vector = DummyVectorCSVItem();

            //Assert
            Assert.False(vector.Equals(null));
        }

        [Fact]
        public void Equals_OfAVectorCSVItemAndObject_WillReturnFalse()
        {
            //Act
            var vector = DummyVectorCSVItem();

            //Assert
            Assert.False(vector.Equals(new object()));
        }

        [Fact]
        public void Equals_OfTwoSimilarVectorCSVItems_WillReturnTrue()
        {
            //Arrange
            var vector1 = DummyVectorCSVItem();
            var vector2 = DummyVectorCSVItem();
            
            //Act
            var Equals = vector1.Equals(vector2);
            
            //Assert
            Assert.True(Equals);
        }

        [Fact]
        public void Equals_OfTwoVectorCSVItemsWithDiffentTime_WillReturnFasle()
        {
            //Arrange
            var time = "00:00:01";
            var length = 0;
            var Vector = new Vector2D(0, 1);

            var vector1 = new VectorCSVItem(time, length, Vector.ToString());

            var newTime = "00:00:02";
            var vector2 = new VectorCSVItem(newTime, length, Vector.ToString());
            
            //Act
            var Equals = vector1.Equals(vector2);

            //Assert
            Assert.False(Equals);
        }
        [Fact]
        public void Equals_OfTwoVectorCSVItemsWithDiffentLength_WillReturnFasle()
        {
            //Arrange
            var time = "00:00:01";
            var length = 0;
            var Vector = new Vector2D(0, 1);

            var vector1 = new VectorCSVItem(time, length, Vector.ToString());

            var newLength = 1;
            var vector2 = new VectorCSVItem(time, newLength, Vector.ToString());

            //Act
            var Equals = vector1.Equals(vector2);


            //Assert
            Assert.False(Equals);
        }

        [Fact]
        public void Equals_OfTwoVectorCSVItemsWithDiffentVector_WillReturnFasle()
        {
            //Arrange
            var time = "00:00:01";
            var length = 0;
            var Vector = new Vector2D(0, 1);

            var VectorCSVItem1 = new VectorCSVItem(time, length, Vector.ToString());

            var newVector = new Vector2D(2, 3);
            var VectorCSVItem2 = new VectorCSVItem(time, length, newVector.ToString());


            //Act
            var Equals = VectorCSVItem1.Equals(VectorCSVItem2);


            //Assert
            Assert.False(Equals);
        }


        [Fact]
        public void TheHash_OfTwoSimilarVectorCSVItems_isTheSame()
        {
            //Arrange
            var vectorCSVItem = DummyVectorCSVItem();
            var copy = vectorCSVItem.Copy();

            //Act
            var OriginalHash = vectorCSVItem.GetHashCode();
            var CopyHash = copy.GetHashCode();

            //Assert
            Assert.StrictEqual(OriginalHash, CopyHash);
        }


        [Fact]
        public void TheHash_OfTwoDiffentVectorCSVItems_isTheNotSame()
        {
            //Arrange
            var vectorCSVItem = DummyVectorCSVItem();
            var DiffentvectorCSVItem = DiffentDummyVectorCSVItem();

            //Act
            var OriginalHash = vectorCSVItem.GetHashCode();
            var DiffentHash = DiffentvectorCSVItem.GetHashCode();

            //Assert
            Assert.NotStrictEqual(OriginalHash, DiffentHash);
        }

        public static IEnumerable<object[]> VectorCSVItems()
        {
            Vector2D vector2D1 = new Vector2D(0, 1);
            Vector2D vector2D2 = new Vector2D(1, 2);

            yield return new VectorCSVItem[] { new VectorCSVItem("00:00:01", 0, vector2D1.ToString()) }; // standert
            yield return new VectorCSVItem[] { new VectorCSVItem("00:00:02", 0, vector2D1.ToString()) }; // change time
            yield return new VectorCSVItem[] { new VectorCSVItem("00:00:01", 3, vector2D1.ToString()) }; // change length
            yield return new VectorCSVItem[] { new VectorCSVItem("00:00:01", 3, vector2D2.ToString()) }; // change Vector


        }
        public static VectorCSVItem DummyVectorCSVItem()
        {
            Vector2D vector2D1 = new Vector2D(0, 1);

            return new VectorCSVItem("00:00:01", 0, vector2D1.ToString());
        }

        public static VectorCSVItem DiffentDummyVectorCSVItem()
        {
            Vector2D vector2D1 = new Vector2D(1, 2);

            return new VectorCSVItem("00:00:02", 1, vector2D1.ToString());
        }
    }
}
