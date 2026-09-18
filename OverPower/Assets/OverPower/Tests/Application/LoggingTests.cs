using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using OverPower.Application;
using OverPower.Application.Ports.Logging;
using OverPower.Domain.Run;
using OverPower.Tests.Application;

namespace OverPower.Tests.Application
{
    [TestFixture]
    public class LoggingTests
    {
        [Test]
        public void NullGameLogger_EveryMethod_DoesNotThrow()
        {
            // Arrange
            IGameLogger nullLogger = NullGameLogger.Instance;

            // Act & Assert
            Assert.DoesNotThrow(() => nullLogger.Debug("TEST", "test message"));
            Assert.DoesNotThrow(() => nullLogger.Info("TEST", "test message"));
            Assert.DoesNotThrow(() => nullLogger.Warning("TEST", "test message"));
            Assert.DoesNotThrow(() => nullLogger.Error("TEST", "test message"));
        }

        [Test]
        public void TestGameLogger_RecordsCorrectLevelCategoryAndMessage()
        {
            // Arrange
            var testLogger = new TestGameLogger();

            // Act
            testLogger.Info(GameLogCategories.RunStart, "Started new run with seed 12345");
            testLogger.Error("COMBAT", "Critical database error");

            // Assert
            Assert.That(testLogger.Entries.Count, Is.EqualTo(2));

            var entry1 = testLogger.Entries[0];
            Assert.That(entry1.Level, Is.EqualTo(TestGameLogger.LogLevel.Info));
            Assert.That(entry1.Category, Is.EqualTo(GameLogCategories.RunStart));
            Assert.That(entry1.Message, Is.EqualTo("Started new run with seed 12345"));

            var entry2 = testLogger.Entries[1];
            Assert.That(entry2.Level, Is.EqualTo(TestGameLogger.LogLevel.Error));
            Assert.That(entry2.Category, Is.EqualTo("COMBAT"));
            Assert.That(entry2.Message, Is.EqualTo("Critical database error"));

            // Clear behaves correctly
            testLogger.Clear();
            Assert.That(testLogger.Entries.Count, Is.EqualTo(0));
        }

        [Test]
        public void VerbosityFilteredLogger_WithErrorsOnly_FiltersCorrectly()
        {
            // Arrange
            var testLogger = new TestGameLogger();
            var filteredLogger = new VerbosityFilteredLogger(testLogger, GameLogVerbosity.ErrorsOnly);

            // Act
            filteredLogger.Debug("CAT", "debug");
            filteredLogger.Info("CAT", "info");
            filteredLogger.Warning("CAT", "warning");
            filteredLogger.Error("CAT", "error");

            // Assert
            Assert.That(testLogger.Entries.Count, Is.EqualTo(1));
            Assert.That(testLogger.Entries[0].Level, Is.EqualTo(TestGameLogger.LogLevel.Error));
            Assert.That(testLogger.Entries[0].Message, Is.EqualTo("error"));
        }

        [Test]
        public void VerbosityFilteredLogger_WithNormal_FiltersCorrectly()
        {
            // Arrange
            var testLogger = new TestGameLogger();
            var filteredLogger = new VerbosityFilteredLogger(testLogger, GameLogVerbosity.Normal);

            // Act
            filteredLogger.Debug("CAT", "debug");
            filteredLogger.Info("CAT", "info");
            filteredLogger.Warning("CAT", "warning");
            filteredLogger.Error("CAT", "error");

            // Assert
            Assert.That(testLogger.Entries.Count, Is.EqualTo(3));
            Assert.That(testLogger.Entries[0].Level, Is.EqualTo(TestGameLogger.LogLevel.Info));
            Assert.That(testLogger.Entries[0].Message, Is.EqualTo("info"));
            Assert.That(testLogger.Entries[1].Level, Is.EqualTo(TestGameLogger.LogLevel.Warning));
            Assert.That(testLogger.Entries[1].Message, Is.EqualTo("warning"));
            Assert.That(testLogger.Entries[2].Level, Is.EqualTo(TestGameLogger.LogLevel.Error));
            Assert.That(testLogger.Entries[2].Message, Is.EqualTo("error"));
        }

        [Test]
        public void VerbosityFilteredLogger_WithVerbose_FiltersCorrectly()
        {
            // Arrange
            var testLogger = new TestGameLogger();
            var filteredLogger = new VerbosityFilteredLogger(testLogger, GameLogVerbosity.Verbose);

            // Act
            filteredLogger.Debug("CAT", "debug");
            filteredLogger.Info("CAT", "info");
            filteredLogger.Warning("CAT", "warning");
            filteredLogger.Error("CAT", "error");

            // Assert
            Assert.That(testLogger.Entries.Count, Is.EqualTo(4));
            Assert.That(testLogger.Entries[0].Level, Is.EqualTo(TestGameLogger.LogLevel.Debug));
            Assert.That(testLogger.Entries[0].Message, Is.EqualTo("debug"));
            Assert.That(testLogger.Entries[1].Level, Is.EqualTo(TestGameLogger.LogLevel.Info));
            Assert.That(testLogger.Entries[1].Message, Is.EqualTo("info"));
            Assert.That(testLogger.Entries[2].Level, Is.EqualTo(TestGameLogger.LogLevel.Warning));
            Assert.That(testLogger.Entries[2].Message, Is.EqualTo("warning"));
            Assert.That(testLogger.Entries[3].Level, Is.EqualTo(TestGameLogger.LogLevel.Error));
            Assert.That(testLogger.Entries[3].Message, Is.EqualTo("error"));
        }

        [Test]
        public async Task RunInitialization_WithTestLogger_LogsCorrectRunStartSeed()
        {
            // Arrange
            var testLogger = new TestGameLogger();
            var navigator = new FakeSceneNavigator();
            var controller = new GameFlowController(navigator, testLogger);
            var seed = new RunSeed(123456789UL);

            // Act
            await controller.StartNewRunAsync(seed);

            // Assert
            Assert.That(testLogger.Entries.Count, Is.EqualTo(1));
            var log = testLogger.Entries[0];
            Assert.That(log.Level, Is.EqualTo(TestGameLogger.LogLevel.Info));
            Assert.That(log.Category, Is.EqualTo(GameLogCategories.RunStart));
            Assert.That(log.Message, Contains.Substring("123456789"));
        }

        [Test]
        public async Task RunInitialization_WithNullLogger_DoesNotThrow()
        {
            // Arrange
            var navigator = new FakeSceneNavigator();
            var controller = new GameFlowController(navigator, NullGameLogger.Instance);
            var seed = new RunSeed(123456789UL);

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => await controller.StartNewRunAsync(seed));
        }

        private class FakeSceneNavigator : ISceneNavigator
        {
            public GameSceneId CurrentScene => GameSceneId.Bootstrap;
            public Task LoadSceneAsync(GameSceneId sceneId, CancellationToken cancellationToken = default) => Task.CompletedTask;
        }
    }
}
