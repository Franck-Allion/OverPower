using NUnit.Framework;
using System;
using OverPower.Domain.Content;
using OverPower.Domain.Primitives;

namespace OverPower.Tests.Domain
{
    [TestFixture]
    public class ContentIdTests
    {
        [TestCase("unit.guardian")]
        [TestCase("spell.fireball")]
        [TestCase("artifact.divine_shield")]
        [TestCase("ability.guardian.taunt")]
        [TestCase("resource.gold")]
        public void Create_WithValidId_SucceedsAndStoresValue(string idValue)
        {
            // Act
            Result<ContentId> result = ContentId.Create(idValue);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.Value, Is.EqualTo(idValue));
            Assert.That(result.Value.ToString(), Is.EqualTo(idValue));
        }

        [TestCase(null, "content_id.empty")]
        [TestCase("", "content_id.empty")]
        [TestCase(" ", "content_id.empty")]
        [TestCase("Guardian", "content_id.invalid_format")]
        [TestCase("unit Guardian", "content_id.invalid_format")]
        [TestCase(".unit.guardian", "content_id.invalid_format")]
        [TestCase("unit.guardian.", "content_id.invalid_format")]
        [TestCase("unit..guardian", "content_id.invalid_format")]
        [TestCase("unit/guardian", "content_id.invalid_format")]
        [TestCase("unit.guardian!", "content_id.invalid_format")]
        public void Create_WithInvalidId_FailsWithExpectedErrorCode(string? invalidValue, string expectedErrorCode)
        {
            // Act
            Result<ContentId> result = ContentId.Create(invalidValue!);

            // Assert
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo(expectedErrorCode));
            Assert.That(result.Error.Message, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void Equals_WithIdenticalValues_ReturnsTrue()
        {
            // Arrange
            ContentId id1 = ContentId.Create("unit.guardian").Value;
            ContentId id2 = ContentId.Create("unit.guardian").Value;

            // Act & Assert
            Assert.That(id1.Equals(id2), Is.True);
            Assert.That(id1 == id2, Is.True);
            Assert.That(id1 != id2, Is.False);
            Assert.That(id1.GetHashCode(), Is.EqualTo(id2.GetHashCode()));
        }

        [Test]
        public void Equals_WithDifferentValues_ReturnsFalse()
        {
            // Arrange
            ContentId id1 = ContentId.Create("unit.guardian").Value;
            ContentId id2 = ContentId.Create("spell.fireball").Value;

            // Act & Assert
            Assert.That(id1.Equals(id2), Is.False);
            Assert.That(id1 == id2, Is.False);
            Assert.That(id1 != id2, Is.True);
            Assert.That(id1.GetHashCode(), Is.Not.EqualTo(id2.GetHashCode()));
        }

        [Test]
        public void CompareTo_WithIdenticalValues_ReturnsZero()
        {
            // Arrange
            ContentId id1 = ContentId.Create("unit.guardian").Value;
            ContentId id2 = ContentId.Create("unit.guardian").Value;

            // Act & Assert
            Assert.That(id1.CompareTo(id2), Is.Zero);
        }

        [Test]
        public void CompareTo_WithDifferentValues_IsConsistentWithAlphabeticalOrder()
        {
            // Arrange
            ContentId id1 = ContentId.Create("spell.fireball").Value;
            ContentId id2 = ContentId.Create("unit.guardian").Value;

            // Act & Assert
            Assert.That(id1.CompareTo(id2), Is.LessThan(0));
            Assert.That(id2.CompareTo(id1), Is.GreaterThan(0));
        }

        [Test]
        public void Registry_RegisterAndDetectDuplicates_BehavesDeterministically()
        {
            // Arrange
            var registry = new ContentIdRegistry();
            ContentId guardian = ContentId.Create("unit.guardian").Value;
            ContentId fireball = ContentId.Create("spell.fireball").Value;

            // Act - Register valid first time
            Result reg1 = registry.Register(guardian);
            Result reg2 = registry.Register(fireball);

            // Assert - Succeeded
            Assert.That(reg1.IsSuccess, Is.True);
            Assert.That(reg2.IsSuccess, Is.True);
            Assert.That(registry.IsRegistered(guardian), Is.True);
            Assert.That(registry.IsRegistered(fireball), Is.True);
            Assert.That(registry.RegisteredIds.Count, Is.EqualTo(2));

            // Act - Register duplicate
            Result duplicateReg = registry.Register(guardian);

            // Assert - Fails cleanly
            Assert.That(duplicateReg.IsFailure, Is.True);
            Assert.That(duplicateReg.Error.Code, Is.EqualTo("content_id.duplicate"));
            Assert.That(registry.RegisteredIds.Count, Is.EqualTo(2), "Registry count must not change on failed duplicate registration.");

            // Register third different ID still succeeds
            ContentId gold = ContentId.Create("resource.gold").Value;
            Result reg3 = registry.Register(gold);
            Assert.That(reg3.IsSuccess, Is.True);
            Assert.That(registry.RegisteredIds.Count, Is.EqualTo(3));
        }
    }
}
