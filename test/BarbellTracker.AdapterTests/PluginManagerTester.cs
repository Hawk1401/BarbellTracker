using Autofac.Extras.Moq;
using BarbellTracker.Adapter;
using BarbellTracker.Adapter.Interface;
using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BarbellTracker.AdapterTests
{
    public  class PluginManagerTester
    {
        private PluginManager SUT  = new PluginManager(new ApplicationCode.EventSystem());

        [Fact]
        public void Add_Aplugin_WillSendTheAddedPluginOverTheEventsystem()
        {
            using (var mockAuto = AutoMock.GetStrict())
            {
                //Arrange
                var plugin = CreateIProcessingPlugin("pluginName");
            
                var pluginLoaded = new PluginLoaded() { PluginName = plugin.Name };
                mockAuto.Mock<IEventSystem>()
                    .Setup(x => x.Fire(pluginLoaded))
                    .Returns(true);

                var repoMock = new Moq.Mock<IEventSystem>();
                repoMock.Setup(x => x.Fire(Moq.It.Is<PluginLoaded>(x => x.PluginName == plugin.Name))).Returns(true);
                var eventSystem = repoMock.Object;
                var sut = new PluginManager(eventSystem);


                //Act
                sut.AddPlugin(plugin);

                //Assert
                repoMock.VerifyAll();
            }
        }

        [Theory]
        [MemberData(nameof(ListsofIProcessingAndITrackerPlugin))]
        public void RequestAllProcessingPlugin_FromThePluginManager_WillReturnAllProcessingPluginsInstance(IProcessingPlugin[] processingPlugins, ITrackerPlugin[] ITrackerPlugins)
        {
            //Arrange
            foreach (var processingPlugin in processingPlugins)
            {
                SUT.AddPlugin(processingPlugin);
            }

            foreach (var ITrackerPlugin in ITrackerPlugins)
            {
                SUT.AddPlugin(ITrackerPlugin);
            }
            var expected = processingPlugins;


            //Act
            var actual = SUT.GetProcessingPlugins();


            //Assert
            Assert.Equal(expected.Length, actual.Count);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], actual[i]);
            }
        }


        [Theory]
        [MemberData(nameof(ListsofIProcessingAndITrackerPlugin))]
        public void RequestAllTrackerPlugin_FromThePluginManager_WillReturnAllTrackerPluginsInstance(IProcessingPlugin[] processingPlugins, ITrackerPlugin[] ITrackerPlugins)
        {
            //Arrange
            foreach (var processingPlugin in processingPlugins)
            {
                SUT.AddPlugin(processingPlugin);
            }

            foreach (var ITrackerPlugin in ITrackerPlugins)
            {
                SUT.AddPlugin(ITrackerPlugin);
            }

            var expected = ITrackerPlugins;


            //Act
            var actual = SUT.GetTrackerPlugins();


            //Assert
            Assert.Equal(expected.Length, actual.Count);

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], actual[i]);
            }
        }


        [Theory]
        [InlineData(0,0)]
        [InlineData(5, 0)]
        [InlineData(0, 5)]
        [InlineData(5, 5)]
        public void Request_AExistingProcessingPlugin_WillReturnTrueAndThePlugin(int ProcessingPluginCount, int TrackerPluginCount)
        {
            //Arrange
            FillPluginManagerWithPlugins(ProcessingPluginCount, TrackerPluginCount);

            var PluginName = "MySecretPluginName";
            var expected = CreateIProcessingPlugin(PluginName);
            SUT.AddPlugin(expected);

            //Act
            var containsThePlugin = SUT.TryGetProcessingPluginByName(PluginName, out var actual);

            //Assert
            Assert.True(containsThePlugin);
            Assert.StrictEqual(expected, actual);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(5, 0)]
        [InlineData(0, 5)]
        [InlineData(5, 5)]
        public void Request_ANotExistingProcessingPlugin_WillReturnFalseAndNull(int ProcessingPluginCount, int TrackerPluginCount)
        {
            //Arrange
            FillPluginManagerWithPlugins(ProcessingPluginCount, TrackerPluginCount);
            var PluginName = "MySecretPluginName";

            //Act
            var containsThePlugin = SUT.TryGetProcessingPluginByName(PluginName, out var actual);
            
            //Assert
            Assert.False(containsThePlugin);
            Assert.Null(actual);
        }


        [Theory]
        [InlineData(0, 0)]
        [InlineData(5, 0)]
        [InlineData(0, 5)]
        [InlineData(5, 5)]
        public void Request_AExistingTrackerPlugin_WillReturnTrueAndThePlugin(int ProcessingPluginCount, int TrackerPluginCount)
        {
            //Arrange
            FillPluginManagerWithPlugins(ProcessingPluginCount, TrackerPluginCount);

            var PluginName = "MySecretPluginName";
            var expected = CreateITrackerPlugin(PluginName);
            SUT.AddPlugin(expected);

            //Act
            var containsThePlugin = SUT.TryGetTrackerPluginByName(PluginName, out var actual);

            //Assert
            Assert.True(containsThePlugin);
            Assert.StrictEqual(expected, actual);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(5, 0)]
        [InlineData(0, 5)]
        [InlineData(5, 5)]
        public void Request_ANotExistingTrackerPlugin_WillReturnFalseAndNull(int ProcessingPluginCount, int TrackerPluginCount)
        {
            //Arrange
            FillPluginManagerWithPlugins(ProcessingPluginCount, TrackerPluginCount);

            var PluginName = "MySecretPluginName";

            //Act
            var containsThePlugin = SUT.TryGetTrackerPluginByName(PluginName, out var actual);


            //Assert
            Assert.False(containsThePlugin);
            Assert.Null(actual);
        }

        public void FillPluginManagerWithPlugins(int ProcessingPluginCount , int TrackerPluginCount)
        {
            for (int i = 0; i < ProcessingPluginCount; i++)
            {
                SUT.AddPlugin(CreateIProcessingPlugin("ProcessingDummyNumber" + i));
            }

            for (int i = 0; i < TrackerPluginCount; i++)
            {
                SUT.AddPlugin(CreateITrackerPlugin("TrackerDummyNumber" + i));
            }
        }

        public static IProcessingPlugin CreateIProcessingPlugin(string name)
        {
            using (var mockAuto = AutoMock.GetLoose())
            {
                mockAuto.Mock<IProcessingPlugin>()
                    .Setup(x => x.Name)
                    .Returns(name);

                return mockAuto.Create<IProcessingPlugin>();

            }
        }

        public static ITrackerPlugin CreateITrackerPlugin(string name)
        {
            using (var mockAuto = AutoMock.GetLoose())
            {
                mockAuto.Mock<ITrackerPlugin>()
                    .Setup(x => x.Name)
                    .Returns(name);

                return mockAuto.Create<ITrackerPlugin>();

            }
        }


        public static IEnumerable<object[]> ListsofIProcessingAndITrackerPlugin()
        {
            var SixIProcessingPlugins = new IProcessingPlugin[]
            {
                CreateIProcessingPlugin("ProcessingPlugin_1"),
                CreateIProcessingPlugin("ProcessingPlugin_2"),
                CreateIProcessingPlugin("ProcessingPlugin_3"),
                CreateIProcessingPlugin("ProcessingPlugin_4"),
                CreateIProcessingPlugin("ProcessingPlugin_5"),
                CreateIProcessingPlugin("ProcessingPlugin_6"),
            };

            var SixTrackerPlugins = new ITrackerPlugin[]
            {
                CreateITrackerPlugin("ITrackerPlugin_1"),
                CreateITrackerPlugin("ITrackerPlugin_2"),
                CreateITrackerPlugin("ITrackerPlugin_3"),
                CreateITrackerPlugin("ITrackerPlugin_4"),
                CreateITrackerPlugin("ITrackerPlugin_5"),
                CreateITrackerPlugin("ITrackerPlugin_6"),
            };

            yield return new object[] { SixIProcessingPlugins, new ITrackerPlugin[0] };
            yield return new object[] { new IProcessingPlugin[0], SixTrackerPlugins };
            yield return new object[] { SixIProcessingPlugins, SixTrackerPlugins };

        }

    }
}
