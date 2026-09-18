using System;
using OverPower.Domain.Localization;

namespace OverPower.Domain.Content.Resources
{
    /// <summary>
    /// An immutable definition representing an authored resource type in the game (e.g. Gold, Mana).
    /// </summary>
    public sealed class ResourceType
    {
        public ContentId Id { get; }
        public LocalizationKey NameKey { get; }
        public LocalizationKey DescriptionKey { get; }

        public ResourceType(ContentId id, LocalizationKey nameKey, LocalizationKey descriptionKey)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id), "Resource ID cannot be null.");
            NameKey = nameKey;
            DescriptionKey = descriptionKey;

            if (!id.Value.StartsWith("resource.", StringComparison.Ordinal))
            {
                throw new ArgumentException($"Resource ID '{id}' must start with the 'resource.' prefix.", nameof(id));
            }
        }
    }
}
