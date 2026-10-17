using System;
using System.Globalization;
using System.IO;
using System.Linq;
using BarbellTracker.Adapter;
using BarbellTracker.Adapter.Model;
using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;
using BarbellTracker.DomainCode;
using BarbellTracker.Plugins.Processing;
using BarbellTracker.Plugins.Tracker;
using BarbellTracker.Services;
using BarbellTracker.Services.Implementation;
using Xunit;
using static BarbellTracker.ApplicationCode.IEventSystem;

namespace BarbellTracker.PluginsTests
{
    /// <summary>
    /// Runs the whole event chain like the application does:
    /// start extraction => json loader asks for a file => file selected => processing plugins write csv files.
    /// </summary>
    public class JsonToCsvPipelineTester : IDisposable
    {
        private readonly string outputFolder = Path.Combine(Path.GetTempPath(), "BarbellTrackerPipeline-" + Guid.NewGuid());
        private readonly string testData = Path.Combine(AppContext.BaseDirectory, "TestData");

        private readonly EventSystem events = new EventSystem();
        private readonly PluginManager pluginManager;
        private readonly FileManager fileManager;
        private readonly JsonLoader jsonLoader;
        private readonly UIAdapterManager adapterManager;

        private int extractedEvents;
        private TrackedInformation lastExtracted;

        public JsonToCsvPipelineTester()
        {
            // the csv writes numbers with the current culture, tests must not depend on the machine
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            pluginManager = new PluginManager(events);
            fileManager = new FileManager(events) { FolderPath = outputFolder };

            var settings = new AnalysisSettings();
            var motion = new PhysicalMotionCalculator(new ServiceCache<PhysicalMotion>(), settings);
            var analyzer = new SetAnalyzer(new ServiceCache<SetAnalysis>(), motion, settings);
            var setCsv = new SetCSVTranslater(analyzer, new ServiceCache<SetCSVModel>());
            var velocityCsv = new VelocityCSVTranslater(new VelocityCalculator(new ServiceCache<Velocity>()), new ServiceCache<VelocityCSVModel>());

            var accelerationCalculator = new AccelerationCalculator(new ServiceCache<Acceleration>(), new VelocityCalculator(new ServiceCache<Velocity>()));
            var accelerationCsv = new AccelerationCSVTranslater(accelerationCalculator, new ServiceCache<AccelerationCSVModel>());
            adapterManager = new UIAdapterManager(events);

            jsonLoader = new JsonLoader(events, pluginManager);
            new AccelerationToCSVFile(pluginManager, accelerationCsv, fileManager, events);
            new VelocityToAdapterTable(pluginManager, adapterManager, velocityCsv, events);
            new AccelerationToAdapterTable(pluginManager, adapterManager, accelerationCsv, events);
            new SetMetricsToCSVFile(pluginManager, setCsv, fileManager, events);
            new VelocityToCSVFile(pluginManager, velocityCsv, fileManager, events);

            EventDelegate<ExtracedVideoInfo> observer = info =>
            {
                extractedEvents++;
                lastExtracted = info.trackedInformation;
            };
            events.Subscribe(observer);
        }

        public void Dispose()
        {
            if (Directory.Exists(outputFolder))
            {
                Directory.Delete(outputFolder, true);
            }
        }

        /// <summary>Answers the file request of the tracker like the file dialog of the UI would</summary>
        private void AnswerFileRequestWith(string path)
        {
            EventDelegate<SelectFile> answer = _ => events.Fire(new FileSelected() { FilePath = path });
            events.Subscribe(answer);
        }

        private void StartExtraction(string name = "test", string trackerName = "JsonLoader")
        {
            events.Fire(new StartExtractVideoInfo()
            {
                StartExtractionInformation = new StartExtractionInformation() { ExtractionName = name, PluginName = trackerName }
            });
        }

        private static string Normalize(string text) => text.Replace("\r\n", "\n").TrimEnd();

        [Fact]
        public void SquatSet_ProducesTheExpectedMetricsCsv()
        {
            AnswerFileRequestWith(Path.Combine(testData, "squat_set.json"));
            pluginManager.TurnAllProcessingPluginOn();

            StartExtraction();

            var actual = File.ReadAllText(Path.Combine(fileManager.CruuentExtractionFolder, "SetMetrics.CSV"));
            var golden = File.ReadAllText(Path.Combine(testData, "squat_set.SetMetrics.golden.csv"));
            Assert.Equal(Normalize(golden), Normalize(actual));
        }

