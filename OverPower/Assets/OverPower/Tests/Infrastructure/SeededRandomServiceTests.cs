using NUnit.Framework;
using System;
using System.Collections.Generic;
using OverPower.Domain.Random;
using OverPower.Infrastructure.Random;

namespace OverPower.Tests.Infrastructure
{
    [TestFixture]
    public class SeededRandomServiceTests
    {
        [Test]
        public void SeededRandomService_SameSeed_ProducesIdenticalSequence()
        {
            // Arrange
            ulong seed = 12345UL;
            var rng1 = new SeededRandomService(seed);
            var rng2 = new SeededRandomService(seed);

            // Act & Assert
            for (int i = 0; i < 100; i++)
            {
                int val1 = rng1.NextInt(100);
                int val2 = rng2.NextInt(100);
                Assert.That(val1, Is.EqualTo(val2), $"Sequence mismatch at index {i} for simple range.");

                int val3 = rng1.NextInt(-50, 50);
                int val4 = rng2.NextInt(-50, 50);
                Assert.That(val3, Is.EqualTo(val4), $"Sequence mismatch at index {i} for custom range.");
            }
        }

        [Test]
        public void SeededRandomService_DifferentSeeds_ProduceDivergentSequences()
        {
            // Arrange
            var rng1 = new SeededRandomService(11111UL);
            var rng2 = new SeededRandomService(22222UL);
            int matches = 0;
            int total = 100;

            // Act & Assert
            for (int i = 0; i < total; i++)
            {
                if (rng1.NextInt(100) == rng2.NextInt(100))
                {
                    matches++;
                }
            }

            // Two divergent sequences can occasionally have matching values, 
            // but the overlap probability must be very low (e.g. less than 15%).
            Assert.That(matches, Is.LessThan(total * 0.15), "Sequences from different seeds must be statistically divergent.");
        }

        [Test]
        public void SeededRandomService_RangeBoundaries_AreAlwaysRespected()
        {
            // Arrange
            var rng = new SeededRandomService(99999UL);

            // Act & Assert
            for (int i = 0; i < 1000; i++)
            {
                // Simple bound
                int val1 = rng.NextInt(10);
                Assert.That(val1, Is.GreaterThanOrEqualTo(0));
                Assert.That(val1, Is.LessThan(10));

                // Bounded range
                int val2 = rng.NextInt(-20, -10);
                Assert.That(val2, Is.GreaterThanOrEqualTo(-20));
                Assert.That(val2, Is.LessThan(-10));

                // Range size 1
                int val3 = rng.NextInt(5, 6);
                Assert.That(val3, Is.EqualTo(5));

                // Large range crossing zero
                int val4 = rng.NextInt(-10, 10);
                Assert.That(val4, Is.GreaterThanOrEqualTo(-10));
                Assert.That(val4, Is.LessThan(10));
            }
        }

        [Test]
        public void SeededRandomService_InvalidArguments_ThrowExpectedExceptions()
        {
            // Arrange
            var rng = new SeededRandomService(88888UL);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => rng.NextInt(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => rng.NextInt(-10));
            Assert.Throws<ArgumentOutOfRangeException>(() => rng.NextInt(10, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => rng.NextInt(10, 5));
            Assert.Throws<ArgumentNullException>(() => rng.Shuffle<int>(null!));
        }

        [Test]
        public void SeededRandomService_Shuffle_IsDeterministicAndMaintainsElements()
        {
            // Arrange
            ulong seed = 77777UL;
            var list1 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var list2 = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var rng1 = new SeededRandomService(seed);
            var rng2 = new SeededRandomService(seed);

            // Act
            rng1.Shuffle(list1);
            rng2.Shuffle(list2);

            // Assert - Same seed produces identical shuffle
            Assert.That(list1, Is.EqualTo(list2), "Same seed must produce identical shuffled collections.");

            // Assert - No elements lost or duplicated
            var sortedList = new List<int>(list1);
            sortedList.Sort();
            Assert.That(sortedList, Is.EqualTo(new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }), "Shuffle must not lose or duplicate any original elements.");

            // Empty and single-element collections are valid
            var emptyList = new List<int>();
            var singleList = new List<int> { 42 };

            Assert.DoesNotThrow(() => rng1.Shuffle(emptyList));
            Assert.DoesNotThrow(() => rng1.Shuffle(singleList));
            Assert.That(singleList[0], Is.EqualTo(42));
        }

        [Test]
        public void SeededRandomService_KnownSeed_StabilityVector()
        {
            // Arrange
            var rng = new SeededRandomService(42UL);

            // Act & Assert
            // Seed 42 has a locked stability vector for PCG32 implementation:
            // First 5 elements of NextInt(100): 26, 9, 35, 55, 57
            Assert.That(rng.NextInt(100), Is.EqualTo(26));
            Assert.That(rng.NextInt(100), Is.EqualTo(9));
            Assert.That(rng.NextInt(100), Is.EqualTo(35));
            Assert.That(rng.NextInt(100), Is.EqualTo(55));
            Assert.That(rng.NextInt(100), Is.EqualTo(57));

            // First 5 elements of NextInt(-50, 50): -4, -50, 21, -46, 43
            Assert.That(rng.NextInt(-50, 50), Is.EqualTo(-4));
            Assert.That(rng.NextInt(-50, 50), Is.EqualTo(-50));
            Assert.That(rng.NextInt(-50, 50), Is.EqualTo(21));
            Assert.That(rng.NextInt(-50, 50), Is.EqualTo(-46));
            Assert.That(rng.NextInt(-50, 50), Is.EqualTo(43));
        }

