using System;
using OverPower.Domain.Localization;

namespace OverPower.Domain.Content.Spells
{
    /// <summary>
    /// An immutable definition representing an authored spell card and its mana cost.
    /// </summary>
    public sealed class SpellDefinition
    {
        public ContentId Id { get; }
        public LocalizationKey NameKey { get; }
        public LocalizationKey DescriptionKey { get; }
        public int ManaCost { get; }

        public SpellDefinition(ContentId id, LocalizationKey nameKey, LocalizationKey descriptionKey, int manaCost)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id), "Spell ID cannot be null.");
            NameKey = nameKey;
            DescriptionKey = descriptionKey;

            if (!id.Value.StartsWith("spell.", StringComparison.Ordinal))
            {
                throw new ArgumentException($"Spell ID '{id}' must start with the 'spell.' prefix.", nameof(id));
            }

            if (manaCost < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(manaCost), "Mana cost must be non-negative.");
            }

            ManaCost = manaCost;
        }
    }
}
