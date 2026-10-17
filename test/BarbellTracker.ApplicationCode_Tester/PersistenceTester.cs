using System;
using System.IO;
using System.Linq;
using BarbellTracker.AbstractionCode;
using BarbellTracker.ApplicationCode;
using BarbellTracker.DomainCode;
using Xunit;

namespace BarbellTracker.ApplicationCodeTests
{
    public class PersistenceTester : IDisposable
    {
        private readonly string folder = Path.Combine(Path.GetTempPath(), "BarbellTrackerPersistence-" + Guid.NewGuid());

        public void Dispose()
        {
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
        }

        private static Session CreateSession(string name, DateTime createdAt)
        {
            return new Session()
            {
                Name = name,
                Exercise = "Squat",
                WeightKg = 100.5,
                Notes = "felt heavy",
                CreatedAt = createdAt,
                RepetitionCount = 3,
                BestMeanVelocity = 0.82,
                VelocityLossPercent = 12.5,
                Tracked = new TrackedInformation()
                {
                    Id = "t-" + name,
                    Name = name,
                    FrameRate = 30,
                    PixelPerCm = 2.5,
                    Positions = new[] { new Vector2D(1, 2), new Vector2D(3.5, 4.25) },
                },
            };
        }

        // ---- sessions ----

        [Fact]
        public void Save_ThenTryGet_ReturnsTheSameData()
        {
            var repository = new JsonSessionRepository(folder);
            var session = CreateSession("a", new DateTime(2024, 5, 1, 10, 0, 0));

            repository.Save(session);

            Assert.True(repository.TryGet(session.Id, out var loaded));
            Assert.Equal(session.Name, loaded.Name);
            Assert.Equal("Squat", loaded.Exercise);
            Assert.Equal(100.5, loaded.WeightKg);
            Assert.Equal("felt heavy", loaded.Notes);
            Assert.Equal(session.CreatedAt, loaded.CreatedAt);
            Assert.Equal(3, loaded.RepetitionCount);
            Assert.Equal(0.82, loaded.BestMeanVelocity);
            Assert.Equal(30, loaded.Tracked.FrameRate);
            Assert.Equal(2.5, loaded.Tracked.PixelPerCm);
            Assert.Equal(2, loaded.Tracked.Positions.Length);
            Assert.Equal(new Vector2D(3.5, 4.25), loaded.Tracked.Positions[1]);
        }

        [Fact]
        public void Save_TwiceWithSameId_Overwrites()
        {
            var repository = new JsonSessionRepository(folder);
            var session = CreateSession("a", DateTime.Now);
            repository.Save(session);

            session.Notes = "changed";
            repository.Save(session);

            Assert.Single(repository.GetAll());
            repository.TryGet(session.Id, out var loaded);
            Assert.Equal("changed", loaded.Notes);
            Assert.Empty(Directory.GetFiles(folder, "*.tmp"));
        }

        [Fact]
        public void GetAll_NoFolderYet_IsEmpty()
        {
            Assert.Empty(new JsonSessionRepository(folder).GetAll());
        }

        [Fact]
        public void GetAll_ReturnsNewestFirst()
        {
            var repository = new JsonSessionRepository(folder);
            repository.Save(CreateSession("old", new DateTime(2024, 1, 1)));
            repository.Save(CreateSession("new", new DateTime(2024, 6, 1)));
            repository.Save(CreateSession("middle", new DateTime(2024, 3, 1)));

            Assert.Equal(new[] { "new", "middle", "old" }, repository.GetAll().Select(s => s.Name));
        }

        [Fact]
        public void GetAll_SkipsDamagedFiles()
        {
            var repository = new JsonSessionRepository(folder);
            repository.Save(CreateSession("good", DateTime.Now));
            File.WriteAllText(Path.Combine(folder, "broken.json"), "{ this is not json");

            Assert.Equal(new[] { "good" }, repository.GetAll().Select(s => s.Name));
        }

        [Fact]
        public void Delete_RemovesSession()
        {
            var repository = new JsonSessionRepository(folder);
            var session = CreateSession("a", DateTime.Now);
            repository.Save(session);

            Assert.True(repository.Delete(session.Id));

            Assert.False(repository.TryGet(session.Id, out _));
            Assert.False(repository.Delete(session.Id));
        }

        [Theory]
        [InlineData("")]
        [InlineData("..\\evil")]
        [InlineData("../evil")]
        [InlineData("a/b")]
        [InlineData("..")]
        public void InvalidIds_AreRejected(string id)
        {
            var repository = new JsonSessionRepository(folder);

            Assert.Throws<ArgumentException>(() => repository.TryGet(id, out _));
            Assert.Throws<ArgumentException>(() => repository.Delete(id));
            Assert.Throws<ArgumentException>(() => repository.Save(new Session() { Id = id }));
        }

        [Fact]
        public void NewSessions_GetDifferentIds()
        {
            Assert.NotEqual(new Session().Id, new Session().Id);
        }

        // ---- settings ----

        [Fact]
        public void Settings_MissingFile_GivesDefaults()
        {
            var settings = new JsonSettingsStore<UserSettings>(Path.Combine(folder, "settings.json")).Load();

            Assert.Equal(5, settings.SmoothingWindow);
            Assert.Equal(0.2, settings.MinRangeOfMotion);
            Assert.Equal(string.Empty, settings.OutputFolder);
        }

        [Fact]
        public void Settings_SaveThenLoad_RoundTrips()
        {
            var store = new JsonSettingsStore<UserSettings>(Path.Combine(folder, "sub", "settings.json"));

            Assert.True(store.Save(new UserSettings() { SmoothingWindow = 9, MinRangeOfMotion = 0.35, OutputFolder = @"C:\out" }));

            var loaded = store.Load();
            Assert.Equal(9, loaded.SmoothingWindow);
            Assert.Equal(0.35, loaded.MinRangeOfMotion);
            Assert.Equal(@"C:\out", loaded.OutputFolder);
        }

        [Fact]
        public void Settings_DamagedFile_GivesDefaults()
        {
            Directory.CreateDirectory(folder);
            var file = Path.Combine(folder, "settings.json");
            File.WriteAllText(file, "not json at all");

            var loaded = new JsonSettingsStore<UserSettings>(file).Load();

            Assert.Equal(5, loaded.SmoothingWindow);
        }
    }
}
