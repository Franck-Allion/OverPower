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
    }
}
