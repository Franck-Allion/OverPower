using System;
using OverPower.Domain.Content;

namespace OverPower.Domain.Cards
{
    /// <summary>
    /// Immutable identity and content reference for a physical card.
    /// Uses reference equality; compare InstanceId explicitly for physical identity.
    /// </summary>
    public sealed class RuntimeCard
    {
        public CardInstanceId InstanceId { get; }
        public CardType Type { get; }
        public ContentId ContentId { get; }

        public RuntimeCard(CardInstanceId instanceId, CardType type, ContentId contentId)
        {
            if (instanceId.Value == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(instanceId), "Card instance ID must be greater than zero.");
            }

            ContentId = contentId ?? throw new ArgumentNullException(nameof(contentId));

            string prefix;
            switch (type)
            {
                case CardType.Unit:
                    prefix = "unit.";
                    break;
                case CardType.Spell:
                    prefix = "spell.";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported card type.");
            }

            if (!contentId.Value.StartsWith(prefix, StringComparison.Ordinal))
            {
                throw new ArgumentException($"Card content ID '{contentId}' must start with '{prefix}' for type '{type}'.", nameof(contentId));
            }

            InstanceId = instanceId;
            Type = type;
        }
    }
}
