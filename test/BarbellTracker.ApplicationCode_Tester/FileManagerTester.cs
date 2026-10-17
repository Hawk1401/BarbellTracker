using System.IO;
using BarbellTracker.ApplicationCode;
using BarbellTracker.ApplicationCode.Event;
using BarbellTracker.DomainCode;
using Xunit;

namespace BarbellTracker.ApplicationCodeTests
{
    public class FileManagerTester : System.IDisposable
    {
        private readonly string folder = Path.Combine(Path.GetTempPath(), "BarbellTrackerTests-" + System.Guid.NewGuid());
        private readonly FileManager fileManager;

        public FileManagerTester()
        {
            fileManager = new FileManager(new EventSystem()) { FolderPath = folder };
            fileManager.NewExtractionStarted(new StartExtractVideoInfo()
            {
                StartExtractionInformation = new StartExtractionInformation() { ExtractionName = "test" }
            });
        }

        public void Dispose()
        {
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
        }

        [Fact]
        public void AppendLine_TwiceOnNewFile_ContainsBothLines()
        {
            Assert.True(fileManager.AppendLine("a.txt", "one"));
            Assert.True(fileManager.AppendLine("a.txt", "two"));

            Assert.True(fileManager.ReadContent("a.txt", out var content));
            Assert.Equal(new[] { "one", "two" }, content);
        }

        [Fact]
        public void Write_ExistingFileWithoutOverride_ReturnsFalseAndKeepsContent()
        {
            fileManager.Write("b.txt", "old");

            Assert.False(fileManager.Write("b.txt", "new", false));

            fileManager.ReadContent("b.txt", out var content);
            Assert.Equal(new[] { "old" }, content);
        }
    }
}
