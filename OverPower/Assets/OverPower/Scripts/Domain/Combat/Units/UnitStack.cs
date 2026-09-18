using System;
using OverPower.Domain.Content.Units;

namespace OverPower.Domain.Combat.Units
{
    /// <summary>
    /// Represents an authoritative runtime unit stack (squad) of identical units.
    /// </summary>
    public sealed class UnitStack
    {
        public UnitDefinition Definition { get; }
        public int InitialQuantity { get; }
        public int MaxHpPerMember => Definition.MaxHpPerMember;
        public int MaximumTotalHp => checked(InitialQuantity * MaxHpPerMember);
        public int TotalRemainingHp { get; private set; }

        public int DisplayedQuantity
        {
            get
            {
                if (TotalRemainingHp == 0) return 0;
                return (TotalRemainingHp + MaxHpPerMember - 1) / MaxHpPerMember;
            }
        }

        public int PartialCurrentMemberHp
        {
            get
            {
                if (TotalRemainingHp == 0) return 0;
                int remainder = TotalRemainingHp % MaxHpPerMember;
                return remainder == 0 ? MaxHpPerMember : remainder;
            }
        }

        private UnitStack(UnitDefinition definition, int initialQuantity, int totalRemainingHp)
        {
            Definition = definition;
            InitialQuantity = initialQuantity;
            TotalRemainingHp = totalRemainingHp;
        }

        /// <summary>
        /// Creates a new UnitStack with maximum total HP.
        /// </summary>
        public UnitStack(UnitDefinition definition, int initialQuantity)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition), "Unit definition cannot be null.");
            if (initialQuantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialQuantity), "Initial quantity must be greater than 0.");
            }

            InitialQuantity = initialQuantity;

            try
            {
                TotalRemainingHp = checked(initialQuantity * definition.MaxHpPerMember);
            }
            catch (OverflowException)
            {
                throw new ArgumentOutOfRangeException(nameof(initialQuantity), "The initial maximum total HP would overflow int.MaxValue.");
            }
        }

        /// <summary>
        /// Restores a UnitStack state from a known remaining HP (useful for loading and battle state reconstruction).
        /// </summary>
        public static UnitStack Restore(UnitDefinition definition, int initialQuantity, int totalRemainingHp)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition), "Unit definition cannot be null.");
            }
            if (initialQuantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialQuantity), "Initial quantity must be greater than 0.");
            }

            int maximumTotalHp;
            try
            {
                maximumTotalHp = checked(initialQuantity * definition.MaxHpPerMember);
            }
            catch (OverflowException)
            {
                throw new ArgumentOutOfRangeException(nameof(initialQuantity), "The initial maximum total HP would overflow int.MaxValue.");
            }

            if (totalRemainingHp < 0 || totalRemainingHp > maximumTotalHp)
            {
                throw new ArgumentOutOfRangeException(nameof(totalRemainingHp), $"Total remaining HP must be between 0 and maximum total HP ({maximumTotalHp}).");
            }

            return new UnitStack(definition, initialQuantity, totalRemainingHp);
        }
    }
}
