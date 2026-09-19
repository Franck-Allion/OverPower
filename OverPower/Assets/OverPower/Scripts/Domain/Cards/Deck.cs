using System;
using System.Collections.Generic;
using OverPower.Domain.Content;

namespace OverPower.Domain.Cards
{
    /// <summary>
    /// Immutable deck composition, preserving supplied card references and order.
    /// Unit cards identify unit types; recruited quantities belong to the run roster.
    /// </summary>
    public sealed class Deck
    {
        public IReadOnlyList<RuntimeCard> Cards { get; }
        public int Count => Cards.Count;

        public Deck(IEnumerable<RuntimeCard> cards)
        {
            if (cards == null)
            {
                throw new ArgumentNullException(nameof(cards));
            }

            var cardList = new List<RuntimeCard>();
            var instanceIds = new HashSet<CardInstanceId>();
            var unitIds = new HashSet<ContentId>();

            foreach (RuntimeCard card in cards)
            {
                if (card == null)
                {
                    throw new ArgumentException("Deck cannot contain null cards.", nameof(cards));
                }

                if (!instanceIds.Add(card.InstanceId))
                {
                    throw new ArgumentException($"Duplicate physical card ID '{card.InstanceId}'.", nameof(cards));
                }

                if (card.Type == CardType.Unit && !unitIds.Add(card.ContentId))
                {
                    throw new ArgumentException($"Deck can contain only one card for unit type '{card.ContentId}'.", nameof(cards));
                }

                cardList.Add(card);
            }

            Cards = cardList.AsReadOnly();
        }
    }
}
