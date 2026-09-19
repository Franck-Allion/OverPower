using System;
using System.Collections.Generic;
using OverPower.Domain.Content;
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

        public int Armor => Definition.Armor;
        public int Attack => Definition.Attack;
        public IReadOnlyList<ContentId> AbilityIds => Definition.AbilityIds;

        public bool IsEmpty => TotalRemainingHp == 0;

        public int TemporaryBonusQuantity => Math.Max(0, DisplayedQuantity - InitialQuantity);

        public int DisplayedQuantity
        {
            get
            {
                if (TotalRemainingHp == 0) return 0;
                int full = TotalRemainingHp / MaxHpPerMember;
                int remainder = TotalRemainingHp % MaxHpPerMember;
                return full + (remainder > 0 ? 1 : 0);
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

        /// <summary>
        /// Applies an already-resolved amount of HP damage to the stack.
        /// </summary>
        public void ApplyDamage(int damage)
        {
            if (damage < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damage), "Damage cannot be negative.");
            }

            if (damage == 0)
            {
                return;
            }

            if (damage >= TotalRemainingHp)
            {
                TotalRemainingHp = 0;
            }
            else
            {
                TotalRemainingHp -= damage;
            }
        }

        /// <summary>
        /// Applies healing to the stack according to the specified healing mode capabilities.
        /// </summary>
        public void Heal(int amount, HealingMode mode)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Healing amount cannot be negative.");
            }

            if (amount == 0)
            {
                return;
            }

            switch (mode)
            {
                case HealingMode.SurvivorOnly:
                    int survivorCap = DisplayedQuantity * MaxHpPerMember;
                    if (survivorCap == 0)
                    {
                        return; // Empty stack cannot receive survivor-only healing
                    }
                    try
                    {
                        int newHp = checked(TotalRemainingHp + amount);
                        TotalRemainingHp = Math.Min(survivorCap, newHp);
                    }
                    catch (OverflowException)
                    {
                        TotalRemainingHp = survivorCap;
                    }
                    break;

                case HealingMode.ReviveToInitial:
                    try
                    {
                        int newHp = checked(TotalRemainingHp + amount);
                        TotalRemainingHp = Math.Min(MaximumTotalHp, newHp);
                    }
                    catch (OverflowException)
                    {
                        TotalRemainingHp = MaximumTotalHp;
                    }
                    break;

                case HealingMode.TemporaryOverflow:
                    try
                    {
                        TotalRemainingHp = checked(TotalRemainingHp + amount);
                    }
                    catch (OverflowException)
                    {
                        TotalRemainingHp = int.MaxValue;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), "Unsupported healing mode.");
            }
        }

        /// <summary>
        /// Normalizes the stack at end of combat, removing any temporary extra members.
        /// </summary>
        public void RemoveTemporaryOverflow()
        {
            if (TotalRemainingHp > MaximumTotalHp)
            {
                TotalRemainingHp = MaximumTotalHp;
            }
        }
    }
}
