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
            var copy = vectorCSVItem.Copy();

            Assert.Equal(vectorCSVItem, copy);
            Assert.NotSame(vectorCSVItem, copy);
        }

        [Fact]
        public void Equals_OfAVectorCSVItemAndNull_WillReturnFalse()
        {
            var vector = DummyVectorCSVItem();

            Assert.False(vector.Equals(null));
        }

        [Fact]
        public void Equals_OfAVectorCSVItemAndObject_WillReturnFalse()
        {
            var vector = DummyVectorCSVItem();

            Assert.False(vector.Equals(new object()));
        }

        [Fact]
        public void Equals_OfTwoSimilarVectorCSVItems_WillReturnTrue()
        {
            var vector1 = DummyVectorCSVItem();
            var vector2 = DummyVectorCSVItem();

            Assert.True(vector1.Equals(vector2));
        }

        [Fact]
        public void Equals_OfTwoVectorCSVItemsWithDiffentTime_WillReturnFasle()
        {
            var time = "00:00:01";
            var length = 0;
            var Vector = new Vector2D(0, 1);

            var vector1 = new VectorCSVItem(time, length, Vector.ToString());

            var newTime = "00:00:02";
            var vector2 = new VectorCSVItem(newTime, length, Vector.ToString());

            Assert.False(vector1.Equals(vector2));
        }
        [Fact]
        public void Equals_OfTwoVectorCSVItemsWithDiffentLength_WillReturnFasle()
        {
            var time = "00:00:01";
            var length = 0;
            var Vector = new Vector2D(0, 1);

            var vector1 = new VectorCSVItem(time, length, Vector.ToString());

            var newLength = 1;
            var vector2 = new VectorCSVItem(time, newLength, Vector.ToString());

            Assert.False(vector1.Equals(vector2));
        }

        [Fact]
        public void Equals_OfTwoVectorCSVItemsWithDiffentVector_WillReturnFasle()
        {
            var time = "00:00:01";
            var length = 0;
            var Vector = new Vector2D(0, 1);

            var VectorCSVItem1 = new VectorCSVItem(time, length, Vector.ToString());

            var newVector = new Vector2D(2, 3);
            var VectorCSVItem2 = new VectorCSVItem(time, length, newVector.ToString());

            Assert.False(VectorCSVItem1.Equals(VectorCSVItem2));
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
    }
}
