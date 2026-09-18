using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using OverPower.Domain.Content;
using OverPower.Domain.Content.Abilities;
using OverPower.Domain.Content.Resources;
using OverPower.Domain.Content.Units;
using OverPower.Domain.Content.Spells;
using OverPower.Domain.Content.Artifacts;
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
        public void UnitDefinition_ValidConstruction_SucceedsAndPreservesStableIdsMetadataAndStats()
        {
            // Arrange
            var id = ContentId.Create("unit.guardian").Value;
            var nameKey = new LocalizationKey("unit.guardian.name");
            var descKey = new LocalizationKey("unit.guardian.desc");
            var ability1 = ContentId.Create("ability.guardian_shield").Value;
            var ability2 = ContentId.Create("ability.taunt").Value;
            var abilities = new List<ContentId> { ability1, ability2 };

            // Act
            var unit = new UnitDefinition(id, nameKey, descKey, 15, 2, 5, abilities);

            // Assert
            Assert.That(unit.Id, Is.SameAs(id));
            Assert.That(unit.Id.Value, Is.EqualTo("unit.guardian"));
            Assert.That(unit.NameKey, Is.EqualTo(nameKey));
            Assert.That(unit.DescriptionKey, Is.EqualTo(descKey));
            Assert.That(unit.MaxHpPerMember, Is.EqualTo(15));
            Assert.That(unit.Armor, Is.EqualTo(2));
            Assert.That(unit.Attack, Is.EqualTo(5));
            Assert.That(unit.AbilityIds.Count, Is.EqualTo(2));
            Assert.That(unit.AbilityIds[0], Is.SameAs(ability1));
            Assert.That(unit.AbilityIds[1], Is.SameAs(ability2));
        }

        [Test]
        public void UnitDefinition_NullId_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UnitDefinition(
                null!, 
                new LocalizationKey("n"), 
                new LocalizationKey("d"), 
                10, 0, 1, 
                Enumerable.Empty<ContentId>()));
        }

        [Test]
        public void UnitDefinition_InvalidPrefix_ThrowsArgumentException()
        {
            // Arrange
            var id = ContentId.Create("ability.fire").Value;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new UnitDefinition(
                id, 
                new LocalizationKey("n"), 
                new LocalizationKey("d"), 
                10, 0, 1, 
                Enumerable.Empty<ContentId>()));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void UnitDefinition_InvalidHp_ThrowsArgumentOutOfRangeException(int hp)
        {
            // Arrange
            var id = ContentId.Create("unit.guardian").Value;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new UnitDefinition(
                id, 
                new LocalizationKey("n"), 
                new LocalizationKey("d"), 
                hp, 0, 1, 
                Enumerable.Empty<ContentId>()));
        }

        [Test]
        public void UnitDefinition_NegativeArmor_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var id = ContentId.Create("unit.guardian").Value;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new UnitDefinition(
                id, 
                new LocalizationKey("n"), 
                new LocalizationKey("d"), 
                10, -1, 1, 
                Enumerable.Empty<ContentId>()));
        }

        [Test]
        public void UnitDefinition_NegativeAttack_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var id = ContentId.Create("unit.guardian").Value;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new UnitDefinition(
                id, 
                new LocalizationKey("n"), 
                new LocalizationKey("d"), 
                10, 0, -5, 
                Enumerable.Empty<ContentId>()));
        }

        [Test]
        public void UnitDefinition_NullAbilitiesCollection_ThrowsArgumentNullException()
        {
            // Arrange
            var id = ContentId.Create("unit.guardian").Value;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UnitDefinition(
                id, 
                new LocalizationKey("n"), 
                new LocalizationKey("d"), 
                10, 0, 1, 
                null!));
        }

        [Test]
        public void UnitDefinition_CollectionWithNullReferences_ThrowsArgumentException()
        {
            // Arrange
            var id = ContentId.Create("unit.guardian").Value;
            var listWithNull = new List<ContentId?> { ContentId.Create("ability.taunt").Value, null };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new UnitDefinition(
                id, 
                new LocalizationKey("n"), 
                new LocalizationKey("d"), 
                10, 0, 1, 
                listWithNull!));
        }

        [Test]
        public void UnitDefinition_CollectionImmutability_ProtectsAgainstExternalMutation()
        {
            // Arrange
            var id = ContentId.Create("unit.guardian").Value;
            var mutableList = new List<ContentId> { ContentId.Create("ability.taunt").Value };
            var unit = new UnitDefinition(id, new LocalizationKey("n"), new LocalizationKey("d"), 10, 0, 1, mutableList);

            // Act
            mutableList.Add(ContentId.Create("ability.shield").Value);

            // Assert
            Assert.That(unit.AbilityIds.Count, Is.EqualTo(1));
            Assert.That(unit.AbilityIds[0].Value, Is.EqualTo("ability.taunt"));
        }

        [Test]
        public void SpellDefinition_ValidConstruction_SucceedsAndPreservesStableIdsAndStats()
        {
            // Arrange
            var id = ContentId.Create("spell.fireball").Value;
            var nameKey = new LocalizationKey("spell.fireball.name");
            var descKey = new LocalizationKey("spell.fireball.desc");

            // Act
            var spell = new SpellDefinition(id, nameKey, descKey, 3);

            // Assert
            Assert.That(spell.Id, Is.SameAs(id));
            Assert.That(spell.Id.Value, Is.EqualTo("spell.fireball"));
            Assert.That(spell.NameKey, Is.EqualTo(nameKey));
            Assert.That(spell.DescriptionKey, Is.EqualTo(descKey));
            Assert.That(spell.ManaCost, Is.EqualTo(3));
        }

        [Test]
        public void SpellDefinition_NullId_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SpellDefinition(
                null!, 
                new LocalizationKey("n"), 
                new LocalizationKey("d"), 
                2));
        }

        [Test]
        public void SpellDefinition_InvalidPrefix_ThrowsArgumentException()
        {
            // Arrange
            var id = ContentId.Create("unit.guardian").Value;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new SpellDefinition(
                id, 
                new LocalizationKey("n"), 
                new LocalizationKey("d"), 
                2));
        }

        [Test]
        public void SpellDefinition_NegativeManaCost_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var id = ContentId.Create("spell.fireball").Value;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new SpellDefinition(
                id, 
                new LocalizationKey("n"), 
                new LocalizationKey("d"), 
                -1));
        }

        [Test]
        public void ArtifactDefinition_ValidConstruction_SucceedsAndPreservesStableIdsAndMetadata()
        {
            // Arrange
            var id = ContentId.Create("artifact.ancient_ring").Value;
            var nameKey = new LocalizationKey("artifact.ancient_ring.name");
            var descKey = new LocalizationKey("artifact.ancient_ring.desc");

            // Act
            var artifact = new ArtifactDefinition(id, nameKey, descKey);

            // Assert
            Assert.That(artifact.Id, Is.SameAs(id));
            Assert.That(artifact.Id.Value, Is.EqualTo("artifact.ancient_ring"));
            Assert.That(artifact.NameKey, Is.EqualTo(nameKey));
            Assert.That(artifact.DescriptionKey, Is.EqualTo(descKey));
        }

        [Test]
        public void ArtifactDefinition_NullId_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ArtifactDefinition(
                null!, 
                new LocalizationKey("n"), 
                new LocalizationKey("d")));
        }

        [Test]
        public void ArtifactDefinition_InvalidPrefix_ThrowsArgumentException()
        {
            // Arrange
            var id = ContentId.Create("spell.fireball").Value;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new ArtifactDefinition(
                id, 
                new LocalizationKey("n"), 
                new LocalizationKey("d")));
        }

        [Test]
        public void DuplicateRegistration_RejectsAllDefinitionCategories()
        {
            // Arrange
            var registry = new ContentIdRegistry();
            var unitId = ContentId.Create("unit.warrior").Value;
            var spellId = ContentId.Create("spell.heal").Value;
            var artifactId = ContentId.Create("artifact.crown").Value;

            // Act
            var resUnit1 = registry.Register(unitId);
            var resUnit2 = registry.Register(unitId);

            var resSpell1 = registry.Register(spellId);
            var resSpell2 = registry.Register(spellId);

            var resArtifact1 = registry.Register(artifactId);
            var resArtifact2 = registry.Register(artifactId);

            // Assert
            Assert.That(resUnit1.IsSuccess, Is.True);
            Assert.That(resUnit2.IsFailure, Is.True);

            Assert.That(resSpell1.IsSuccess, Is.True);
            Assert.That(resSpell2.IsFailure, Is.True);

            Assert.That(resArtifact1.IsSuccess, Is.True);
            Assert.That(resArtifact2.IsFailure, Is.True);
        }
    }
}
