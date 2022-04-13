using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using BarbellTracker.Adapter;
using BarbellTracker.Adapter.Interface;
using Xunit;

namespace BarbellTracker.AdapterTests
{
    public class UIAdapterManagerTester
    {
        private UIAdapterManager SUT = new UIAdapterManager(new ApplicationCode.EventSystem());


        [Fact]
        public void Request_AExistingAdapterByName_WillRetrunTrueAndTheAdapter()
        {
            //Arrange
            var AdapterName = "TestAdapterName";
            var Adapter = CreateAdapter(AdapterName);
            SUT.AddNewAdapter(Adapter);


            //Act
            var Exist = SUT.TryGetUIAdapterByName(AdapterName, out var RequestedAdapter);

            //Assert
            Assert.True(Exist);
            Assert.Same(Adapter, RequestedAdapter);
        }

        [Fact]
        public void Request_ANotExistingAdapterByName_WillRetrunFalseAndNull()
        {
            //Arrange
            var AdapterName = "TestAdapterName";
            var Adapter = CreateAdapter(AdapterName);
            SUT.AddNewAdapter(Adapter);

            var NotExistingAdapterName = "NotExistingTestAdapterName";

            //Act
            var Exist = SUT.TryGetUIAdapterByName(NotExistingAdapterName, out var RequestedAdapter);

            //Assert

            Assert.False(Exist);
            Assert.Null(RequestedAdapter);
        }

        [Fact]
        public void Add_AExistingAdapterName_WillReplaceTheAdapter()
        {
            //Arrange
            var AdapterName = "TestAdapterName";
            var Adapter = CreateAdapter(AdapterName);
            SUT.AddNewAdapter(Adapter);

            var ReplacingAdapter = CreateAdapter(AdapterName);
            SUT.AddNewAdapter(ReplacingAdapter);

            //Act
            var Exist = SUT.TryGetUIAdapterByName(AdapterName, out var RequestedAdapter);
            
            //Assert
            Assert.True(Exist);
            Assert.NotSame(Adapter, RequestedAdapter);
            Assert.Same(ReplacingAdapter, RequestedAdapter);
        }

        [Fact]
        public void TryIsType_WithTheRigthType_WillReturnTrue()
        {
            //Arrange
            var AdapterName = "TestAdapterName";
            var Adapter = CreateAdapter(AdapterName);
            var type = Adapter.GetType();
            SUT.AddNewAdapter(Adapter);

            //Act
            var IsType = SUT.TryIsType(AdapterName, type);

            //Assert
            Assert.True(IsType);
        }

        [Fact]

        public void TryIsType_WithTheWongType_WillReturnFalse()
        {
            //Arrange
            var AdapterName = "TestAdapterName";
            var Adapter = CreateAdapter(AdapterName);
            var type = new object().GetType();
            SUT.AddNewAdapter(Adapter);


            //Act
            var IsType = SUT.TryIsType(AdapterName, type);

            //Assert
            Assert.False(IsType);
        }

        [Fact]
        public void TryIsType_WithNotExistingName_WillReturnFalse()
        {
            //Arrange
            var AdapterName = "TestAdapterName";
            var Adapter = CreateAdapter(AdapterName);
            var type = Adapter.GetType();
            SUT.AddNewAdapter(Adapter);

            var NotExistingAdapterName = "NotExistingTestAdapterName";

            //Act
            var IsType = SUT.TryIsType(NotExistingAdapterName, type);

            //Assert
            Assert.False(IsType);


        }

        public IUIAdapter CreateAdapter(string name)
        {
            using (var mockAuto = AutoMock.GetLoose())
            {
                mockAuto.Mock<IUIAdapter>()
                    .Setup(x => x.Name)
                    .Returns(name);

                return mockAuto.Create<IUIAdapter>();

            }
        }
    }
}
