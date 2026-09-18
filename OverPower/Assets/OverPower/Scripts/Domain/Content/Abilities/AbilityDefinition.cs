using System;
using OverPower.Domain.Localization;

namespace OverPower.Domain.Content.Abilities
{
    /// <summary>
    /// An immutable definition representing an authored game ability.
    /// </summary>
    public sealed class AbilityDefinition
    {
        public ContentId Id { get; }
        public LocalizationKey NameKey { get; }
        public LocalizationKey DescriptionKey { get; }

        public AbilityDefinition(ContentId id, LocalizationKey nameKey, LocalizationKey descriptionKey)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id), "Ability ID cannot be null.");
            NameKey = nameKey;
            DescriptionKey = descriptionKey;

            if (!id.Value.StartsWith("ability.", StringComparison.Ordinal))
            {
                throw new ArgumentException($"Ability ID '{id}' must start with the 'ability.' prefix.", nameof(id));
            }
        }
    }
}
