using NUnit.Framework;
using System;
using OverPower.Domain.Content;
using OverPower.Domain.Content.Abilities;
using OverPower.Domain.Content.Resources;
using OverPower.Domain.Localization;

namespace OverPower.Tests.Domain
{
    [TestFixture]
    public class ContentDefinitionTests
    {
        [Test]
        public void LocalizationKey_ValidConstruction_SucceedsAndStoresValue()
        {
            // Act
            var key = new LocalizationKey("ability.guardian_shield.name");

            // Assert
            Assert.That(key.Value, Is.EqualTo("ability.guardian_shield.name"));
        }

        [TestCase(null)]
        public void LocalizationKey_NullValue_ThrowsArgumentNullException(string? value)
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new LocalizationKey(value!));
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("   \n")]
        public void LocalizationKey_EmptyOrWhitespaceValue_ThrowsArgumentException(string value)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new LocalizationKey(value));
        }

        [Test]
        public void LocalizationKey_EqualityAndHashcode_BehavesDeterministically()
        {
            // Arrange
            var key1 = new LocalizationKey("key.a");
            var key2 = new LocalizationKey("key.a");
            var key3 = new LocalizationKey("key.b");

            // Assert
            Assert.That(key1, Is.EqualTo(key2));
            Assert.That(key1, Is.Not.EqualTo(key3));
            Assert.That(key1.GetHashCode(), Is.EqualTo(key2.GetHashCode()));
            Assert.That(key1.GetHashCode(), Is.Not.EqualTo(key3.GetHashCode()));
            Assert.That(key1 == key2, Is.True);
            Assert.That(key1 != key3, Is.True);
        }

        [Test]
        public void AbilityDefinition_ValidConstruction_SucceedsAndPreservesStableIdsAndMetadata()
        {
            // Arrange
            var id = ContentId.Create("ability.shield").Value;
            var nameKey = new LocalizationKey("ability.shield.name");
            var descKey = new LocalizationKey("ability.shield.desc");

            // Act
            var ability = new AbilityDefinition(id, nameKey, descKey);

            // Assert
            Assert.That(ability.Id, Is.SameAs(id));
            Assert.That(ability.Id.Value, Is.EqualTo("ability.shield"));
            Assert.That(ability.NameKey, Is.EqualTo(nameKey));
            Assert.That(ability.DescriptionKey, Is.EqualTo(descKey));
        }

        [Test]
        public void AbilityDefinition_NullId_ThrowsArgumentNullException()
        {
            // Arrange
            var nameKey = new LocalizationKey("ability.shield.name");
            var descKey = new LocalizationKey("ability.shield.desc");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AbilityDefinition(null!, nameKey, descKey));
        }

        [Test]
        public void AbilityDefinition_InvalidPrefix_ThrowsArgumentException()
        {
            // Arrange
            var id = ContentId.Create("resource.gold").Value;
            var nameKey = new LocalizationKey("ability.shield.name");
            var descKey = new LocalizationKey("ability.shield.desc");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new AbilityDefinition(id, nameKey, descKey));
        }

        [Test]
        public void ResourceType_ValidConstruction_SucceedsAndPreservesStableIdsAndMetadata()
        {
            // Arrange
            var id = ContentId.Create("resource.gold").Value;
            var nameKey = new LocalizationKey("resource.gold.name");
            var descKey = new LocalizationKey("resource.gold.desc");

            // Act
            var resource = new ResourceType(id, nameKey, descKey);

            // Assert
            Assert.That(resource.Id, Is.SameAs(id));
            Assert.That(resource.Id.Value, Is.EqualTo("resource.gold"));
            Assert.That(resource.NameKey, Is.EqualTo(nameKey));
            Assert.That(resource.DescriptionKey, Is.EqualTo(descKey));
        }

        [Test]
        public void ResourceType_NullId_ThrowsArgumentNullException()
        {
            // Arrange
            var nameKey = new LocalizationKey("resource.gold.name");
            var descKey = new LocalizationKey("resource.gold.desc");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ResourceType(null!, nameKey, descKey));
        }

        [Test]
        public void ResourceType_InvalidPrefix_ThrowsArgumentException()
        {
            // Arrange
            var id = ContentId.Create("ability.shield").Value;
            var nameKey = new LocalizationKey("resource.gold.name");
            var descKey = new LocalizationKey("resource.gold.desc");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new ResourceType(id, nameKey, descKey));
        }

        [Test]
        public void DuplicateRegistration_IsRejectedByRegistry()
        {
            // Arrange
            var registry = new ContentIdRegistry();
            var id1 = ContentId.Create("ability.fire").Value;
            var id2 = ContentId.Create("ability.fire").Value;

            var ability1 = new AbilityDefinition(id1, new LocalizationKey("name"), new LocalizationKey("desc"));
            var ability2 = new AbilityDefinition(id2, new LocalizationKey("name"), new LocalizationKey("desc"));

            // Act
            var result1 = registry.Register(ability1.Id);
            var result2 = registry.Register(ability2.Id);

            // Assert
            Assert.That(result1.IsSuccess, Is.True);
            Assert.That(result2.IsFailure, Is.True);
            Assert.That(result2.Error.Code, Is.EqualTo("content_id.duplicate"));
        }
    }
}
