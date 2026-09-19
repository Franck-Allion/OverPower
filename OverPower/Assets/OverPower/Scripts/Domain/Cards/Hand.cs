using System;
using System.Collections.Generic;

namespace OverPower.Domain.Cards
{
    /// <summary>
    /// Represents a player's hand of cards in battle, subject to a maximum capacity constraint.
    /// Contains unique physical card instances based on physical ID.
    /// </summary>
    public sealed class Hand
    {
        private readonly List<RuntimeCard> _cards;
        private readonly HashSet<CardInstanceId> _instanceIds;

        public int MaximumSize { get; }
        public int Count => _cards.Count;
        public bool IsFull => _cards.Count >= MaximumSize;
        public IReadOnlyList<RuntimeCard> Cards => _cards.AsReadOnly();

        public Hand(int maximumSize)
        {
            if (maximumSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumSize), "Maximum hand size must be greater than zero.");
            }

            MaximumSize = maximumSize;
            _cards = new List<RuntimeCard>();
            _instanceIds = new HashSet<CardInstanceId>();
        }

        /// <summary>
        /// Attempts to add a card to the hand.
        /// Returns false if the hand is full.
        /// Throws ArgumentNullException if the card is null.
        /// Throws InvalidOperationException if a card with the same physical ID already exists in the hand.
        /// </summary>
        public bool TryAdd(RuntimeCard card)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card), "Cannot add a null card to the hand.");
            }

            if (IsFull)
            {
                return false;
            }

            if (_instanceIds.Contains(card.InstanceId))
            {
                throw new InvalidOperationException($"A card with physical ID '{card.InstanceId}' already exists in the hand.");
            }

            _instanceIds.Add(card.InstanceId);
            _cards.Add(card);
            return true;
        }

        /// <summary>
        /// Removes a specific card from the hand.
        /// </summary>
        public bool Remove(RuntimeCard card)
        {
            if (card == null)
            {
                return false;
            }

            if (_instanceIds.Remove(card.InstanceId))
            {
                for (int i = 0; i < _cards.Count; i++)
                {
                    if (_cards[i].InstanceId == card.InstanceId)
                    {
                        _cards.RemoveAt(i);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
