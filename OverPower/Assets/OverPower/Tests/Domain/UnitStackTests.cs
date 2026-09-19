using NUnit.Framework;
using System;
using System.Collections.Generic;
using OverPower.Domain.Content;
using OverPower.Domain.Content.Units;
using OverPower.Domain.Combat.Units;
using OverPower.Domain.Localization;

namespace OverPower.Tests.Domain
{
    [TestFixture]
    public class UnitStackTests
    {
        private UnitDefinition _guardianDefinition;
        private ContentId _abilityId;

        [SetUp]
        public void SetUp()
        {
            var id = ContentId.Create("unit.guardian").Value;
            var nameKey = new LocalizationKey("unit.guardian.name");
            var descKey = new LocalizationKey("unit.guardian.desc");
            _abilityId = ContentId.Create("ability.guardian_shield").Value;
            var abilities = new List<ContentId> { _abilityId };
            _guardianDefinition = new UnitDefinition(id, nameKey, descKey, 10, 2, 5, abilities);
        }

        [Test]
        public void UnitStack_Constructor_SucceedsAndInitializesCorrectly()
        {
            // Act
            var stack = new UnitStack(_guardianDefinition, 10);

            // Assert
            Assert.That(stack.Definition, Is.SameAs(_guardianDefinition));
            Assert.That(stack.InitialQuantity, Is.EqualTo(10));
            Assert.That(stack.MaxHpPerMember, Is.EqualTo(10));
            Assert.That(stack.MaximumTotalHp, Is.EqualTo(100));
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(100));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(10));
            Assert.That(stack.PartialCurrentMemberHp, Is.EqualTo(10));
            Assert.That(stack.IsEmpty, Is.False);
            Assert.That(stack.TemporaryBonusQuantity, Is.EqualTo(0));
        }

        [Test]
        public void UnitStack_ExposesAuthoredCombatStatsAndAbilities()
        {
            // Arrange
            var stack = new UnitStack(_guardianDefinition, 5);

            // Assert
            Assert.That(stack.Armor, Is.EqualTo(2));
            Assert.That(stack.Attack, Is.EqualTo(5));
            Assert.That(stack.AbilityIds.Count, Is.EqualTo(1));
            Assert.That(stack.AbilityIds[0], Is.SameAs(_abilityId));
        }

        [Test]
        public void UnitStack_Constructor_NullDefinition_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UnitStack(null!, 5));
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-100)]
        public void UnitStack_Constructor_InvalidQuantity_ThrowsArgumentOutOfRangeException(int quantity)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new UnitStack(_guardianDefinition, quantity));
        }

        [Test]
        public void UnitStack_Restore_SucceedsAndPreservesDerivedState_73HpExample()
        {
            // Act
            var stack = UnitStack.Restore(_guardianDefinition, 10, 73);

            // Assert
            Assert.That(stack.Definition, Is.SameAs(_guardianDefinition));
            Assert.That(stack.InitialQuantity, Is.EqualTo(10));
            Assert.That(stack.MaxHpPerMember, Is.EqualTo(10));
            Assert.That(stack.MaximumTotalHp, Is.EqualTo(100));
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(73));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(8));
            Assert.That(stack.PartialCurrentMemberHp, Is.EqualTo(3));
            Assert.That(stack.IsEmpty, Is.False);
            Assert.That(stack.TemporaryBonusQuantity, Is.EqualTo(0));
        }

        [TestCase(100, 10, 10, false)]
        [TestCase(99, 10, 9, false)]
        [TestCase(91, 10, 1, false)]
        [TestCase(90, 9, 10, false)]
        [TestCase(73, 8, 3, false)]
        [TestCase(10, 1, 10, false)]
        [TestCase(1, 1, 1, false)]
        [TestCase(0, 0, 0, true)]
        public void UnitStack_BoundaryValues_DeriveCorrectQuantityAndPartialHp(int remainingHp, int expectedQuantity, int expectedPartialHp, bool expectedIsEmpty)
        {
            // Act
            var stack = UnitStack.Restore(_guardianDefinition, 10, remainingHp);

            // Assert
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(remainingHp));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(expectedQuantity));
            Assert.That(stack.PartialCurrentMemberHp, Is.EqualTo(expectedPartialHp));
            Assert.That(stack.IsEmpty, Is.EqualTo(expectedIsEmpty));
        }

        [Test]
        public void ApplyDamage_ZeroDamage_DoesNotChangeHp()
        {
            // Arrange
            var stack = new UnitStack(_guardianDefinition, 10);

            // Act
            stack.ApplyDamage(0);

            // Assert
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(100));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(10));
            Assert.That(stack.PartialCurrentMemberHp, Is.EqualTo(10));
        }

        [Test]
        public void ApplyDamage_PartialDamage_UpdatesHpAndDerivedStateCorrectly()
        {
            // Arrange
            var stack = new UnitStack(_guardianDefinition, 10); // 100 HP initial

            // Act
            stack.ApplyDamage(27);

            // Assert
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(73));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(8));
            Assert.That(stack.PartialCurrentMemberHp, Is.EqualTo(3));
            Assert.That(stack.IsEmpty, Is.False);
        }

        [Test]
        public void ApplyDamage_ConsecutiveDamage_DerivesCorrectState()
        {
            // Arrange
            var stack = UnitStack.Restore(_guardianDefinition, 10, 73);

            // Act
            stack.ApplyDamage(20);

            // Assert
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(53));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(6));
            Assert.That(stack.PartialCurrentMemberHp, Is.EqualTo(3));
            Assert.That(stack.IsEmpty, Is.False);
        }

        [Test]
        public void ApplyDamage_LethalDamage_SetsHpToZeroAndIsEmptyTrue()
        {
            // Arrange
            var stack = UnitStack.Restore(_guardianDefinition, 10, 10);

            // Act
            stack.ApplyDamage(10);

            // Assert
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(0));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(0));
            Assert.That(stack.PartialCurrentMemberHp, Is.EqualTo(0));
            Assert.That(stack.IsEmpty, Is.True);
        }

        [Test]
        public void ApplyDamage_OverkillDamage_ClampsToZeroAndIsEmptyTrue()
        {
            // Arrange
            var stack = UnitStack.Restore(_guardianDefinition, 10, 7);

            // Act
            stack.ApplyDamage(100);

            // Assert
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(0));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(0));
            Assert.That(stack.PartialCurrentMemberHp, Is.EqualTo(0));
            Assert.That(stack.IsEmpty, Is.True);
        }

        [Test]
        public void ApplyDamage_NegativeDamage_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var stack = new UnitStack(_guardianDefinition, 10);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => stack.ApplyDamage(-1));
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(100)); // Unchanged
        }

        [Test]
        public void ApplyDamage_OnZeroHp_RemainsZero()
        {
            // Arrange
            var stack = UnitStack.Restore(_guardianDefinition, 10, 0);

            // Act
            stack.ApplyDamage(10);

            // Assert
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(0));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(0));
            Assert.That(stack.PartialCurrentMemberHp, Is.EqualTo(0));
            Assert.That(stack.IsEmpty, Is.True);
        }

        [Test]
        public void ApplyDamage_VeryLargeDamage_ClampsToZeroAndDoesNotOverflow()
        {
            // Arrange
            var stack = new UnitStack(_guardianDefinition, 10); // 100 HP initial

            // Act
            stack.ApplyDamage(int.MaxValue);

            // Assert
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(0));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(0));
            Assert.That(stack.PartialCurrentMemberHp, Is.EqualTo(0));
            Assert.That(stack.IsEmpty, Is.True);
        }

        [Test]
        public void Heal_NegativeAmount_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var stack = new UnitStack(_guardianDefinition, 10);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => stack.Heal(-1, HealingMode.SurvivorOnly));
            Assert.Throws<ArgumentOutOfRangeException>(() => stack.Heal(-5, HealingMode.ReviveToInitial));
            Assert.Throws<ArgumentOutOfRangeException>(() => stack.Heal(-10, HealingMode.TemporaryOverflow));
        }

        [Test]
        public void Heal_ZeroAmount_DoesNotChangeHp()
        {
            // Arrange
            var stack = UnitStack.Restore(_guardianDefinition, 10, 73);

            // Act
            stack.Heal(0, HealingMode.SurvivorOnly);
            stack.Heal(0, HealingMode.ReviveToInitial);
            stack.Heal(0, HealingMode.TemporaryOverflow);

            // Assert
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(73));
        }

        [Test]
        public void Heal_SurvivorOnly_RestoresWithinCurrentSurvivors_73HpExample()
        {
            // Arrange
            // 73 HP out of 100 max has 8 displayed survivors (survivor capacity: 8 * 10 = 80 HP)
            var stack = UnitStack.Restore(_guardianDefinition, 10, 73);

            // Act & Assert
            stack.Heal(2, HealingMode.SurvivorOnly);
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(75));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(8));

            stack.Heal(5, HealingMode.SurvivorOnly); // Reaches survivorCap of 80 HP
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(80));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(8));

            stack.Heal(50, HealingMode.SurvivorOnly); // Cannot exceed survivorCap
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(80));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(8));
        }

        [Test]
        public void Heal_SurvivorOnly_OnEmptyStack_DoesNotHealOrResurrect()
        {
            // Arrange
            var stack = UnitStack.Restore(_guardianDefinition, 10, 0);

            // Act
            stack.Heal(50, HealingMode.SurvivorOnly);

            // Assert
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(0));
            Assert.That(stack.IsEmpty, Is.True);
        }

        [Test]
        public void Heal_ReviveToInitial_RestoresLostMembersButCapsAtInitialMax_73HpExample()
        {
            // Arrange
            var stack = UnitStack.Restore(_guardianDefinition, 10, 73);

            // Act & Assert
            stack.Heal(7, HealingMode.ReviveToInitial);
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(80));

            stack.Heal(10, HealingMode.ReviveToInitial); // Exceeds survivorCap, resurrecting 9th member
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(90));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(9));

            stack.Heal(10, HealingMode.ReviveToInitial); // Recreates 10th member
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(100));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(10));

            stack.Heal(50, HealingMode.ReviveToInitial); // Cannot exceed initial capacity (100 HP)
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(100));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(10));
            Assert.That(stack.TemporaryBonusQuantity, Is.EqualTo(0));
        }

        [Test]
        public void Heal_ReviveToInitial_OnEmptyStack_SuccessfullyResurrectsAndHeals()
        {
            // Arrange
            var stack = UnitStack.Restore(_guardianDefinition, 10, 0);

            // Act
            stack.Heal(15, HealingMode.ReviveToInitial);

            // Assert
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(15));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(2));
            Assert.That(stack.PartialCurrentMemberHp, Is.EqualTo(5));
            Assert.That(stack.IsEmpty, Is.False);
        }

        [Test]
        public void Heal_TemporaryOverflow_ExceedsInitialRoster_73HpAnd100HpExamples()
        {
            // Arrange
            var stack = UnitStack.Restore(_guardianDefinition, 10, 73);

            // Act & Assert
            stack.Heal(27, HealingMode.TemporaryOverflow); // Reaches 100 HP
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(100));
            Assert.That(stack.TemporaryBonusQuantity, Is.EqualTo(0));

            stack.Heal(5, HealingMode.TemporaryOverflow); // Exceeds initial cap, 105 HP, 11 members
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(105));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(11));
            Assert.That(stack.PartialCurrentMemberHp, Is.EqualTo(5));
            Assert.That(stack.TemporaryBonusQuantity, Is.EqualTo(1));

            // From 100 HP
            var fullStack = new UnitStack(_guardianDefinition, 10);
            fullStack.Heal(25, HealingMode.TemporaryOverflow);
            Assert.That(fullStack.TotalRemainingHp, Is.EqualTo(125));
            Assert.That(fullStack.DisplayedQuantity, Is.EqualTo(13));
            Assert.That(fullStack.PartialCurrentMemberHp, Is.EqualTo(5));
            Assert.That(fullStack.TemporaryBonusQuantity, Is.EqualTo(3));
        }

        [Test]
        public void Heal_TemporaryOverflow_OnEmptyStack_Succeeds()
        {
            // Arrange
            var stack = UnitStack.Restore(_guardianDefinition, 10, 0);

            // Act
            stack.Heal(115, HealingMode.TemporaryOverflow);

            // Assert
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(115));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(12));
            Assert.That(stack.PartialCurrentMemberHp, Is.EqualTo(5));
            Assert.That(stack.TemporaryBonusQuantity, Is.EqualTo(2));
        }

        [Test]
        public void Damage_AndRemoveTemporaryOverflow_InteractionsCorrectlyPreserveCombatState()
        {
            // Arrange
            var stack = new UnitStack(_guardianDefinition, 10); // 100 HP

            // Act - Overflow to 125 HP (13 members)
            stack.Heal(25, HealingMode.TemporaryOverflow);
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(125));
            Assert.That(stack.TemporaryBonusQuantity, Is.EqualTo(3));

            // Apply Damage of 40 HP
            stack.ApplyDamage(40);
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(85));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(9));
            Assert.That(stack.TemporaryBonusQuantity, Is.EqualTo(0));

            // End Combat (Remove temporary overflow)
            stack.RemoveTemporaryOverflow();
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(85)); // No effect on stacks below initial cap
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(9));
        }

        [Test]
        public void RemoveTemporaryOverflow_ClampsExcessHpOnly_125HpExample()
        {
            // Arrange
            var stack = new UnitStack(_guardianDefinition, 10); // 100 HP
            stack.Heal(25, HealingMode.TemporaryOverflow); // 125 HP, 13 members

            // Act
            stack.RemoveTemporaryOverflow();

            // Assert
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(100));
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(10));
            Assert.That(stack.TemporaryBonusQuantity, Is.EqualTo(0));
        }

        [Test]
        public void Heal_TemporaryOverflow_HugeValues_SaturatesSafelyToIntMax()
        {
            // Arrange
            var stack = new UnitStack(_guardianDefinition, 10);

            // Act
            stack.Heal(int.MaxValue, HealingMode.TemporaryOverflow);

            // Assert
            Assert.That(stack.TotalRemainingHp, Is.EqualTo(int.MaxValue));
            Assert.That(stack.DisplayedQuantity, Is.GreaterThan(200000000));
        }

        [Test]
        public void DisplayedQuantity_IsOverflowSafeWhenHpIsIntMax()
        {
            // Arrange
            var id = ContentId.Create("unit.one_hp").Value;
            var oneHpDefinition = new UnitDefinition(id, new LocalizationKey("n"), new LocalizationKey("d"), 1, 0, 0, new List<ContentId>());
            var stack = UnitStack.Restore(oneHpDefinition, int.MaxValue, int.MaxValue);

            // Act & Assert
            Assert.DoesNotThrow(() => { int q = stack.DisplayedQuantity; });
            Assert.That(stack.DisplayedQuantity, Is.EqualTo(int.MaxValue));
        }

        [Test]
        public void UnitStack_Restore_NullDefinition_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => UnitStack.Restore(null!, 10, 50));
        }

        [TestCase(0)]
        [TestCase(-5)]
        public void UnitStack_Restore_InvalidQuantity_ThrowsArgumentOutOfRangeException(int quantity)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => UnitStack.Restore(_guardianDefinition, quantity, 50));
        }

        [TestCase(-1)]
        [TestCase(101)]
        [TestCase(150)]
        public void UnitStack_Restore_InvalidHp_ThrowsArgumentOutOfRangeException(int hp)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => UnitStack.Restore(_guardianDefinition, 10, hp));
        }

        [Test]
        public void UnitStack_OverflowChecked_ThrowsWhenMaximumTotalHpExceedsIntMax()
        {
            // Arrange
            var hugeHpId = ContentId.Create("unit.huge_hp").Value;
            var hugeHpDefinition = new UnitDefinition(
                hugeHpId, 
                new LocalizationKey("n"), 
                new LocalizationKey("d"), 
                1_000_000, 0, 0, 
                new List<ContentId>());

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new UnitStack(hugeHpDefinition, 3000));
            Assert.Throws<ArgumentOutOfRangeException>(() => UnitStack.Restore(hugeHpDefinition, 3000, 100));
        }
    }
}
