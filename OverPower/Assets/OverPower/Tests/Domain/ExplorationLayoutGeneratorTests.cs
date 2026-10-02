using System;
using System.Collections.Generic;
using NUnit.Framework;
using OverPower.Domain.Exploration;
using OverPower.Domain.Random;

namespace OverPower.Tests.Domain
{
    [TestFixture]
    public sealed class ExplorationLayoutGeneratorTests
    {
        private sealed class TestRandomService : IRandomService
        {
            private readonly System.Random _random;

            public TestRandomService(int seed)
            {
                _random = new System.Random(seed);
            }

            public int NextInt(int maxExclusive)
            {
                if (maxExclusive <= 0) throw new ArgumentOutOfRangeException(nameof(maxExclusive));
                return _random.Next(maxExclusive);
            }

            public int NextInt(int minInclusive, int maxExclusive)
            {
                if (maxExclusive <= minInclusive) throw new ArgumentOutOfRangeException(nameof(maxExclusive));
                return _random.Next(minInclusive, maxExclusive);
            }

            public void Shuffle<T>(IList<T> items)
            {
                if (items == null) throw new ArgumentNullException(nameof(items));
                int n = items.Count;
                while (n > 1)
                {
                    n--;
                    int k = NextInt(n + 1);
                    T value = items[k];
                    items[k] = items[n];
                    items[n] = value;
                }
            }

            public IRandomService CreateSubstream(string streamId)
            {
                if (string.IsNullOrWhiteSpace(streamId)) throw new ArgumentException("StreamId cannot be null or whitespace.", nameof(streamId));
                uint hash = 2166136261;
                foreach (char c in streamId)
                {
                    hash = unchecked((hash ^ c) * 16777619);
                }
                return new TestRandomService((int)hash);
            }
        }

        [Test]
        public void Configuration_Construction_ValidatesAllBounds()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ExplorationGenerationConfiguration(0, 5, 10, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ExplorationGenerationConfiguration(5, 0, 10, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ExplorationGenerationConfiguration(5, 5, -1, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ExplorationGenerationConfiguration(5, 5, 10, -1));

            // Max allowed blocked in 2x2 grid (4 cells) is 4 - 2 = 2. Blocked 3 must be rejected.
            Assert.Throws<ArgumentException>(() => new ExplorationGenerationConfiguration(2, 2, 5, 3));

            // 1x1 grid (1 cell): max allowed blocked is 0. Blocked 1 must be rejected.
            Assert.Throws<ArgumentException>(() => new ExplorationGenerationConfiguration(1, 1, 5, 1));

            // Valid boundary configurations
            Assert.DoesNotThrow(() => new ExplorationGenerationConfiguration(2, 2, 5, 2));
            Assert.DoesNotThrow(() => new ExplorationGenerationConfiguration(1, 1, 5, 0));
        }

        [Test]
        public void Generate_NullArguments_ThrowsArgumentNullException()
        {
            var config = new ExplorationGenerationConfiguration(6, 4, 10, 4);
            var random = new TestRandomService(42);

            Assert.Throws<ArgumentNullException>(() => ExplorationLayoutGenerator.Generate(null!, random));
            Assert.Throws<ArgumentNullException>(() => ExplorationLayoutGenerator.Generate(config, null!));
        }

        [Test]
        public void Generate_StandardConfiguration_SatisfiesAllInvariants()
        {
            var config = new ExplorationGenerationConfiguration(8, 6, 12, 8);
            var random = new TestRandomService(12345);

            var layout = ExplorationLayoutGenerator.Generate(config, random);

            Assert.That(layout.Grid.Width, Is.EqualTo(8));
            Assert.That(layout.Grid.Height, Is.EqualTo(6));
            Assert.That(layout.Grid.TotalCellCount, Is.EqualTo(48));
            Assert.That(layout.InitialActionPoints, Is.EqualTo(12));

            // Starting position is valid and walkable
            Assert.That(layout.Grid.Contains(layout.StartingPosition), Is.True);
            Assert.That(layout.Grid.IsWalkable(layout.StartingPosition), Is.True);

            // Blocked count is exact
            int blockedCount = 0;
            foreach (var cell in layout.Grid.Cells)
            {
                if (!cell.IsWalkable) blockedCount++;
            }
            Assert.That(blockedCount, Is.EqualTo(8));

            // At least one orthogonal neighbor is walkable
            int walkableNeighbors = 0;
            int sx = layout.StartingPosition.X;
            int sy = layout.StartingPosition.Y;
            var candidates = new[]
            {
                new ExplorationCoordinate(sx, sy + 1),
                new ExplorationCoordinate(sx, sy - 1),
                new ExplorationCoordinate(sx - 1, sy),
                new ExplorationCoordinate(sx + 1, sy)
            };

            foreach (var neighbor in candidates)
            {
                if (layout.Grid.Contains(neighbor) && layout.Grid.IsWalkable(neighbor))
                {
                    walkableNeighbors++;
                }
            }
            Assert.That(walkableNeighbors, Is.GreaterThanOrEqualTo(1), "Player must have at least one legal starting move.");

            // CreateState succeeds and matches layout
            var state = layout.CreateState();
            Assert.That(state.PlayerPosition, Is.EqualTo(layout.StartingPosition));
            Assert.That(state.RemainingActionPoints, Is.EqualTo(12));
            Assert.That(state.IsCompleted, Is.False);
        }

        [Test]
        public void Generate_1x1Grid_HandlesZeroNeighborEdgeCase()
        {
            var config = new ExplorationGenerationConfiguration(1, 1, 3, 0);
            var random = new TestRandomService(999);

            var layout = ExplorationLayoutGenerator.Generate(config, random);

            Assert.That(layout.Grid.TotalCellCount, Is.EqualTo(1));
            Assert.That(layout.StartingPosition, Is.EqualTo(new ExplorationCoordinate(0, 0)));
            Assert.That(layout.Grid.IsWalkable(0, 0), Is.True);
        }
    }
}