        [Test]
        public void SeededRandomService_ExtremeIntRange_DoesNotOverflowOrThrow()
        {
            // Arrange
            var rng = new SeededRandomService(42UL);

            // Act & Assert
            for (int i = 0; i < 1000; i++)
            {
                int value = rng.NextInt(int.MinValue, int.MaxValue);

                Assert.That(value, Is.GreaterThanOrEqualTo(int.MinValue));
                Assert.That(value, Is.LessThan(int.MaxValue));
            }
        }

        [Test]
        public void SeededRandomService_SameSeedAndStream_ProducesIdenticalSequence()
        {
            // Arrange
            ulong seed = 98765UL;
            string streamId = "run.battle";
            var rng1 = new SeededRandomService(seed);
            var rng2 = new SeededRandomService(seed);

            // Act
            IRandomService sub1 = rng1.CreateSubstream(streamId);
            IRandomService sub2 = rng2.CreateSubstream(streamId);

            // Assert
            for (int i = 0; i < 100; i++)
            {
                Assert.That(sub1.NextInt(100), Is.EqualTo(sub2.NextInt(100)));
            }
        }

        [Test]
        public void SeededRandomService_SameSeedAndDifferentStreams_ProduceDivergentSequences()
        {
            // Arrange
            ulong seed = 98765UL;
            var rng = new SeededRandomService(seed);
            IRandomService sub1 = rng.CreateSubstream("run.battle");
            IRandomService sub2 = rng.CreateSubstream("run.exploration");
            int matches = 0;
            int total = 100;

            // Act & Assert
            for (int i = 0; i < total; i++)
            {
                if (sub1.NextInt(100) == sub2.NextInt(100))
                {
                    matches++;
                }
            }

            Assert.That(matches, Is.LessThan(total * 0.15), "Different streams must produce statistically divergent sequences.");
        }

        [Test]
        public void SeededRandomService_ParentConsumption_IsIsolated()
        {
            // Arrange
            ulong seed = 1234567UL;
            string streamId = "run.battle";
            var parentA = new SeededRandomService(seed);
            var parentB = new SeededRandomService(seed);

            // Act - Consume many values from parent A
            for (int i = 0; i < 500; i++)
            {
                parentA.NextInt(100);
            }

            // Deriving substream "run.battle" from both parents after consumption
            IRandomService streamA = parentA.CreateSubstream(streamId);
            IRandomService streamB = parentB.CreateSubstream(streamId);

            // Assert - The substream sequences remain identical
            for (int i = 0; i < 100; i++)
            {
                Assert.That(streamA.NextInt(100), Is.EqualTo(streamB.NextInt(100)), "Parent state consumption must not affect substream generation.");
            }
        }

        [Test]
        public void SeededRandomService_CrossSystem_IsIsolated()
        {
            // Arrange
            ulong seed = 42UL;
            var parent = new SeededRandomService(seed);
            IRandomService exploration = parent.CreateSubstream("run.exploration");
            IRandomService battle = parent.CreateSubstream("run.battle");

            var expectedBattleSequence = new List<int>();
            // Save initial Battle sequence
            for (int i = 0; i < 100; i++)
            {
                expectedBattleSequence.Add(battle.NextInt(100));
            }

            // Recreate identical system
            var parentNew = new SeededRandomService(seed);
            IRandomService explorationNew = parentNew.CreateSubstream("run.exploration");
            IRandomService battleNew = parentNew.CreateSubstream("run.battle");

            // Consume 100 values from Exploration
            for (int i = 0; i < 100; i++)
            {
                explorationNew.NextInt(100);
            }

            // Assert - Battle's sequence remains completely unaffected
            for (int i = 0; i < 100; i++)
            {
                Assert.That(battleNew.NextInt(100), Is.EqualTo(expectedBattleSequence[i]), "Exploration usage must not perturb Battle sequence.");
            }
        }

        [Test]
        public void SeededRandomService_Substream_HasKnownStabilityVector()
        {
            // Arrange
            var rng = new SeededRandomService(42UL);

            // Act
            IRandomService battleStream = rng.CreateSubstream("run.battle");

            // Assert - Seed 42 and stream "run.battle" has locked stability output: 92, 19, 2, 35, 56
            Assert.That(battleStream.NextInt(100), Is.EqualTo(92));
            Assert.That(battleStream.NextInt(100), Is.EqualTo(19));
            Assert.That(battleStream.NextInt(100), Is.EqualTo(2));
            Assert.That(battleStream.NextInt(100), Is.EqualTo(35));
            Assert.That(battleStream.NextInt(100), Is.EqualTo(56));
        }

        [Test]
        public void SeededRandomService_InvalidSubstreamIds_ThrowExceptions()
        {
            // Arrange
            var rng = new SeededRandomService(42UL);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => rng.CreateSubstream(null!));
            Assert.Throws<ArgumentException>(() => rng.CreateSubstream(""));
            Assert.Throws<ArgumentException>(() => rng.CreateSubstream("   "));
        }
    }
}