        [Fact]
        public void SquatSet_AlsoWritesTheVelocityCsv()
        {
            AnswerFileRequestWith(Path.Combine(testData, "squat_set.json"));
            pluginManager.TurnAllProcessingPluginOn();

            StartExtraction();

            var lines = File.ReadAllLines(Path.Combine(fileManager.CruuentExtractionFolder, "Velocity.CSV"));
            Assert.Equal(VectorCSVModel.GetHeader(), lines[0]);
            Assert.True(lines.Length > 100);
        }

        [Fact]
        public void SquatSet_AlsoWritesTheAccelerationCsv()
        {
            AnswerFileRequestWith(Path.Combine(testData, "squat_set.json"));
            pluginManager.TurnAllProcessingPluginOn();

            StartExtraction();

            var lines = File.ReadAllLines(Path.Combine(fileManager.CruuentExtractionFolder, "Acceleration.CSV"));
            Assert.Equal(VectorCSVModel.GetHeader(), lines[0]);
            Assert.True(lines.Length > 100);
        }

        [Fact]
        public void SquatSet_ShowsVelocityAndAccelerationTablesInTheUi()
        {
            AnswerFileRequestWith(Path.Combine(testData, "squat_set.json"));
            pluginManager.TurnAllProcessingPluginOn();

            StartExtraction();

            Assert.True(adapterManager.TryGetUIAdapterByName("Velocity Squat set (3 reps)", out var velocity));
            Assert.True(adapterManager.TryGetUIAdapterByName("Acceleration Squat set (3 reps)", out var acceleration));
            Assert.NotEmpty(((UICSVVectorAdapter)velocity).CSV.GetTable());
            Assert.NotEmpty(((UICSVVectorAdapter)acceleration).CSV.GetTable());
        }

        [Fact]
        public void LoadedData_GetsAnIdThatIsUniquePerExtraction()
        {
            AnswerFileRequestWith(Path.Combine(testData, "squat_set.json"));

            StartExtraction("first");
            var firstId = lastExtracted.Id;
            StartExtraction("second");

            Assert.Equal(2, extractedEvents);
            Assert.NotEqual(firstId, lastExtracted.Id);
            Assert.Equal("Squat set (3 reps)", lastExtracted.Name);
            Assert.Equal(30, lastExtracted.FrameRate);
        }

        [Fact]
        public void DeactivatedPlugins_WriteNothing()
        {
            AnswerFileRequestWith(Path.Combine(testData, "squat_set.json"));
            pluginManager.TurnAllProcessingPluginOn();
            pluginManager.TurnAllProcessingPluginOff();

            StartExtraction();

            Assert.Equal(1, extractedEvents);
            Assert.False(Directory.Exists(fileManager.CruuentExtractionFolder));
        }

        [Fact]
        public void OtherTrackerName_IsIgnoredByTheJsonLoader()
        {
            var asked = false;
            EventDelegate<SelectFile> observer = _ => asked = true;
            events.Subscribe(observer);

            StartExtraction(trackerName: "SomeOtherTracker");

            Assert.False(asked);
            Assert.Equal(0, extractedEvents);
        }

        [Theory]
        [InlineData("does-not-exist.json")]
        [InlineData("broken.json")]
        [InlineData("empty-object.json")]
        public void UnusableFile_DoesNotThrowAndGivesNoResult(string fileName)
        {
            Directory.CreateDirectory(outputFolder);
            File.WriteAllText(Path.Combine(outputFolder, "broken.json"), "{ not json");
            File.WriteAllText(Path.Combine(outputFolder, "empty-object.json"), "{}");
            AnswerFileRequestWith(Path.Combine(outputFolder, fileName));
            pluginManager.TurnAllProcessingPluginOn();

            var exception = Record.Exception(() => StartExtraction());

            Assert.Null(exception);
            Assert.Equal(0, extractedEvents);
        }

        [Fact]
        public void JsonLoader_OnlyReactsToTheFileItAskedFor()
        {
            AnswerFileRequestWith(Path.Combine(testData, "squat_set.json"));
            StartExtraction();
            Assert.Equal(1, extractedEvents);

            // a file selection that has nothing to do with the json loader anymore
            events.Fire(new FileSelected() { FilePath = Path.Combine(testData, "squat_set.json") });

            Assert.Equal(1, extractedEvents);
        }

        [Fact]
        public void Pipeline_RegistersAllPlugins()
        {
            Assert.Equal(new[] { "JsonLoader" }, pluginManager.GetTrackerPlugins().Select(p => p.Name));
            Assert.Contains("SetMetricsToCSVFile", pluginManager.GetProcessingPlugins().Select(p => p.Name));
            Assert.NotNull(jsonLoader);
        }
    }
}
