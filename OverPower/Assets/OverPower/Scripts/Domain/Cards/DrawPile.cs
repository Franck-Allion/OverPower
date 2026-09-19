using System;
using System.Collections.Generic;

namespace OverPower.Domain.Cards
{
    /// <summary>
    /// Represents a mutable, ordered pile of cards not yet drawn.
    /// Index 0 represents the top of the pile (next card to be drawn).
    /// </summary>
    public sealed class DrawPile
    {
        private readonly List<RuntimeCard> _cards;

        public IReadOnlyList<RuntimeCard> Cards => _cards.AsReadOnly();
        public int Count => _cards.Count;
        public bool IsEmpty => _cards.Count == 0;

        public DrawPile(IEnumerable<RuntimeCard> orderedCards)
        {
            if (orderedCards == null)
            {
                throw new ArgumentNullException(nameof(orderedCards));
            }

            _cards = new List<RuntimeCard>();
            foreach (var card in orderedCards)
            {
                if (card == null)
                {
                    throw new ArgumentException("Draw pile cannot contain null cards.", nameof(orderedCards));
                }
                _cards.Add(card);
            }
        }

        /// <summary>
        /// Draws the top card from the pile.
        /// Index 0 is the top of the pile.
        /// Returns null if the pile is empty.
        /// </summary>
        public RuntimeCard? Draw()
        {
            if (IsEmpty)
            {
                return null;
            }

            RuntimeCard card = _cards[0];
            _cards.RemoveAt(0);
            return card;
        }

        /// <summary>
        /// Moves a card from this DrawPile into the hand, enforcing full-hand and empty-pile rules.
        /// </summary>
        public bool TryDrawToHand(Hand hand)
        {
            if (hand == null)
            {
                throw new ArgumentNullException(nameof(hand));
            }

            if (hand.IsFull || IsEmpty)
            {
                return false;
            }

            RuntimeCard? card = Draw();
            if (card == null)
            {
                return false;
            }

            try
            {
                bool added = hand.TryAdd(card);
                if (!added)
                {
                    // If the hand didn't add it (e.g. hand was full or similar), put it back at the top.
                    _cards.Insert(0, card);
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                // In case of duplicate exception or any other failure, put it back at the top to preserve card conservation.
                _cards.Insert(0, card);
                throw;
            }
        }

        /// <summary>
        /// Draws up to requested count of cards into the hand.
        /// Returns the number of cards actually drawn.
        /// </summary>
        public int DrawToHand(Hand hand, int requestedCount)
        {
            if (hand == null)
            {
                throw new ArgumentNullException(nameof(hand));
            }
            if (requestedCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(requestedCount), "Requested count cannot be negative.");
            }

            int drawn = 0;
            while (drawn < requestedCount && !IsEmpty && !hand.IsFull)
            {
                if (TryDrawToHand(hand))
                {
                    drawn++;
                }
                else
                {
                    break;
                }
            }

            return drawn;
        }
    }
}
