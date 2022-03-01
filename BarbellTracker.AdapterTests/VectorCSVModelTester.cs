using BarbellTracker.Adapter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BarbellTracker.AdapterTests
{
    public class VectorCSVModelTester
    {
        private VectorCSVModel SUT = new VectorCSVModel();

        [Fact]
        public void GetHeader_WillReturnTheHeader()
        {
            var header = VectorCSVModel.GetHeader();

            var ExpectedHeader = "Time;Length;Vector";

            Assert.Equal(header, ExpectedHeader);
        }


        [Theory]
        [MemberData(nameof(VectorCSVModelWithExpectedTabel))]
        public void GetTable_WillReturnTheTableWithAllAddedItems(VectorCSVModel vectorCSVModel, List<VectorCSVItem> ExpectedTabel)
        {
            var ActualTabel = vectorCSVModel.GetTable();

            Assert.Equal(ExpectedTabel, ActualTabel);
        }

        [Theory]
        [MemberData(nameof(VectorCSVModelWithExpectedTabelString))]
        public void ToString_WillReturnTheTableWithAllAddedItemsAsaString(VectorCSVModel vectorCSVModel, string ExpectedTabel)
        {
            var ActualTabel = vectorCSVModel.ToString();

            Assert.Equal(ExpectedTabel, ActualTabel);
        }


        [Fact]
        public void Equal_OfVectorCSVModelAndNUll_WillReturnFalse()
        {
            VectorCSVModel vectorCSVModel = new VectorCSVModel();

            var IsEqual = vectorCSVModel.Equals(null);

            Assert.False(IsEqual);
        }

        [Fact]
        public void Equal_OfVectorCSVModelAndObject_WillReturnFalse()
        {
            VectorCSVModel vectorCSVModel = new VectorCSVModel();

            var IsEqual = vectorCSVModel.Equals(new object());

            Assert.False(IsEqual);
        }

        [Fact]
        public void Equal_OfTwoVectorCSVModelWithDifferentContentCount_WillReturnFalse()
        {
            var vectorCSVModelWithOneItem = new VectorCSVModel();
            vectorCSVModelWithOneItem.AddItem("00:00:01", 0, "{ x=0, y=1}");

            var vectorCSVModelWithTwoItem = new VectorCSVModel();
            vectorCSVModelWithTwoItem.AddItem("00:00:01", 0, "{ x=0, y=1}");
            vectorCSVModelWithTwoItem.AddItem("00:00:02", 1, "{ x=1, y=2}");


            var IsEqual = vectorCSVModelWithOneItem.Equals(vectorCSVModelWithTwoItem);

            Assert.False(IsEqual);
        }

        [Fact]
        public void Equal_OfTwoVectorCSVModelWithDifferentContent_WillReturnFalse()
        {
            var vectorCSVModelWithOneItem = new VectorCSVModel();
            vectorCSVModelWithOneItem.AddItem("00:00:01", 0, "{ x=0, y=1}");
            vectorCSVModelWithOneItem.AddItem("00:00:02", 1, "{ x=1, y=2}");

            var vectorCSVModelWithTwoItem = new VectorCSVModel();
            vectorCSVModelWithTwoItem.AddItem("00:00:03", 2, "{ x=3, y=4}");
            vectorCSVModelWithTwoItem.AddItem("00:00:04", 3, "{ x=5, y=6}");


            var IsEqual = vectorCSVModelWithOneItem.Equals(vectorCSVModelWithTwoItem);

            Assert.False(IsEqual);
        }



        [Fact]
        public void Equal_OfTwoVectorCSVModelWithSameContent_WillReturnTrue()
        {


            var vectorCSVModelWithOneItem = new VectorCSVModel();
            vectorCSVModelWithOneItem.AddItem("00:00:01", 0, "{ x=0, y=1}");
            vectorCSVModelWithOneItem.AddItem("00:00:02", 1, "{ x=1, y=2}");

            var vectorCSVModelWithTwoItem = new VectorCSVModel();
            vectorCSVModelWithTwoItem.AddItem("00:00:01", 0, "{ x=0, y=1}");
            vectorCSVModelWithTwoItem.AddItem("00:00:02", 1, "{ x=1, y=2}");


            var IsEqual = vectorCSVModelWithOneItem.Equals(vectorCSVModelWithTwoItem);

            Assert.True(IsEqual);
        }

        public static IEnumerable<object[]> VectorCSVModelWithExpectedTabel()
        {
            var EmptyTabel = new VectorCSVModel();
            var EmptyTabelList = new List<VectorCSVItem>();

            yield return new object[] { EmptyTabel, EmptyTabelList };


            var time = "00:00:50";
            var length = 0;
            var vector = "{ x=0, y=0 }";


            var OneItemTabel = new VectorCSVModel();
            OneItemTabel.AddItem(time, length, vector);


            var OneItemTabelList = new List<VectorCSVItem>();
            OneItemTabelList.Add(new VectorCSVItem(time, length, vector));

            yield return new object[] { OneItemTabel, OneItemTabelList };
        }

        public static IEnumerable<object[]> VectorCSVModelWithExpectedTabelString()
        {
            var EmptyTabel = new VectorCSVModel();
            var EmptyTabelList = "Time;Length;Vector\r\n";

            yield return new object[] { EmptyTabel, EmptyTabelList };


            var time = "00:00:50";
            var length = 0;
            var vector = "{ x=0, y=0 }";


            var OneItemTabel = new VectorCSVModel();
            OneItemTabel.AddItem(time, length, vector);


            var OneItemTabelString = "Time;Length;Vector\r\n00:00:50;0;{ x=0, y=0 }\r\n";

            yield return new object[] { OneItemTabel, OneItemTabelString.ToString() };
        }
    }
}
