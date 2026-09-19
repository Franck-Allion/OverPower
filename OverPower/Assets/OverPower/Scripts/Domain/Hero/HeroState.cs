using System;

namespace OverPower.Domain.Hero
{
    /// <summary>
    /// Authoritative Domain runtime state for hero resources (HP, Mana, Gold, XP).
    /// </summary>
    public sealed class HeroState
    {
        public int MaxHp { get; private set; }
        public int CurrentHp { get; private set; }

        public int MaxMana { get; private set; }
        public int CurrentMana { get; private set; }

        public int Gold { get; private set; }
        public int Experience { get; private set; }

        /// <summary>
        /// Gets whether the hero has reached zero HP.
        /// </summary>
        public bool IsDead => CurrentHp == 0;

        /// <summary>
        /// Creates a new HeroState with explicit initial values.
        /// </summary>
        public HeroState(int maxHp, int currentHp, int maxMana, int currentMana, int gold, int experience)
        {
            if (maxHp <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxHp), "Maximum HP must be greater than zero.");
            }
            if (currentHp < 0 || currentHp > maxHp)
            {
                throw new ArgumentOutOfRangeException(nameof(currentHp), "Current HP must be greater than or equal to zero, and less than or equal to maximum HP.");
            }
            if (maxMana < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxMana), "Maximum Mana cannot be negative.");
            }
            if (currentMana < 0 || currentMana > maxMana)
            {
                throw new ArgumentOutOfRangeException(nameof(currentMana), "Current Mana must be greater than or equal to zero, and less than or equal to maximum Mana.");
            }
            if (gold < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(gold), "Gold cannot be negative.");
            }
            if (experience < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(experience), "Experience cannot be negative.");
            }

            MaxHp = maxHp;
            CurrentHp = currentHp;
            MaxMana = maxMana;
            CurrentMana = currentMana;
            Gold = gold;
            Experience = experience;
        }

        /// <summary>
        /// Inflicts direct resolved damage on the hero's HP pool.
        /// Returns the actual damage applied.
        /// </summary>
        public int TakeDamage(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Damage amount cannot be negative.");
            }
            if (amount == 0)
            {
                return 0;
            }

            int appliedDamage = Math.Min(amount, CurrentHp);
            CurrentHp -= appliedDamage;
            return appliedDamage;
        }

        /// <summary>
        /// Restores HP to the hero up to the maximum HP pool limit.
        /// Returns the actual HP healed.
        /// </summary>
        public int Heal(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Heal amount cannot be negative.");
            }
            if (amount == 0)
            {
                return 0;
            }

            int maxPossibleHealing = MaxHp - CurrentHp;
            int actualHealed = Math.Min(amount, maxPossibleHealing);
            CurrentHp += actualHealed;
            return actualHealed;
        }

        /// <summary>
        /// Increases the maximum HP pool, immediately restoring the same amount to current HP.
        /// </summary>
        public void IncreaseMaxHp(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount to increase maximum HP must be greater than zero.");
            }

            checked
            {
                MaxHp += amount;
                CurrentHp += amount;
            }
        }

        /// <summary>
        /// Attempts to spend the requested mana amount.
        /// Returns false and leaves state completely unchanged on failure.
        /// </summary>
        public bool TrySpendMana(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Mana spend amount cannot be negative.");
            }
            if (amount == 0)
            {
                return true;
            }

            if (amount <= CurrentMana)
            {
                CurrentMana -= amount;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Restores Mana to the hero up to the maximum Mana limit.
        /// Returns the actual Mana restored.
        /// </summary>
        public int RestoreMana(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Mana restore amount cannot be negative.");
            }
            if (amount == 0)
            {
                return 0;
            }

            int maxPossibleRestore = MaxMana - CurrentMana;
            int actualRestored = Math.Min(amount, maxPossibleRestore);
            CurrentMana += actualRestored;
            return actualRestored;
        }

        /// <summary>
        /// Increases the maximum Mana pool, immediately restoring the same amount to current Mana.
        /// </summary>
        public void IncreaseMaxMana(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount to increase maximum Mana must be greater than zero.");
            }

            checked
            {
                MaxMana += amount;
                CurrentMana += amount;
            }
        }

        /// <summary>
        /// Increases the hero's gold count.
        /// </summary>
        public void GainGold(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Gold gain amount cannot be negative.");
            }
            if (amount == 0)
            {
                return;
            }

            checked
            {
                Gold += amount;
            }
        }

        /// <summary>
        /// Attempts to spend the requested gold amount.
        /// Returns false and leaves state completely unchanged on failure.
        /// </summary>
        public bool TrySpendGold(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Gold spend amount cannot be negative.");
            }
            if (amount == 0)
            {
                return true;
            }

            if (amount <= Gold)
            {
                Gold -= amount;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Accumulates experience points safely.
        /// Level-up progression is deferred to subsequent milestones.
        /// </summary>
        public void GainExperience(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Experience gain amount cannot be negative.");
            }
            if (amount == 0)
            {
                return;
            }

            checked
            {
                Experience += amount;
            }
        }
    }
}
