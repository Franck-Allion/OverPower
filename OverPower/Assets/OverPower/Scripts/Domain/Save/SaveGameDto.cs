using System;
using System.Collections.Generic;
using OverPower.Domain.Content;

namespace OverPower.Domain.Save
{
    /// <summary>
    /// Pure immutable data contract for persistent save data representing the root state.
    /// </summary>
    public sealed class SaveGameDto
    {
        public int SchemaVersion { get; }
        public SettingsSaveDto Settings { get; }
        public int MetaCurrency { get; }
        public IReadOnlyList<string> UnlockIds { get; }
        public StatisticsSaveDto Statistics { get; }

        /// <summary>
        /// Creates a new SaveGameDto with explicit schema and semantic validation.
        /// </summary>
        public SaveGameDto(
            int schemaVersion,
            SettingsSaveDto settings,
            int metaCurrency,
            IEnumerable<string> unlockIds,
            StatisticsSaveDto statistics)
        {
            if (schemaVersion <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(schemaVersion), "Save schema version must be greater than zero.");
            }
            if (schemaVersion > SaveSchema.CurrentVersion)
            {
                throw new ArgumentException($"Save schema version {schemaVersion} is unsupported (current supported version is {SaveSchema.CurrentVersion}).", nameof(schemaVersion));
            }

            Settings = settings ?? throw new ArgumentNullException(nameof(settings), "Settings section cannot be null.");
            
            if (metaCurrency < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(metaCurrency), "Meta currency cannot be negative.");
            }
            MetaCurrency = metaCurrency;

            if (unlockIds == null)
            {
                throw new ArgumentNullException(nameof(unlockIds), "Unlock IDs collection cannot be null.");
            }

            var uniqueIds = new List<string>();
            foreach (var id in unlockIds)
            {
                if (id == null)
                {
                    throw new ArgumentException("Unlock IDs cannot contain null elements.", nameof(unlockIds));
                }
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentException("Unlock IDs cannot contain empty or whitespace elements.", nameof(unlockIds));
                }

                // Verify that the string conforms structurally to a valid ContentId.
                var contentIdResult = ContentId.Create(id);
                if (contentIdResult.IsFailure)
                {
                    throw new ArgumentException($"Invalid format for Content ID '{id}' inside unlock IDs.", nameof(unlockIds));
                }

                if (uniqueIds.Contains(id))
                {
                    throw new ArgumentException($"Duplicate unlock ID '{id}' is not allowed.", nameof(unlockIds));
                }

                uniqueIds.Add(id);
            }

            UnlockIds = uniqueIds.AsReadOnly();
            Statistics = statistics ?? throw new ArgumentNullException(nameof(statistics), "Statistics section cannot be null.");
            SchemaVersion = schemaVersion;
        }
    }
}
