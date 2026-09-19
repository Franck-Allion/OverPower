using System;
using System.Collections.Generic;
using NUnit.Framework;
using OverPower.Domain.Cards;
using OverPower.Domain.Content;
using OverPower.Domain.Random;

namespace OverPower.Tests.Domain
{
    [TestFixture]
    public class StartingHandBuilderTests
    {
        private class DeterministicTestRandom : IRandomService
        {
            private readonly int _seed;
            private readonly System.Random _random;
            private readonly string _streamId;

            public DeterministicTestRandom(int seed, string streamId = "root")
            {
                _seed = seed;
                _random = new System.Random(seed);
                _streamId = streamId;
            }

            public int NextInt(int maxExclusive)
            {
                if (maxExclusive <= 0) throw new ArgumentOutOfRangeException(nameof(maxExclusive));
                return _random.Next(maxExclusive);
            }

            public int NextInt(int minInclusive, int maxExclusive)
            {
                if (maxExclusive <= minInclusive) throw new ArgumentOutOfRangeException(nameof(maxExclusive));
                return _random.Next(minInclusive, maxExclusive);
            }

            public void Shuffle<T>(IList<T> items)
            {
                if (items == null) throw new ArgumentNullException(nameof(items));
                int n = items.Count;
                while (n > 1)
                {
                    n--;
                    int k = NextInt(n + 1);
                    T value = items[k];
                    items[k] = items[n];
                    items[n] = value;
                }
            }

            public IRandomService CreateSubstream(string streamId)
            {
                if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentException(nameof(streamId));
                
                // Pure FNV-1a hash of streamId combined with _seed
                uint hash = 2166136261;
                foreach (char c in streamId)
                {
                    hash = unchecked((hash ^ c) * 16777619);
                }
                int derivedSeed = unchecked((int)(hash ^ (uint)_seed));
                return new DeterministicTestRandom(derivedSeed, streamId);
            }
        }

        private static RuntimeCard CreateCard(ulong id, CardType type, string content)
        {
            return new RuntimeCard(new CardInstanceId(id), type, ContentId.Create(content).Value);
        }

        [Test]
        public void Create_NullArguments_ThrowsArgumentNullException()
        {
            var deck = new Deck(Array.Empty<RuntimeCard>());
            var config = CardDrawConfiguration.MvpDefault;
            var random = new DeterministicTestRandom(12345);

            Assert.Throws<ArgumentNullException>(() => StartingHandBuilder.Create(null!, config, random));
            Assert.Throws<ArgumentNullException>(() => StartingHandBuilder.Create(deck, null!, random));
            Assert.Throws<ArgumentNullException>(() => StartingHandBuilder.Create(deck, config, null!));
        }

        [Test]
        public void Create_EmptyDeck_ReturnsEmptyHandAndDrawPile()
        {
            var deck = new Deck(Array.Empty<RuntimeCard>());
            var config = CardDrawConfiguration.MvpDefault;
            var random = new DeterministicTestRandom(42);

            var result = StartingHandBuilder.Create(deck, config, random);

            Assert.That(result.Hand.Count, Is.Zero);
            Assert.That(result.DrawPile.Count, Is.Zero);
            Assert.That(result.DrawPile.IsEmpty, Is.True);
        }

        [Test]
        public void Create_StartingHandSizeZero_ReturnsEmptyHandAndAllCardsInDrawPile()
        {
            var cards = new[]
            {
                CreateCard(1, CardType.Unit, "unit.guardian"),
                CreateCard(2, CardType.Spell, "spell.fireball"),
                CreateCard(3, CardType.Spell, "spell.heal")
            };
            var deck = new Deck(cards);
            var config = new CardDrawConfiguration(0, 4, 1);
            var random = new DeterministicTestRandom(42);

            var result = StartingHandBuilder.Create(deck, config, random);

            Assert.That(result.Hand.Count, Is.Zero);
            Assert.That(result.DrawPile.Count, Is.EqualTo(3));

            // Verify conservation
            var idsInDrawPile = new HashSet<ulong>();
            foreach (var card in result.DrawPile.Cards)
            {
                idsInDrawPile.Add(card.InstanceId.Value);
            }
            Assert.That(idsInDrawPile, Is.EquivalentTo(new ulong[] { 1, 2, 3 }));
        }

