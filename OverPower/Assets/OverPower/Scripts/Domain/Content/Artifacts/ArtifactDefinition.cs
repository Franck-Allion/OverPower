using System;
using OverPower.Domain.Localization;

namespace OverPower.Domain.Content.Artifacts
{
    /// <summary>
    /// An immutable definition representing an authored static artifact/relic.
    /// </summary>
    public sealed class ArtifactDefinition
    {
        public ContentId Id { get; }
        public LocalizationKey NameKey { get; }
        public LocalizationKey DescriptionKey { get; }

        public ArtifactDefinition(ContentId id, LocalizationKey nameKey, LocalizationKey descriptionKey)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id), "Artifact ID cannot be null.");
            NameKey = nameKey;
            DescriptionKey = descriptionKey;

            if (!id.Value.StartsWith("artifact.", StringComparison.Ordinal))
            {
                throw new ArgumentException($"Artifact ID '{id}' must start with the 'artifact.' prefix.", nameof(id));
            }
        }
    }
}
