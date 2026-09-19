using System;
using System.Collections.Generic;
using OverPower.Domain.Random;

namespace OverPower.Domain.Cards
{
    /// <summary>
    /// Holds the atomically generated starting Hand and DrawPile.
    /// </summary>
    public sealed class StartingHandResult
    {
        public Hand Hand { get; }
        public DrawPile DrawPile { get; }

        public StartingHandResult(Hand hand, DrawPile drawPile)
        {
            Hand = hand ?? throw new ArgumentNullException(nameof(hand));
            DrawPile = drawPile ?? throw new ArgumentNullException(nameof(drawPile));
        }
    }

    /// <summary>
    /// Dedicated operation to build the starting battle hand and draw pile deterministically from a deck.
    /// </summary>
    public static class StartingHandBuilder
    {
        private const string SubstreamId = "battle.starting-hand";

        /// <summary>
        /// Creates a starting hand and draw pile atomically under a deterministic seed.
        /// If the deck contains at least one Unit card and StartingHandSize > 0, the resulting hand
        /// is guaranteed to contain at least one Unit card, preserving randomness for all other cards.
        /// </summary>
        public static StartingHandResult Create(
            Deck deck,
            CardDrawConfiguration configuration,
            IRandomService random)
        {
            if (deck == null)
            {
                throw new ArgumentNullException(nameof(deck));
            }
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            // If starting hand size is 0 or deck is empty, build empty hand and remaining/empty draw pile.
            if (configuration.StartingHandSize == 0 || deck.Count == 0)
            {
                var workingCards = new List<RuntimeCard>(deck.Cards);
                var substream = random.CreateSubstream(SubstreamId);
                substream.Shuffle(workingCards);

                return new StartingHandResult(
                    new Hand(configuration.MaximumHandSize),
                    new DrawPile(workingCards));
            }

            // If deck size is smaller than or equal to starting hand size, draw all available cards.
            if (deck.Count <= configuration.StartingHandSize)
            {
                var workingCards = new List<RuntimeCard>(deck.Cards);
                var substream = random.CreateSubstream(SubstreamId);
                substream.Shuffle(workingCards);

                var hand = new Hand(configuration.MaximumHandSize);
                foreach (var card in workingCards)
                {
                    hand.TryAdd(card);
                }

                return new StartingHandResult(hand, new DrawPile(Array.Empty<RuntimeCard>()));
            }

            // General case: deck.Count > StartingHandSize > 0
            var cardsCopy = new List<RuntimeCard>(deck.Cards);
            var cardRandom = random.CreateSubstream(SubstreamId);
            cardRandom.Shuffle(cardsCopy);

            bool hasUnitInDeck = false;
            foreach (var card in deck.Cards)
            {
                if (card.Type == CardType.Unit)
                {
                    hasUnitInDeck = true;
                    break;
                }
            }

            int openingSize = configuration.StartingHandSize;

            if (hasUnitInDeck)
            {
                bool hasUnitInOpening = false;
                for (int i = 0; i < openingSize; i++)
                {
                    if (cardsCopy[i].Type == CardType.Unit)
                    {
                        hasUnitInOpening = true;
                        break;
                    }
                }

                // If opening hand contains no unit card, swap one unit card from outside with a card inside.
                if (!hasUnitInOpening)
                {
                    var outerUnitIndices = new List<int>();
                    for (int i = openingSize; i < cardsCopy.Count; i++)
                    {
                        if (cardsCopy[i].Type == CardType.Unit)
                        {
                            outerUnitIndices.Add(i);
                        }
                    }

                    // Select one unit outside the opening window deterministically.
                    int selectedUnitListIndex = outerUnitIndices[cardRandom.NextInt(outerUnitIndices.Count)];

                    // Select one card in the opening window to be replaced/swapped deterministically.
                    int targetHandIndex = cardRandom.NextInt(openingSize);

                    // Swap them to satisfy the unit-card guarantee while preserving remaining order and randomness.
                    RuntimeCard temp = cardsCopy[targetHandIndex];
                    cardsCopy[targetHandIndex] = cardsCopy[selectedUnitListIndex];
                    cardsCopy[selectedUnitListIndex] = temp;
                }
            }

            var handResult = new Hand(configuration.MaximumHandSize);
            for (int i = 0; i < openingSize; i++)
            {
                handResult.TryAdd(cardsCopy[i]);
            }

            var drawPileCards = new List<RuntimeCard>();
            for (int i = openingSize; i < cardsCopy.Count; i++)
            {
                drawPileCards.Add(cardsCopy[i]);
            }
            var drawPileResult = new DrawPile(drawPileCards);

            return new StartingHandResult(handResult, drawPileResult);
        }
    }
}