        [Test]
        public void Create_DeckSmallerThanStartingSize_PutsAllCardsInHand()
        {
            var cards = new[]
            {
                CreateCard(1, CardType.Unit, "unit.guardian"),
                CreateCard(2, CardType.Spell, "spell.fireball")
            };
            var deck = new Deck(cards);
            var config = new CardDrawConfiguration(3, 4, 1); // Starting size 3, deck has 2
            var random = new DeterministicTestRandom(42);

            var result = StartingHandBuilder.Create(deck, config, random);

            Assert.That(result.Hand.Count, Is.EqualTo(2));
            Assert.That(result.DrawPile.Count, Is.Zero);

            // Unit is naturally preserved in hand
            bool hasUnit = false;
            foreach (var card in result.Hand.Cards)
            {
                if (card.Type == CardType.Unit) hasUnit = true;
            }
            Assert.That(hasUnit, Is.True);
        }

        [Test]
        public void Create_OneCardUnitDeck_PutsUnitInHand()
        {
            var card = CreateCard(1, CardType.Unit, "unit.guardian");
            var deck = new Deck(new[] { card });
            var config = CardDrawConfiguration.MvpDefault;
            var random = new DeterministicTestRandom(42);

            var result = StartingHandBuilder.Create(deck, config, random);

            Assert.That(result.Hand.Count, Is.EqualTo(1));
            Assert.That(result.Hand.Cards[0], Is.SameAs(card));
            Assert.That(result.DrawPile.Count, Is.Zero);
        }

        [Test]
        public void Create_OneCardSpellDeck_PutsSpellInHand()
        {
            var card = CreateCard(1, CardType.Spell, "spell.fireball");
            var deck = new Deck(new[] { card });
            var config = CardDrawConfiguration.MvpDefault;
            var random = new DeterministicTestRandom(42);

            var result = StartingHandBuilder.Create(deck, config, random);

            Assert.That(result.Hand.Count, Is.EqualTo(1));
            Assert.That(result.Hand.Cards[0], Is.SameAs(card));
            Assert.That(result.DrawPile.Count, Is.Zero);
        }

        [Test]
        public void Create_StartingHandSizeOneWithUnitAvailable_GuaranteesUnitInHand()
        {
            var cards = new[]
            {
                CreateCard(1, CardType.Spell, "spell.fireball"),
                CreateCard(2, CardType.Spell, "spell.heal"),
                CreateCard(3, CardType.Unit, "unit.guardian") // Unit is at index 2
            };
            var deck = new Deck(cards);
            var config = new CardDrawConfiguration(1, 4, 1); // Starting hand size 1
            var random = new DeterministicTestRandom(999);

            var result = StartingHandBuilder.Create(deck, config, random);

            Assert.That(result.Hand.Count, Is.EqualTo(1));
            Assert.That(result.Hand.Cards[0].Type, Is.EqualTo(CardType.Unit));
            Assert.That(result.Hand.Cards[0].InstanceId.Value, Is.EqualTo(3UL)); // Must swap guardian in
            Assert.That(result.DrawPile.Count, Is.EqualTo(2));
        }

        [Test]
        public void Create_SpellsOnlyDeck_NoExceptionAndBuildsNormally()
        {
            var cards = new[]
            {
                CreateCard(1, CardType.Spell, "spell.fireball"),
                CreateCard(2, CardType.Spell, "spell.heal"),
                CreateCard(3, CardType.Spell, "spell.fireball")
            };
            var deck = new Deck(cards);
            var config = new CardDrawConfiguration(2, 4, 1);
            var random = new DeterministicTestRandom(12345);

            var result = StartingHandBuilder.Create(deck, config, random);

            Assert.That(result.Hand.Count, Is.EqualTo(2));
            Assert.That(result.DrawPile.Count, Is.EqualTo(1));

            // Verify card conservation
            var allResultIds = new List<ulong>();
            foreach (var c in result.Hand.Cards) allResultIds.Add(c.InstanceId.Value);
            foreach (var c in result.DrawPile.Cards) allResultIds.Add(c.InstanceId.Value);

            Assert.That(allResultIds, Is.EquivalentTo(new ulong[] { 1, 2, 3 }));
        }

