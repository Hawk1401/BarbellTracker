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
            var AdapterName = "TestAdapterName";
            var Adapter = GetAdapter(AdapterName);
            SUT.AddNewAdapter(Adapter);

            var Exist = SUT.TryGetUIAdapterByName(AdapterName, out var RequestedAdapter);


            Assert.True(Exist);
            Assert.Same(Adapter, RequestedAdapter);
        }

        [Fact]
        public void Request_ANotExistingAdapterByName_WillRetrunFalseAndNull()
        {
            var AdapterName = "TestAdapterName";
            var Adapter = GetAdapter(AdapterName);
            SUT.AddNewAdapter(Adapter);


            var NotExistingAdapterName = "NotExistingTestAdapterName";

            var Exist = SUT.TryGetUIAdapterByName(NotExistingAdapterName, out var RequestedAdapter);


            Assert.False(Exist);
            Assert.Null(RequestedAdapter);
        }

        [Fact]
        public void Add_AExistingAdapterName_WillReplaceTheAdapter()
        {
            var AdapterName = "TestAdapterName";
            var Adapter = GetAdapter(AdapterName);
            SUT.AddNewAdapter(Adapter);



            var ReplacingAdapter = GetAdapter(AdapterName);
            SUT.AddNewAdapter(ReplacingAdapter);

            var Exist = SUT.TryGetUIAdapterByName(AdapterName, out var RequestedAdapter);

            Assert.True(Exist);
            Assert.NotSame(Adapter, RequestedAdapter);
            Assert.Same(ReplacingAdapter, RequestedAdapter);
        }

        [Fact]
        public void TryIsType_WithTheRigthType_WillReturnTrue()
        {
            var AdapterName = "TestAdapterName";
            var Adapter = GetAdapter(AdapterName);
            var type = Adapter.GetType();
            SUT.AddNewAdapter(Adapter);


            var IsType = SUT.TryIsType(AdapterName, type);


            Assert.True(IsType);
        }

        [Fact]

        public void TryIsType_WithTheWongType_WillReturnFalse()
        {
            var AdapterName = "TestAdapterName";
            var Adapter = GetAdapter(AdapterName);
            var type = new object().GetType();
            SUT.AddNewAdapter(Adapter);


            var IsType = SUT.TryIsType(AdapterName, type);

            Assert.False(IsType);
        }

        [Fact]

        public void TryIsType_WithNotExistingName_WillReturnFalse()
        {
            var AdapterName = "TestAdapterName";
            var Adapter = GetAdapter(AdapterName);
            var type = Adapter.GetType();
            SUT.AddNewAdapter(Adapter);

            var NotExistingAdapterName = "NotExistingTestAdapterName";


            var IsType = SUT.TryIsType(NotExistingAdapterName, type);

            Assert.False(IsType);


        }

        public IUIAdapter GetAdapter(string name)
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
