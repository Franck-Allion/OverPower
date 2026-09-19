using System;
using NUnit.Framework;
using OverPower.Domain.Cards;
using OverPower.Domain.Content;

namespace OverPower.Tests.Domain
{
    [TestFixture]
    public class RuntimeCardTests
    {
        [TestCase(1UL, CardType.Unit, "unit.guardian")]
        [TestCase(2UL, CardType.Spell, "spell.fireball")]
        public void Constructor_WithCompatibleContent_PreservesProperties(ulong value, CardType type, string content)
        {
            var instanceId = new CardInstanceId(value);
            ContentId contentId = ContentId.Create(content).Value;

            var card = new RuntimeCard(instanceId, type, contentId);

            Assert.That(card.InstanceId, Is.EqualTo(instanceId));
            Assert.That(card.Type, Is.EqualTo(type));
            Assert.That(card.ContentId, Is.SameAs(contentId));
        }

        [TestCase(CardType.Unit, "spell.fireball")]
        [TestCase(CardType.Spell, "unit.guardian")]
        [TestCase(CardType.Unit, "artifact.ring")]
        [TestCase(CardType.Spell, "ability.fire")]
        [TestCase(CardType.Unit, "units.guardian")]
        [TestCase(CardType.Spell, "spells.fireball")]
        public void Constructor_WithIncompatibleContent_Throws(CardType type, string content)
        {
            Assert.Throws<ArgumentException>(() =>
                new RuntimeCard(new CardInstanceId(1), type, ContentId.Create(content).Value));
        }

        [Test]
        public void Constructor_WithNullContent_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new RuntimeCard(new CardInstanceId(1), CardType.Unit, null!));
        }

        [Test]
        public void Constructor_WithDefaultInstanceId_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new RuntimeCard(default, CardType.Unit, ContentId.Create("unit.guardian").Value));
        }

        [TestCase(-1)]
        [TestCase(2)]
        public void Constructor_WithUndefinedType_Throws(int type)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new RuntimeCard(new CardInstanceId(1), (CardType)type, ContentId.Create("unit.guardian").Value));
        }

        [Test]
        public void Constructor_WithSameSpellContent_PreservesDistinctPhysicalIdentities()
        {
            var first = new RuntimeCard(new CardInstanceId(100), CardType.Spell, ContentId.Create("spell.fireball").Value);
            var second = new RuntimeCard(new CardInstanceId(101), CardType.Spell, ContentId.Create("spell.fireball").Value);

            Assert.That(first.ContentId, Is.EqualTo(second.ContentId));
            Assert.That(first.InstanceId, Is.Not.EqualTo(second.InstanceId));
            Assert.That(first, Is.Not.SameAs(second));
            Assert.That(first.Equals(second), Is.False);
        }
    }
}
