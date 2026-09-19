using System;
using NUnit.Framework;
using OverPower.Domain.Hero;

namespace OverPower.Tests.Domain
{
    [TestFixture]
    public class HeroStateTests
    {
        #region Construction Tests

        [Test]
        public void Constructor_WithValidArguments_SucceedsAndPreservesValues()
        {
            var hero = new HeroState(
                maxHp: 100,
                currentHp: 80,
                maxMana: 5,
                currentMana: 3,
                gold: 20,
                experience: 10
            );

            Assert.That(hero.MaxHp, Is.EqualTo(100));
            Assert.That(hero.CurrentHp, Is.EqualTo(80));
            Assert.That(hero.MaxMana, Is.EqualTo(5));
            Assert.That(hero.CurrentMana, Is.EqualTo(3));
            Assert.That(hero.Gold, Is.EqualTo(20));
            Assert.That(hero.Experience, Is.EqualTo(10));
            Assert.That(hero.IsDead, Is.False);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Constructor_WithInvalidMaxHp_ThrowsArgumentOutOfRangeException(int maxHp)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new HeroState(maxHp, 10, 5, 3, 20, 0));
        }

        [TestCase(-1)]
        [TestCase(101)]
        public void Constructor_WithInvalidCurrentHp_ThrowsArgumentOutOfRangeException(int currentHp)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new HeroState(100, currentHp, 5, 3, 20, 0));
        }

        [Test]
        public void Constructor_WithNegativeMaxMana_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new HeroState(100, 80, -1, 0, 20, 0));
        }

        [TestCase(-1)]
        [TestCase(6)]
        public void Constructor_WithInvalidCurrentMana_ThrowsArgumentOutOfRangeException(int currentMana)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new HeroState(100, 80, 5, currentMana, 20, 0));
        }

        [Test]
        public void Constructor_WithNegativeGold_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new HeroState(100, 80, 5, 3, -1, 0));
        }

        [Test]
        public void Constructor_WithNegativeExperience_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new HeroState(100, 80, 5, 3, 20, -1));
        }

        [Test]
        public void Constructor_WithZeroMaxMana_IsSupportedAndValid()
        {
            var hero = new HeroState(100, 80, 0, 0, 20, 0);

            Assert.That(hero.MaxMana, Is.Zero);
            Assert.That(hero.CurrentMana, Is.Zero);
        }

        #endregion

        #region HP & Damage & Healing Tests

        [Test]
        public void TakeDamage_NormalDamage_ReducesHpAndReturnsAppliedDamage()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            int applied = hero.TakeDamage(30);

            Assert.That(applied, Is.EqualTo(30));
            Assert.That(hero.CurrentHp, Is.EqualTo(50));
            Assert.That(hero.IsDead, Is.False);
        }

        [Test]
        public void TakeDamage_ZeroDamage_IsNoOpAndReturnsZero()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            int applied = hero.TakeDamage(0);

            Assert.That(applied, Is.Zero);
            Assert.That(hero.CurrentHp, Is.EqualTo(80));
        }

        [Test]
        public void TakeDamage_NegativeDamage_ThrowsArgumentOutOfRangeException()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            Assert.Throws<ArgumentOutOfRangeException>(() => hero.TakeDamage(-1));
        }

        [Test]
        public void TakeDamage_LethalDamage_ClampsAtZeroAndFlagsIsDead()
        {
            var hero = new HeroState(100, 30, 5, 3, 20, 0);

            int applied = hero.TakeDamage(50);

            Assert.That(applied, Is.EqualTo(30)); // Clamped to actual remaining HP
            Assert.That(hero.CurrentHp, Is.Zero);
            Assert.That(hero.IsDead, Is.True);
        }

        [Test]
        public void TakeDamage_OverkillDamage_ClampsAtZeroAndFlagsIsDead()
        {
            var hero = new HeroState(100, 10, 5, 3, 20, 0);

            int applied = hero.TakeDamage(1000);

            Assert.That(applied, Is.EqualTo(10));
            Assert.That(hero.CurrentHp, Is.Zero);
            Assert.That(hero.IsDead, Is.True);
        }

        [Test]
        public void Heal_NormalHealing_IncreasesHpAndReturnsActualHealed()
        {
            var hero = new HeroState(100, 50, 5, 3, 20, 0);

            int healed = hero.Heal(30);

            Assert.That(healed, Is.EqualTo(30));
            Assert.That(hero.CurrentHp, Is.EqualTo(80));
        }

        [Test]
        public void Heal_Overhealing_CapsAtMaxHpAndReturnsActualHealed()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            int healed = hero.Heal(50);

            Assert.That(healed, Is.EqualTo(20)); // Capped at MaxHp - CurrentHp
            Assert.That(hero.CurrentHp, Is.EqualTo(100));
        }

        [Test]
        public void Heal_ZeroHealing_IsNoOpAndReturnsZero()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            int healed = hero.Heal(0);

            Assert.That(healed, Is.Zero);
            Assert.That(hero.CurrentHp, Is.EqualTo(80));
        }

        [Test]
        public void Heal_NegativeHealing_ThrowsArgumentOutOfRangeException()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            Assert.Throws<ArgumentOutOfRangeException>(() => hero.Heal(-1));
        }

        #endregion

        #region Max HP Upgrade Tests

        [Test]
        public void IncreaseMaxHp_WithValidAmount_IncreasesBothMaxAndCurrentHp()
        {
            var hero = new HeroState(100, 70, 5, 3, 20, 0);

            hero.IncreaseMaxHp(10);

            Assert.That(hero.MaxHp, Is.EqualTo(110));
            Assert.That(hero.CurrentHp, Is.EqualTo(80));
        }

        [TestCase(0)]
        [TestCase(-5)]
        public void IncreaseMaxHp_WithInvalidAmount_ThrowsArgumentOutOfRangeException(int amount)
        {
            var hero = new HeroState(100, 70, 5, 3, 20, 0);

            Assert.Throws<ArgumentOutOfRangeException>(() => hero.IncreaseMaxHp(amount));
        }

        [Test]
        public void IncreaseMaxHp_IntegerOverflow_ThrowsOverflowException()
        {
            var hero = new HeroState(int.MaxValue - 5, int.MaxValue - 10, 5, 3, 20, 0);

            Assert.Throws<OverflowException>(() => hero.IncreaseMaxHp(10));
        }

        #endregion

        #region Mana Spending & Restoration Tests

        [Test]
        public void TrySpendMana_WithValidAmount_SubtractsAndReturnsTrue()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            bool success = hero.TrySpendMana(2);

            Assert.That(success, Is.True);
            Assert.That(hero.CurrentMana, Is.EqualTo(1));
        }

        [Test]
        public void TrySpendMana_ExactBalance_SubtractsToZeroAndReturnsTrue()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            bool success = hero.TrySpendMana(3);

            Assert.That(success, Is.True);
            Assert.That(hero.CurrentMana, Is.Zero);
        }

        [Test]
        public void TrySpendMana_InsufficientMana_ReturnsFalseAndLeavesStateUnchanged()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            bool success = hero.TrySpendMana(4);

            Assert.That(success, Is.False);
            Assert.That(hero.CurrentMana, Is.EqualTo(3)); // Unchanged
        }

        [Test]
        public void TrySpendMana_ZeroSpend_IsSuccessAndNoOp()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            bool success = hero.TrySpendMana(0);

            Assert.That(success, Is.True);
            Assert.That(hero.CurrentMana, Is.EqualTo(3));
        }

        [Test]
        public void TrySpendMana_NegativeSpend_ThrowsArgumentOutOfRangeException()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            Assert.Throws<ArgumentOutOfRangeException>(() => hero.TrySpendMana(-1));
        }

        [Test]
        public void RestoreMana_NormalRestore_AddsManaAndReturnsActualRestored()
        {
            var hero = new HeroState(100, 80, 5, 2, 20, 0);

            int restored = hero.RestoreMana(2);

            Assert.That(restored, Is.EqualTo(2));
            Assert.That(hero.CurrentMana, Is.EqualTo(4));
        }

        [Test]
        public void RestoreMana_OverRestoring_CapsAtMaxManaAndReturnsActualRestored()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            int restored = hero.RestoreMana(10);

            Assert.That(restored, Is.EqualTo(2)); // Capped at MaxMana - CurrentMana
            Assert.That(hero.CurrentMana, Is.EqualTo(5));
        }

        [Test]
        public void RestoreMana_NegativeRestore_ThrowsArgumentOutOfRangeException()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            Assert.Throws<ArgumentOutOfRangeException>(() => hero.RestoreMana(-1));
        }

        #endregion

        #region Max Mana Upgrade Tests

        [Test]
        public void IncreaseMaxMana_WithValidAmount_IncreasesBothMaxAndCurrentMana()
        {
            var hero = new HeroState(100, 80, 5, 2, 20, 0);

            hero.IncreaseMaxMana(3);

            Assert.That(hero.MaxMana, Is.EqualTo(8));
            Assert.That(hero.CurrentMana, Is.EqualTo(5));
        }

        [TestCase(0)]
        [TestCase(-2)]
        public void IncreaseMaxMana_WithInvalidAmount_ThrowsArgumentOutOfRangeException(int amount)
        {
            var hero = new HeroState(100, 80, 5, 2, 20, 0);

            Assert.Throws<ArgumentOutOfRangeException>(() => hero.IncreaseMaxMana(amount));
        }

        [Test]
        public void IncreaseMaxMana_IntegerOverflow_ThrowsOverflowException()
        {
            var hero = new HeroState(100, 80, int.MaxValue - 2, int.MaxValue - 4, 20, 0);

            Assert.Throws<OverflowException>(() => hero.IncreaseMaxMana(5));
        }

        #endregion

        #region Gold Tests

        [Test]
        public void GainGold_WithValidAmount_AddsToGold()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            hero.GainGold(15);

            Assert.That(hero.Gold, Is.EqualTo(35));
        }

        [Test]
        public void GainGold_ZeroAmount_IsNoOp()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            hero.GainGold(0);

            Assert.That(hero.Gold, Is.EqualTo(20));
        }

        [Test]
        public void GainGold_NegativeAmount_ThrowsArgumentOutOfRangeException()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            Assert.Throws<ArgumentOutOfRangeException>(() => hero.GainGold(-1));
        }

        [Test]
        public void GainGold_IntegerOverflow_ThrowsOverflowException()
        {
            var hero = new HeroState(100, 80, 5, 3, int.MaxValue - 5, 0);

            Assert.Throws<OverflowException>(() => hero.GainGold(10));
        }

        [Test]
        public void TrySpendGold_WithValidAmount_SubtractsAndReturnsTrue()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            bool success = hero.TrySpendGold(15);

            Assert.That(success, Is.True);
            Assert.That(hero.Gold, Is.EqualTo(5));
        }

        [Test]
        public void TrySpendGold_ExactBalance_SubtractsToZeroAndReturnsTrue()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            bool success = hero.TrySpendGold(20);

            Assert.That(success, Is.True);
            Assert.That(hero.Gold, Is.Zero);
        }

        [Test]
        public void TrySpendGold_InsufficientGold_ReturnsFalseAndLeavesStateUnchanged()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            bool success = hero.TrySpendGold(25);

            Assert.That(success, Is.False);
            Assert.That(hero.Gold, Is.EqualTo(20)); // Completely unchanged
        }

        [Test]
        public void TrySpendGold_NegativeAmount_ThrowsArgumentOutOfRangeException()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            Assert.Throws<ArgumentOutOfRangeException>(() => hero.TrySpendGold(-1));
        }

        [Test]
        public void TrySpendGold_ZeroSpend_ReturnsTrueAndIsNoOp()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 0);

            bool success = hero.TrySpendGold(0);

            Assert.That(success, Is.True);
            Assert.That(hero.Gold, Is.EqualTo(20));
        }

        #endregion

        #region XP Tests

        [Test]
        public void GainExperience_WithValidAmount_AddsToExperience()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 10);

            hero.GainExperience(15);

            Assert.That(hero.Experience, Is.EqualTo(25));
        }

        [Test]
        public void GainExperience_ZeroAmount_IsNoOp()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 10);

            hero.GainExperience(0);

            Assert.That(hero.Experience, Is.EqualTo(10));
        }

        [Test]
        public void GainExperience_NegativeAmount_ThrowsArgumentOutOfRangeException()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, 10);

            Assert.Throws<ArgumentOutOfRangeException>(() => hero.GainExperience(-1));
        }

        [Test]
        public void GainExperience_IntegerOverflow_ThrowsOverflowException()
        {
            var hero = new HeroState(100, 80, 5, 3, 20, int.MaxValue - 5);

            Assert.Throws<OverflowException>(() => hero.GainExperience(10));
        }

        #endregion
    }
}
