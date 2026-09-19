using System;
using System.Collections.Generic;
using NUnit.Framework;
using OverPower.Domain.Save;

namespace OverPower.Tests.Domain
{
    [TestFixture]
    public class SaveGameDtoTests
    {
        #region SaveSchema Tests

        [Test]
        public void SaveSchema_CurrentVersion_IsOne()
        {
            Assert.That(SaveSchema.CurrentVersion, Is.EqualTo(1));
        }

        [Test]
        public void SaveSchema_IsSupported_WithVersionOne_ReturnsTrue()
        {
            Assert.That(SaveSchema.IsSupported(1), Is.True);
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(2)] // Future version
        public void SaveSchema_IsSupported_WithUnsupportedVersions_ReturnsFalse(int version)
        {
            Assert.That(SaveSchema.IsSupported(version), Is.False);
        }

        #endregion

        #region SettingsSaveDto Tests

        [TestCase("en")]
        [TestCase("fr")]
        public void Settings_WithSupportedLocales_SucceedsAndPreservesValue(string locale)
        {
            var settings = new SettingsSaveDto(locale);
            Assert.That(settings.Locale, Is.EqualTo(locale));
        }

        [Test]
        public void Settings_WithNullLocale_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SettingsSaveDto(null!));
        }

        [TestCase("")]
        [TestCase("   ")]
        public void Settings_WithEmptyOrWhitespaceLocale_ThrowsArgumentException(string locale)
        {
            Assert.Throws<ArgumentException>(() => new SettingsSaveDto(locale));
        }

        [TestCase("de")]
        [TestCase("es")]
        [TestCase("EN")] // Case sensitive check
        public void Settings_WithUnsupportedLocale_ThrowsArgumentException(string locale)
        {
            Assert.Throws<ArgumentException>(() => new SettingsSaveDto(locale));
        }

        #endregion

        #region StatisticsSaveDto Tests

        [Test]
        public void Statistics_WithValidCounters_SucceedsAndPreservesValues()
        {
            var stats = new StatisticsSaveDto(
                runsStarted: 10,
                runsCompleted: 8,
                victories: 4,
                heroDeaths: 3
            );

            Assert.That(stats.RunsStarted, Is.EqualTo(10));
            Assert.That(stats.RunsCompleted, Is.EqualTo(8));
            Assert.That(stats.Victories, Is.EqualTo(4));
            Assert.That(stats.HeroDeaths, Is.EqualTo(3));
        }

        [Test]
        public void Statistics_WithZeroCounters_IsSupportedAndValid()
        {
            var stats = new StatisticsSaveDto(0, 0, 0, 0);

            Assert.That(stats.RunsStarted, Is.Zero);
            Assert.That(stats.RunsCompleted, Is.Zero);
            Assert.That(stats.Victories, Is.Zero);
            Assert.That(stats.HeroDeaths, Is.Zero);
        }

        [TestCase(-1, 0, 0, 0)]
        [TestCase(0, -1, 0, 0)]
        [TestCase(0, 0, -1, 0)]
        [TestCase(0, 0, 0, -1)]
        public void Statistics_WithNegativeCounters_ThrowsArgumentOutOfRangeException(int started, int completed, int victories, int deaths)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new StatisticsSaveDto(started, completed, victories, deaths));
        }

        [Test]
        public void Statistics_CompletedExceedsStarted_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new StatisticsSaveDto(5, 6, 2, 2));
        }

        [Test]
        public void Statistics_SumOfOutcomesExceedsCompleted_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new StatisticsSaveDto(10, 5, 4, 2)); // Victories (4) + deaths (2) = 6 > completed (5)
        }

        #endregion

        #region SaveGameDto Tests

        private static SettingsSaveDto CreateValidSettings() => new SettingsSaveDto("en");
        private static StatisticsSaveDto CreateValidStats() => new StatisticsSaveDto(10, 8, 4, 3);

        [Test]
        public void SaveGameDto_WithValidArguments_SucceedsAndPreservesValues()
        {
            var settings = CreateValidSettings();
            var stats = CreateValidStats();
            var unlocks = new[] { "artifact.ruby", "spell.fireball" };

            var save = new SaveGameDto(
                schemaVersion: 1,
                settings: settings,
                metaCurrency: 150,
                unlockIds: unlocks,
                statistics: stats
            );

            Assert.That(save.SchemaVersion, Is.EqualTo(1));
            Assert.That(save.Settings, Is.SameAs(settings));
            Assert.That(save.MetaCurrency, Is.EqualTo(150));
            Assert.That(save.UnlockIds, Is.EquivalentTo(unlocks));
            Assert.That(save.Statistics, Is.SameAs(stats));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void SaveGameDto_WithInvalidSchemaVersion_ThrowsArgumentOutOfRangeException(int version)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SaveGameDto(version, CreateValidSettings(), 100, Array.Empty<string>(), CreateValidStats()));
        }

        [Test]
        public void SaveGameDto_WithUnsupportedNewSchemaVersion_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new SaveGameDto(SaveSchema.CurrentVersion + 1, CreateValidSettings(), 100, Array.Empty<string>(), CreateValidStats()));
        }

        [Test]
        public void SaveGameDto_WithNullSettings_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SaveGameDto(1, null!, 100, Array.Empty<string>(), CreateValidStats()));
        }

        [Test]
        public void SaveGameDto_WithNegativeMetaCurrency_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SaveGameDto(1, CreateValidSettings(), -1, Array.Empty<string>(), CreateValidStats()));
        }

        [Test]
        public void SaveGameDto_WithNullUnlockIds_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SaveGameDto(1, CreateValidSettings(), 100, null!, CreateValidStats()));
        }

        [Test]
        public void SaveGameDto_WithNullStatistics_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SaveGameDto(1, CreateValidSettings(), 100, Array.Empty<string>(), null!));
        }

        [Test]
        public void SaveGameDto_WithEmptyUnlocks_IsSufficientAndValid()
        {
            var save = new SaveGameDto(1, CreateValidSettings(), 100, Array.Empty<string>(), CreateValidStats());
            Assert.That(save.UnlockIds, Is.Empty);
        }

        [Test]
        public void SaveGameDto_WithDuplicateUnlockIds_ThrowsArgumentException()
        {
            var duplicateUnlocks = new[] { "artifact.ruby", "artifact.ruby" };

            Assert.Throws<ArgumentException>(() =>
                new SaveGameDto(1, CreateValidSettings(), 100, duplicateUnlocks, CreateValidStats()));
        }

        [Test]
        public void SaveGameDto_WithNullElementInUnlockIds_ThrowsArgumentException()
        {
            var badUnlocks = new[] { "artifact.ruby", null! };

            Assert.Throws<ArgumentException>(() =>
                new SaveGameDto(1, CreateValidSettings(), 100, badUnlocks, CreateValidStats()));
        }

        [TestCase("")]
        [TestCase("   ")]
        public void SaveGameDto_WithEmptyOrWhitespaceElementInUnlockIds_ThrowsArgumentException(string id)
        {
            var badUnlocks = new[] { "artifact.ruby", id };

            Assert.Throws<ArgumentException>(() =>
                new SaveGameDto(1, CreateValidSettings(), 100, badUnlocks, CreateValidStats()));
        }

        [TestCase("ruby")] // Missing prefix (less than 2 segments)
        [TestCase("artifact.ruby-item")] // Contains invalid hyphen
        [TestCase("artifact.Ruby")] // Contains invalid uppercase letters
        public void SaveGameDto_WithInvalidContentIdInUnlockIds_ThrowsArgumentException(string id)
        {
            var badUnlocks = new[] { id };

            Assert.Throws<ArgumentException>(() =>
                new SaveGameDto(1, CreateValidSettings(), 100, badUnlocks, CreateValidStats()));
        }

        [Test]
        public void SaveGameDto_UnlockIds_IsDefensivelyCopiedAndReadOnly()
        {
            var inputList = new List<string> { "artifact.ruby", "spell.fireball" };
            var save = new SaveGameDto(1, CreateValidSettings(), 100, inputList, CreateValidStats());

            // Mutate the original input list
            inputList.Add("spell.heal");

            // Verify the DTO was not mutated (copied defensively)
            Assert.That(save.UnlockIds.Count, Is.EqualTo(2));
            Assert.That(save.UnlockIds, Is.Not.Contains("spell.heal"));

            // Verify that the exposed list is read-only
            Assert.Throws<NotSupportedException>(() => ((IList<string>)save.UnlockIds).Add("spell.heal"));
        }

        #endregion
    }
}