        [Test]
        public void Create_DeckWithUnitAndUnitNaturallyInOpening_NoGuaranteeReplacementNeeded()
        {
            var cards = new[]
            {
                CreateCard(1, CardType.Unit, "unit.guardian"),
                CreateCard(2, CardType.Spell, "spell.fireball"),
                CreateCard(3, CardType.Spell, "spell.heal"),
                CreateCard(4, CardType.Spell, "spell.fireball"),
                CreateCard(5, CardType.Spell, "spell.heal")
            };
            var deck = new Deck(cards);
            var config = CardDrawConfiguration.MvpDefault; // Starting size 3

            // Find a seed where the unit is naturally in the opening 3 cards
            int matchingSeed = -1;
            List<RuntimeCard> naturalList = null!;

            for (int seed = 0; seed < 1000; seed++)
            {
                var testRng = new DeterministicTestRandom(seed);
                var sub = testRng.CreateSubstream("battle.starting-hand");
                var list = new List<RuntimeCard>(cards);
                sub.Shuffle(list);

                if (list[0].Type == CardType.Unit || list[1].Type == CardType.Unit || list[2].Type == CardType.Unit)
                {
                    matchingSeed = seed;
                    naturalList = list;
                    break;
                }
            }

            Assert.That(matchingSeed, Is.Not.EqualTo(-1), "Should find a seed that naturally places the unit in the starting hand.");

            // Act
            var result = StartingHandBuilder.Create(deck, config, new DeterministicTestRandom(matchingSeed));

            // Assert - Hand contains exactly the first 3 cards from the shuffled list
            Assert.That(result.Hand.Count, Is.EqualTo(3));
            Assert.That(result.Hand.Cards[0], Is.SameAs(naturalList[0]));
            Assert.That(result.Hand.Cards[1], Is.SameAs(naturalList[1]));
            Assert.That(result.Hand.Cards[2], Is.SameAs(naturalList[2]));

            Assert.That(result.DrawPile.Count, Is.EqualTo(2));
            Assert.That(result.DrawPile.Cards[0], Is.SameAs(naturalList[3]));
            Assert.That(result.DrawPile.Cards[1], Is.SameAs(naturalList[4]));
        }

        [Test]
        public void Create_DeckWithUnitAndUnitNotInOpening_CorrectlySwapsOneUnitIntoHand()
        {
            var cards = new[]
            {
                CreateCard(1, CardType.Unit, "unit.guardian"),
                CreateCard(2, CardType.Spell, "spell.fireball"),
                CreateCard(3, CardType.Spell, "spell.heal"),
                CreateCard(4, CardType.Spell, "spell.fireball"),
                CreateCard(5, CardType.Spell, "spell.heal")
            };
            var deck = new Deck(cards);
            var config = CardDrawConfiguration.MvpDefault; // Starting size 3

            // Find a seed where the unit is NOT in the opening 3 cards
            int matchingSeed = -1;
            List<RuntimeCard> naturalList = null!;

            for (int seed = 0; seed < 1000; seed++)
            {
                var testRng = new DeterministicTestRandom(seed);
                var sub = testRng.CreateSubstream("battle.starting-hand");
                var list = new List<RuntimeCard>(cards);
                sub.Shuffle(list);

                if (list[0].Type != CardType.Unit && list[1].Type != CardType.Unit && list[2].Type != CardType.Unit)
                {
                    matchingSeed = seed;
                    naturalList = list;
                    break;
                }
            }

            Assert.That(matchingSeed, Is.Not.EqualTo(-1), "Should find a seed that naturally excludes the unit from the starting hand.");

            // Act
            var result = StartingHandBuilder.Create(deck, config, new DeterministicTestRandom(matchingSeed));

            // Assert
            Assert.That(result.Hand.Count, Is.EqualTo(3));
            Assert.That(result.DrawPile.Count, Is.EqualTo(2));

            // Verify hand contains at least one Unit card now!
            bool hasUnitInHand = false;
            foreach (var card in result.Hand.Cards)
            {
                if (card.Type == CardType.Unit) hasUnitInHand = true;
            }
            Assert.That(hasUnitInHand, Is.True, "The unit card must have been swapped into the opening hand.");

            // Verify full card conservation (no duplicates, no card lost)
            var allResultIds = new List<ulong>();
            foreach (var c in result.Hand.Cards) allResultIds.Add(c.InstanceId.Value);
            foreach (var c in result.DrawPile.Cards) allResultIds.Add(c.InstanceId.Value);

            Assert.That(allResultIds, Is.EquivalentTo(new ulong[] { 1, 2, 3, 4, 5 }));
            Assert.That(allResultIds, Is.Unique);
        }

