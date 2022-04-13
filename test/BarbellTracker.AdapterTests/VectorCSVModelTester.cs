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
            //Arrange
            var ExpectedHeader = "Time;Length;Vector";

            //Act
            var header = VectorCSVModel.GetHeader();

            //Assert
            Assert.Equal(header, ExpectedHeader);
        }


        [Theory]
        [MemberData(nameof(VectorCSVModelWithExpectedTabel))]
        public void GetTable_WillReturnTheTableWithAllAddedItems(VectorCSVModel vectorCSVModel, List<VectorCSVItem> ExpectedTabel)
        {
            //Act
            var ActualTabel = vectorCSVModel.GetTable();

            //Assert
            Assert.Equal(ExpectedTabel, ActualTabel);
        }

        [Theory]
        [MemberData(nameof(VectorCSVModelWithExpectedTabelString))]
        public void ToString_WillReturnTheTableWithAllAddedItemsAsaString(VectorCSVModel vectorCSVModel, string ExpectedTabel)
        {
            //Act
            var ActualTabel = vectorCSVModel.ToString();

            //Assert
            Assert.Equal(ExpectedTabel, ActualTabel);
        }


        [Fact]
        public void Equal_OfVectorCSVModelAndNUll_WillReturnFalse()
        {
            //Arrange
            VectorCSVModel vectorCSVModel = new VectorCSVModel();

            //Act
            var IsEqual = vectorCSVModel.Equals(null);

            //Assert
            Assert.False(IsEqual);
        }

        [Fact]
        public void Equal_OfVectorCSVModelAndObject_WillReturnFalse()
        {
            //Arrange
            VectorCSVModel vectorCSVModel = new VectorCSVModel();

            //Act
            var IsEqual = vectorCSVModel.Equals(new object());

            //Assert
            Assert.False(IsEqual);
        }

        [Fact]
        public void Equal_OfTwoVectorCSVModelWithDifferentContentCount_WillReturnFalse()
        {
            //Arrange
            var vectorCSVModelWithOneItem = new VectorCSVModel();
            vectorCSVModelWithOneItem.AddItem("00:00:01", 0, "{ x=0, y=1}");

            var vectorCSVModelWithTwoItem = new VectorCSVModel();
            vectorCSVModelWithTwoItem.AddItem("00:00:01", 0, "{ x=0, y=1}");
            vectorCSVModelWithTwoItem.AddItem("00:00:02", 1, "{ x=1, y=2}");


            //Act
            var IsEqual = vectorCSVModelWithOneItem.Equals(vectorCSVModelWithTwoItem);

            //Assert
            Assert.False(IsEqual);
        }

        [Fact]
        public void Equal_OfTwoVectorCSVModelWithDifferentContent_WillReturnFalse()
        {
            //Arrange
            var vectorCSVModelWithOneItem = new VectorCSVModel();
            vectorCSVModelWithOneItem.AddItem("00:00:01", 0, "{ x=0, y=1}");
            vectorCSVModelWithOneItem.AddItem("00:00:02", 1, "{ x=1, y=2}");

            var vectorCSVModelWithTwoItem = new VectorCSVModel();
            vectorCSVModelWithTwoItem.AddItem("00:00:03", 2, "{ x=3, y=4}");
            vectorCSVModelWithTwoItem.AddItem("00:00:04", 3, "{ x=5, y=6}");


            //Act
            var IsEqual = vectorCSVModelWithOneItem.Equals(vectorCSVModelWithTwoItem);

            //Assert
            Assert.False(IsEqual);
        }



        [Fact]
        public void Equal_OfTwoVectorCSVModelWithSameContent_WillReturnTrue()
        {
            //Arrange
            var vectorCSVModelWithOneItem = new VectorCSVModel();
            vectorCSVModelWithOneItem.AddItem("00:00:01", 0, "{ x=0, y=1}");
            vectorCSVModelWithOneItem.AddItem("00:00:02", 1, "{ x=1, y=2}");

            var vectorCSVModelWithTwoItem = new VectorCSVModel();
            vectorCSVModelWithTwoItem.AddItem("00:00:01", 0, "{ x=0, y=1}");
            vectorCSVModelWithTwoItem.AddItem("00:00:02", 1, "{ x=1, y=2}");


            //Act
            var IsEqual = vectorCSVModelWithOneItem.Equals(vectorCSVModelWithTwoItem);

            //Assert
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
