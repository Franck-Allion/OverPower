using System;
using System.Collections.Generic;
using NUnit.Framework;
using OverPower.Domain.Cards;
using OverPower.Domain.Content;

namespace OverPower.Tests.Domain
{
    [TestFixture]
    public class CardContainerTests
    {
        private RuntimeCard CreateCard(ulong id, CardType type, string content)
        {
            return new RuntimeCard(new CardInstanceId(id), type, ContentId.Create(content).Value);
        }

        #region Configuration Tests

        [Test]
        public void Configuration_MvpDefault_IsCorrect()
        {
            var config = CardDrawConfiguration.MvpDefault;

            Assert.That(config.StartingHandSize, Is.EqualTo(3));
            Assert.That(config.MaximumHandSize, Is.EqualTo(4));
            Assert.That(config.DrawCountPerTurn, Is.EqualTo(1));
        }

        [Test]
        public void Configuration_ValidSetup_Succeeds()
        {
            var config = new CardDrawConfiguration(3, 4, 1);

            Assert.That(config.StartingHandSize, Is.EqualTo(3));
            Assert.That(config.MaximumHandSize, Is.EqualTo(4));
            Assert.That(config.DrawCountPerTurn, Is.EqualTo(1));
        }

        [Test]
        public void Configuration_StartingGreaterThanMax_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new CardDrawConfiguration(5, 4, 1));
        }

        [Test]
        public void Configuration_NegativeStarting_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CardDrawConfiguration(-1, 4, 1));
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        public void Configuration_MaxLessThanOrEqualToZero_ThrowsArgumentOutOfRangeException(int max)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CardDrawConfiguration(0, max, 1));
        }

        [Test]
        public void Configuration_NegativeDrawCount_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CardDrawConfiguration(2, 4, -1));
        }

        [Test]
        public void Configuration_SupportsZeroDrawCountAndZeroStartingHand()
        {
            var config = new CardDrawConfiguration(0, 4, 0);

            Assert.That(config.StartingHandSize, Is.EqualTo(0));
            Assert.That(config.DrawCountPerTurn, Is.EqualTo(0));
        }

        #endregion

        #region DrawPile Tests

        [Test]
        public void DrawPile_Constructor_NullOrderedCards_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new DrawPile(null!));
        }

        [Test]
        public void DrawPile_Constructor_ListWithNull_ThrowsArgumentException()
        {
            var cards = new List<RuntimeCard> { CreateCard(1, CardType.Unit, "unit.guardian"), null! };
            Assert.Throws<ArgumentException>(() => new DrawPile(cards));
        }

        [Test]
        public void DrawPile_PreservesSuppliedPhysicalReferencesAndOrder()
        {
            var card1 = CreateCard(1, CardType.Unit, "unit.guardian");
            var card2 = CreateCard(2, CardType.Spell, "spell.fireball");
            var card3 = CreateCard(3, CardType.Spell, "spell.heal");

            var cards = new[] { card1, card2, card3 };
            var pile = new DrawPile(cards);

            Assert.That(pile.Count, Is.EqualTo(3));
            Assert.That(pile.IsEmpty, Is.False);
            Assert.That(pile.Cards[0], Is.SameAs(card1));
            Assert.That(pile.Cards[1], Is.SameAs(card2));
            Assert.That(pile.Cards[2], Is.SameAs(card3));
        }

        [Test]
        public void Draw_RemovesFirstTopCard_CountDecreases_IsEmptyCorrect()
        {
            var card1 = CreateCard(1, CardType.Unit, "unit.guardian");
            var card2 = CreateCard(2, CardType.Spell, "spell.fireball");

            var pile = new DrawPile(new[] { card1, card2 });

            var drawn1 = pile.Draw();
            Assert.That(drawn1, Is.SameAs(card1));
            Assert.That(pile.Count, Is.EqualTo(1));
            Assert.That(pile.IsEmpty, Is.False);

            var drawn2 = pile.Draw();
            Assert.That(drawn2, Is.SameAs(card2));
            Assert.That(pile.Count, Is.EqualTo(0));
            Assert.That(pile.IsEmpty, Is.True);
        }

        [Test]
        public void Draw_EmptyDrawPile_ReturnsNullAndNoFatigue()
        {
            var pile = new DrawPile(new RuntimeCard[0]);

            Assert.That(pile.IsEmpty, Is.True);
            Assert.That(pile.Draw(), Is.Null);
            Assert.That(pile.IsEmpty, Is.True);
        }

        #endregion

        #region Hand Tests

        [TestCase(0)]
        [TestCase(-1)]
        public void Hand_Constructor_InvalidMaxSize_ThrowsArgumentOutOfRangeException(int size)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Hand(size));
        }

        [Test]
        public void Hand_EnforcesMaximumSize()
        {
            var hand = new Hand(2);
            var card1 = CreateCard(1, CardType.Unit, "unit.guardian");
            var card2 = CreateCard(2, CardType.Spell, "spell.fireball");
            var card3 = CreateCard(3, CardType.Spell, "spell.heal");

            Assert.That(hand.TryAdd(card1), Is.True);
            Assert.That(hand.IsFull, Is.False);

            Assert.That(hand.TryAdd(card2), Is.True);
            Assert.That(hand.IsFull, Is.True);

            Assert.That(hand.TryAdd(card3), Is.False);
            Assert.That(hand.Count, Is.EqualTo(2));
        }

        [Test]
        public void Hand_TryAdd_NullCard_ThrowsArgumentNullException()
        {
            var hand = new Hand(4);
            Assert.Throws<ArgumentNullException>(() => hand.TryAdd(null!));
        }

        [Test]
        public void Hand_CannotAddSamePhysicalCardTwice()
        {
            var hand = new Hand(4);
            var card = CreateCard(1, CardType.Unit, "unit.guardian");

            Assert.That(hand.TryAdd(card), Is.True);
            Assert.Throws<InvalidOperationException>(() => hand.TryAdd(card));
        }

        [Test]
        public void Hand_Remove_CorrectlyRemovesAndAllowsReadding()
        {
            var hand = new Hand(4);
            var card = CreateCard(1, CardType.Unit, "unit.guardian");

            Assert.That(hand.TryAdd(card), Is.True);
            Assert.That(hand.Count, Is.EqualTo(1));

            Assert.That(hand.Remove(card), Is.True);
            Assert.That(hand.Count, Is.EqualTo(0));

            // Should be able to add back now that it was removed
            Assert.That(hand.TryAdd(card), Is.True);
        }

        [Test]
        public void Hand_Remove_NullOrNonExistent_ReturnsFalse()
        {
            var hand = new Hand(4);
            var card1 = CreateCard(1, CardType.Unit, "unit.guardian");
            var card2 = CreateCard(2, CardType.Spell, "spell.fireball");

            hand.TryAdd(card1);

            Assert.That(hand.Remove(null!), Is.False);
            Assert.That(hand.Remove(card2), Is.False);
            Assert.That(hand.Count, Is.EqualTo(1));
        }

        #endregion

        #region Draw To Hand Tests

        [Test]
        public void TryDrawToHand_NormalDraw_MovesSameRuntimeCard()
        {
            var card = CreateCard(1, CardType.Unit, "unit.guardian");
            var pile = new DrawPile(new[] { card });
            var hand = new Hand(4);

            bool success = pile.TryDrawToHand(hand);

            Assert.That(success, Is.True);
            Assert.That(hand.Count, Is.EqualTo(1));
            Assert.That(hand.Cards[0], Is.SameAs(card));
            Assert.That(pile.IsEmpty, Is.True);
        }

        [Test]
        public void TryDrawToHand_FullHand_ConsumesNothingAndReturnsFalse()
        {
            var card = CreateCard(1, CardType.Unit, "unit.guardian");
            var pile = new DrawPile(new[] { card });
            var hand = new Hand(1);
            hand.TryAdd(CreateCard(2, CardType.Spell, "spell.fireball"));

            bool success = pile.TryDrawToHand(hand);

            Assert.That(success, Is.False);
            Assert.That(pile.Count, Is.EqualTo(1));
            Assert.That(pile.Cards[0], Is.SameAs(card));
        }

        [Test]
        public void TryDrawToHand_EmptyPile_ChangesNothingAndReturnsFalse()
        {
            var pile = new DrawPile(new RuntimeCard[0]);
            var hand = new Hand(4);

            bool success = pile.TryDrawToHand(hand);

            Assert.That(success, Is.False);
            Assert.That(hand.Count, Is.EqualTo(0));
        }

        [Test]
        public void DrawToHand_RequestedCountZero_ChangesNothing()
        {
            var card = CreateCard(1, CardType.Unit, "unit.guardian");
            var pile = new DrawPile(new[] { card });
            var hand = new Hand(4);

            int drawn = pile.DrawToHand(hand, 0);

            Assert.That(drawn, Is.EqualTo(0));
            Assert.That(pile.Count, Is.EqualTo(1));
            Assert.That(hand.Count, Is.EqualTo(0));
        }

        [Test]
        public void DrawToHand_NegativeCount_ThrowsArgumentOutOfRangeException()
        {
            var pile = new DrawPile(new RuntimeCard[0]);
            var hand = new Hand(4);

            Assert.Throws<ArgumentOutOfRangeException>(() => pile.DrawToHand(hand, -1));
        }

        [Test]
        public void DrawToHand_PartialDraw_RequestThreeWithTwoAvailable_DrawsExactlyTwo()
        {
            var card1 = CreateCard(1, CardType.Unit, "unit.guardian");
            var card2 = CreateCard(2, CardType.Spell, "spell.fireball");
            var pile = new DrawPile(new[] { card1, card2 });
            var hand = new Hand(4);

            int drawn = pile.DrawToHand(hand, 3);

            Assert.That(drawn, Is.EqualTo(2));
            Assert.That(hand.Count, Is.EqualTo(2));
            Assert.That(hand.Cards[0], Is.SameAs(card1));
            Assert.That(hand.Cards[1], Is.SameAs(card2));
            Assert.That(pile.IsEmpty, Is.True);
        }

        [Test]
        public void DrawToHand_CapacityLimitedDraw_HandMaximumEnforced_PileRetainsRemaining()
        {
            var card1 = CreateCard(1, CardType.Unit, "unit.guardian");
            var card2 = CreateCard(2, CardType.Spell, "spell.fireball");
            var card3 = CreateCard(3, CardType.Spell, "spell.heal");

            var pile = new DrawPile(new[] { card1, card2, card3 });
            var hand = new Hand(2);
            hand.TryAdd(CreateCard(10, CardType.Spell, "spell.heal")); // 1 slot left

            int drawn = pile.DrawToHand(hand, 3);

            Assert.That(drawn, Is.EqualTo(1));
            Assert.That(hand.Count, Is.EqualTo(2));
            Assert.That(hand.Cards[1], Is.SameAs(card1));
            Assert.That(pile.Count, Is.EqualTo(2));
            Assert.That(pile.Cards[0], Is.SameAs(card2));
            Assert.That(pile.Cards[1], Is.SameAs(card3));
        }

        [Test]
        public void DrawToHand_IdentityAndConservationInvariants()
        {
            var card1 = CreateCard(1, CardType.Unit, "unit.guardian");
            var card2 = CreateCard(2, CardType.Spell, "spell.fireball");
            var card3 = CreateCard(3, CardType.Spell, "spell.heal");

            var pile = new DrawPile(new[] { card1, card2, card3 });
            var hand = new Hand(4);

            pile.DrawToHand(hand, 2);

            Assert.That(hand.Count, Is.EqualTo(2));
            Assert.That(hand.Cards[0].InstanceId.Value, Is.EqualTo(1UL));
            Assert.That(hand.Cards[1].InstanceId.Value, Is.EqualTo(2UL));

            Assert.That(pile.Count, Is.EqualTo(1));
            Assert.That(pile.Cards[0].InstanceId.Value, Is.EqualTo(3UL));
        }

        [Test]
        public void DrawToHand_GenericStartingHandRegression_DoesNotGuaranteeUnit()
        {
            // StartingHandSize = 3
            // Pile: fireball (spell), heal (spell), guardian (unit), fireball (spell)
            var card1 = CreateCard(1, CardType.Spell, "spell.fireball");
            var card2 = CreateCard(2, CardType.Spell, "spell.heal");
            var card3 = CreateCard(3, CardType.Unit, "unit.guardian");
            var card4 = CreateCard(4, CardType.Spell, "spell.fireball");

            var pile = new DrawPile(new[] { card1, card2, card3, card4 });
            var hand = new Hand(4);

            int drawn = pile.DrawToHand(hand, 3);

            Assert.That(drawn, Is.EqualTo(3));
            Assert.That(hand.Cards[0], Is.SameAs(card1));
            Assert.That(hand.Cards[1], Is.SameAs(card2));
            Assert.That(hand.Cards[2], Is.SameAs(card3));
            Assert.That(pile.Count, Is.EqualTo(1));
            Assert.That(pile.Cards[0], Is.SameAs(card4));
        }

        #endregion
    }
}