        [Test]
        public void Create_MultipleUnitTypes_DeterministicSelectionAndPreservation()
        {
            // Deck contains multiple units (guardian, archer) and multiple spells
            var cards = new[]
            {
                CreateCard(1, CardType.Unit, "unit.guardian"),
                CreateCard(2, CardType.Unit, "unit.archer"),
                CreateCard(10, CardType.Spell, "spell.fireball"),
                CreateCard(11, CardType.Spell, "spell.heal"),
                CreateCard(12, CardType.Spell, "spell.fireball")
            };
            var deck = new Deck(cards);
            var config = CardDrawConfiguration.MvpDefault; // Starting size 3

            // Let's use two different seeds to verify they behave deterministically and can result in different unit selections or hand composition.
            var seed1 = 12345;
            var seed2 = 54321;

            var resultA1 = StartingHandBuilder.Create(deck, config, new DeterministicTestRandom(seed1));
            var resultA2 = StartingHandBuilder.Create(deck, config, new DeterministicTestRandom(seed1));

            // Assert Determinism
            Assert.That(resultA1.Hand.Count, Is.EqualTo(resultA2.Hand.Count));
            for (int i = 0; i < resultA1.Hand.Count; i++)
            {
                Assert.That(resultA1.Hand.Cards[i].InstanceId, Is.EqualTo(resultA2.Hand.Cards[i].InstanceId));
            }
            for (int i = 0; i < resultA1.DrawPile.Count; i++)
            {
                Assert.That(resultA1.DrawPile.Cards[i].InstanceId, Is.EqualTo(resultA2.DrawPile.Cards[i].InstanceId));
            }

            // Let's check with seed2
            var resultB = StartingHandBuilder.Create(deck, config, new DeterministicTestRandom(seed2));

            // Verify Hand & DrawPile card identity conservation for both results
            foreach (var result in new[] { resultA1, resultB })
            {
                var ids = new List<ulong>();
                foreach (var c in result.Hand.Cards) ids.Add(c.InstanceId.Value);
                foreach (var c in result.DrawPile.Cards) ids.Add(c.InstanceId.Value);

                Assert.That(ids, Is.EquivalentTo(new ulong[] { 1, 2, 10, 11, 12 }));
                Assert.That(ids, Is.Unique);

                // Opening hand must have at least one Unit
                bool hasUnit = false;
                foreach (var c in result.Hand.Cards)
                {
                    if (c.Type == CardType.Unit) hasUnit = true;
                }
                Assert.That(hasUnit, Is.True);
            }
        }

        [Test]
        public void Create_RngSubstreamIsolation_ProtectsOpeningHandFromExternalRngConsumption()
        {
            var cards = new[]
            {
                CreateCard(1, CardType.Unit, "unit.guardian"),
                CreateCard(2, CardType.Spell, "spell.fireball"),
                CreateCard(3, CardType.Spell, "spell.heal"),
                CreateCard(4, CardType.Spell, "spell.fireball")
            };
            var deck = new Deck(cards);
            var config = CardDrawConfiguration.MvpDefault;

            int sharedSeed = 42;

            // Scenario 1: build starting hand directly
            var randomDirect = new DeterministicTestRandom(sharedSeed);
            var directResult = StartingHandBuilder.Create(deck, config, randomDirect);

            // Scenario 2: consume other root RNG values first, then build starting hand
            var randomIndirect = new DeterministicTestRandom(sharedSeed);
            // Simulate unrelated system using the root RNG (e.g. exploration movements, other substreams)
            randomIndirect.NextInt(10);
            randomIndirect.CreateSubstream("run.exploration").NextInt(100);

            var indirectResult = StartingHandBuilder.Create(deck, config, randomIndirect);

            // Assert - Hand and DrawPile sequences must be completely identical, proving substream isolation
            Assert.That(directResult.Hand.Count, Is.EqualTo(indirectResult.Hand.Count));
            for (int i = 0; i < directResult.Hand.Count; i++)
            {
                Assert.That(directResult.Hand.Cards[i].InstanceId, Is.EqualTo(indirectResult.Hand.Cards[i].InstanceId));
            }
            Assert.That(directResult.DrawPile.Count, Is.EqualTo(indirectResult.DrawPile.Count));
            for (int i = 0; i < directResult.DrawPile.Count; i++)
            {
                Assert.That(directResult.DrawPile.Cards[i].InstanceId, Is.EqualTo(indirectResult.DrawPile.Cards[i].InstanceId));
            }
        }

        [Test]
        public void Create_PreservesOriginalObjectReferencesAndDoesNotClone()
        {
            var card1 = CreateCard(1, CardType.Unit, "unit.guardian");
            var card2 = CreateCard(2, CardType.Spell, "spell.fireball");
            var card3 = CreateCard(3, CardType.Spell, "spell.heal");

            var cards = new[] { card1, card2, card3 };
            var deck = new Deck(cards);
            var config = CardDrawConfiguration.MvpDefault;
            var random = new DeterministicTestRandom(42);

            var result = StartingHandBuilder.Create(deck, config, random);

            foreach (var card in result.Hand.Cards)
            {
                bool isSourceReference = ReferenceEquals(card, card1) || ReferenceEquals(card, card2) || ReferenceEquals(card, card3);
                Assert.That(isSourceReference, Is.True, "Cards inside Hand must be exact physical source references.");
            }

            foreach (var card in result.DrawPile.Cards)
            {
                bool isSourceReference = ReferenceEquals(card, card1) || ReferenceEquals(card, card2) || ReferenceEquals(card, card3);
                Assert.That(isSourceReference, Is.True, "Cards inside DrawPile must be exact physical source references.");
            }
        }
    }
}
