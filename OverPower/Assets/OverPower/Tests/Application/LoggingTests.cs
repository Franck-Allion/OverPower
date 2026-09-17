using NUnit.Framework;
using OverPower.Application.Ports.Logging;
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
    }
}
