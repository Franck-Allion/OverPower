using System;
using System.Collections.Generic;
using NUnit.Framework;
using OverPower.Domain.Cards;
using OverPower.Domain.Content;

namespace OverPower.Tests.Domain
{
    [TestFixture]
    public class DeckTests
    {
        [Test]
        public void Constructor_WithValidMixedDeck_PreservesCardReferencesAndOrder()
        {
            var cards = new[]
            {
                CreateCard(1, CardType.Unit, "unit.guardian"),
                CreateCard(2, CardType.Unit, "unit.archer"),
                CreateCard(10, CardType.Spell, "spell.fireball"),
                CreateCard(11, CardType.Spell, "spell.fireball"),
                CreateCard(12, CardType.Spell, "spell.heal")
            };

            var deck = new Deck(cards);

            Assert.That(deck.Count, Is.EqualTo(5));
            for (int i = 0; i < cards.Length; i++)
            {
                Assert.That(deck.Cards[i], Is.SameAs(cards[i]));
            }
        }

        [Test]
        public void Constructor_WithSecondUnitCardForSameUnitType_Throws()
        {
            // One unit card represents a unit type, not an individual recruited unit.
            var cards = new[]
            {
                CreateCard(1, CardType.Unit, "unit.guardian"),
                CreateCard(2, CardType.Unit, "unit.guardian")
            };

            Assert.Throws<ArgumentException>(() => new Deck(cards));
        }

        [Test]
        public void Constructor_WithDifferentUnitTypes_AcceptsBothCards()
        {
            var deck = new Deck(new[]
            {
                CreateCard(1, CardType.Unit, "unit.guardian"),
                CreateCard(2, CardType.Unit, "unit.archer")
            });

            Assert.That(deck.Count, Is.EqualTo(2));
        }

        [Test]
        public void Constructor_WithSameSpellContent_KeepsIndependentPhysicalCards()
        {
            var first = CreateCard(10, CardType.Spell, "spell.fireball");
            var second = CreateCard(11, CardType.Spell, "spell.fireball");

            var deck = new Deck(new[] { first, second });

            Assert.That(deck.Count, Is.EqualTo(2));
            Assert.That(deck.Cards[0], Is.SameAs(first));
            Assert.That(deck.Cards[1], Is.SameAs(second));
            Assert.That(deck.Cards[0].ContentId, Is.EqualTo(deck.Cards[1].ContentId));
            Assert.That(deck.Cards[0].InstanceId, Is.Not.EqualTo(deck.Cards[1].InstanceId));
        }

        [TestCase(CardType.Spell, "spell.fireball", CardType.Spell, "spell.heal")]
        [TestCase(CardType.Spell, "spell.fireball", CardType.Spell, "spell.fireball")]
        [TestCase(CardType.Unit, "unit.guardian", CardType.Unit, "unit.archer")]
        [TestCase(CardType.Unit, "unit.guardian", CardType.Spell, "spell.fireball")]
        public void Constructor_WithDuplicatePhysicalId_Throws(
            CardType firstType, string firstContent, CardType secondType, string secondContent)
        {
            var cards = new[]
            {
                CreateCard(10, firstType, firstContent),
                CreateCard(10, secondType, secondContent)
            };

            Assert.Throws<ArgumentException>(() => new Deck(cards));
        }

        [Test]
        public void Constructor_WithSamePhysicalCardTwice_Throws()
        {
            var card = CreateCard(10, CardType.Spell, "spell.fireball");

            Assert.Throws<ArgumentException>(() => new Deck(new[] { card, card }));
        }

        [Test]
        public void Constructor_WithNullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new Deck(null!));
        }

        [Test]
        public void Constructor_WithNullEntry_Throws()
        {
            Assert.Throws<ArgumentException>(() => new Deck(new[]
            {
                CreateCard(1, CardType.Unit, "unit.guardian"), null!
            }));
        }

        [Test]
        public void Constructor_WithEmptyCollection_CreatesEmptyDeck()
        {
            var deck = new Deck(Array.Empty<RuntimeCard>());

            Assert.That(deck.Count, Is.Zero);
            Assert.That(deck.Cards, Is.Empty);
        }

        [Test]
        public void Constructor_WhenSourceListChanges_PreservesComposition()
        {
            var guardian = CreateCard(1, CardType.Unit, "unit.guardian");
            var source = new List<RuntimeCard> { guardian };
            var deck = new Deck(source);

            source.Clear();
            source.Add(CreateCard(2, CardType.Spell, "spell.fireball"));

            Assert.That(deck.Count, Is.EqualTo(1));
            Assert.That(deck.Cards[0], Is.SameAs(guardian));
        }

        [Test]
        public void Cards_WhenCastToMutableCollection_RejectsMutation()
        {
            var guardian = CreateCard(1, CardType.Unit, "unit.guardian");
            var deck = new Deck(new[] { guardian });
            var cards = (IList<RuntimeCard>)deck.Cards;
            var spell = CreateCard(2, CardType.Spell, "spell.fireball");

            Assert.That(cards.IsReadOnly, Is.True);
            Assert.Throws<NotSupportedException>(() => cards.Add(spell));
            Assert.Throws<NotSupportedException>(() => cards[0] = spell);
            Assert.Throws<NotSupportedException>(() => cards.Clear());
            Assert.That(deck.Count, Is.EqualTo(1));
            Assert.That(deck.Cards[0], Is.SameAs(guardian));
        }

        private static RuntimeCard CreateCard(ulong id, CardType type, string content)
        {
            return new RuntimeCard(new CardInstanceId(id), type, ContentId.Create(content).Value);
        }
    }
}
