using System;
using System.Collections.Generic;
using System.Linq;
using OverPower.Domain.Localization;

namespace OverPower.Domain.Content.Units
{
    /// <summary>
    /// An immutable definition representing an authored unit type and its base stats.
    /// </summary>
    public sealed class UnitDefinition
    {
        public ContentId Id { get; }
        public LocalizationKey NameKey { get; }
        public LocalizationKey DescriptionKey { get; }
        public int MaxHpPerMember { get; }
        public int Armor { get; }
        public int Attack { get; }
        public IReadOnlyList<ContentId> AbilityIds { get; }

        public UnitDefinition(
            ContentId id, 
            LocalizationKey nameKey, 
            LocalizationKey descriptionKey, 
            int maxHpPerMember, 
            int armor, 
            int attack, 
            IEnumerable<ContentId> abilityIds)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id), "Unit ID cannot be null.");
            NameKey = nameKey;
            DescriptionKey = descriptionKey;

            if (!id.Value.StartsWith("unit.", StringComparison.Ordinal))
            {
                throw new ArgumentException($"Unit ID '{id}' must start with the 'unit.' prefix.", nameof(id));
            }

            if (maxHpPerMember <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxHpPerMember), "Max HP per member must be greater than 0.");
            }

            if (armor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(armor), "Armor must be non-negative.");
            }

            if (attack < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(attack), "Attack must be non-negative.");
            }

            MaxHpPerMember = maxHpPerMember;
            Armor = armor;
            Attack = attack;

            if (abilityIds == null)
            {
                throw new ArgumentNullException(nameof(abilityIds), "Ability IDs collection cannot be null.");
            }

            // Create a defensive read-only copy of the ability list and reject any null entries.
            var listCopy = abilityIds.ToList();
            if (listCopy.Any(a => a == null))
            {
                throw new ArgumentException("Ability IDs collection cannot contain null references.", nameof(abilityIds));
            }
            AbilityIds = listCopy.AsReadOnly();
        }
    }
}
