using System;
using System.Collections.Generic;
using NUnit.Framework;
using OverPower.Domain.Cards;
using OverPower.Domain.Content;
using OverPower.Domain.Random;
using OverPower.Domain.Run;
using OverPower.Infrastructure.Random;

namespace OverPower.Tests.Infrastructure
{
    [TestFixture]
    public class StartingHandBuilderIntegrationTests
    {
        private static RuntimeCard CreateCard(ulong id, CardType type, string content)
        {
            return new RuntimeCard(new CardInstanceId(id), type, ContentId.Create(content).Value);
        }

        [Test]
        public void Create_WithProductionRNG_IsDeterministicUnderRunSeed()
        {
            // Arrange
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

            var runSeed = new RunSeed(12345UL);

            // Act
            var random1 = new SeededRandomService(runSeed.Value);
            var result1 = StartingHandBuilder.Create(deck, config, random1);

            var random2 = new SeededRandomService(runSeed.Value);
            var result2 = StartingHandBuilder.Create(deck, config, random2);

            // Assert
            Assert.That(result1.Hand.Count, Is.EqualTo(result2.Hand.Count));
            for (int i = 0; i < result1.Hand.Count; i++)
            {
                Assert.That(result1.Hand.Cards[i].InstanceId, Is.EqualTo(result2.Hand.Cards[i].InstanceId));
            }

            Assert.That(result1.DrawPile.Count, Is.EqualTo(result2.DrawPile.Count));
            for (int i = 0; i < result1.DrawPile.Count; i++)
            {
                Assert.That(result1.DrawPile.Cards[i].InstanceId, Is.EqualTo(result2.DrawPile.Cards[i].InstanceId));
            }

            // Verify hand contains at least one Unit
            bool hasUnit = false;
            foreach (var card in result1.Hand.Cards)
            {
                if (card.Type == CardType.Unit) hasUnit = true;
            }
            Assert.That(hasUnit, Is.True);
        }

        [Test]
        public void Create_WithProductionRNG_IsIsolatedBySubstream()
        {
            // Arrange
            var cards = new[]
            {
                CreateCard(1, CardType.Unit, "unit.guardian"),
                CreateCard(2, CardType.Spell, "spell.fireball"),
                CreateCard(3, CardType.Spell, "spell.heal"),
                CreateCard(4, CardType.Spell, "spell.fireball")
            };
            var deck = new Deck(cards);
            var config = CardDrawConfiguration.MvpDefault;

            ulong sharedSeed = 42UL;

            // Scenario 1: build starting hand directly
            var randomDirect = new SeededRandomService(sharedSeed);
            var directResult = StartingHandBuilder.Create(deck, config, randomDirect);

            // Scenario 2: consume other root RNG values first, then build starting hand
            var randomIndirect = new SeededRandomService(sharedSeed);
            
            // Consume unrelated root random states (simulate other systems)
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
    }
}
