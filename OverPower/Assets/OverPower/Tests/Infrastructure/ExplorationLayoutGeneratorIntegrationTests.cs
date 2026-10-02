using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OverPower.Domain.Exploration;
using OverPower.Infrastructure.Random;

namespace OverPower.Tests.Infrastructure
{
    [TestFixture]
    public sealed class ExplorationLayoutGeneratorIntegrationTests
    {
        [Test]
        public void Generate_WithProductionRNG_IsDeterministicUnderSeed()
        {
            var config = new ExplorationGenerationConfiguration(8, 6, 10, 8);

            var rng1 = new SeededRandomService(42UL);
            var layout1 = ExplorationLayoutGenerator.Generate(config, rng1);

            var rng2 = new SeededRandomService(42UL);
            var layout2 = ExplorationLayoutGenerator.Generate(config, rng2);

            Assert.That(layout1.StartingPosition, Is.EqualTo(layout2.StartingPosition));
            Assert.That(layout1.InitialActionPoints, Is.EqualTo(layout2.InitialActionPoints));
            Assert.That(layout1.Grid.TotalCellCount, Is.EqualTo(layout2.Grid.TotalCellCount));

            foreach (var cell1 in layout1.Grid.Cells)
            {
                Assert.That(layout2.Grid.TryGetCell(cell1.Coordinate, out var cell2), Is.True);
                Assert.That(cell1.IsWalkable, Is.EqualTo(cell2.IsWalkable));
            }
        }

        [Test]
        public void Generate_WithDifferentSeeds_ProducesDifferentLayouts()
        {
            var config = new ExplorationGenerationConfiguration(8, 6, 10, 8);

            var rng1 = new SeededRandomService(42UL);
            var layout1 = ExplorationLayoutGenerator.Generate(config, rng1);

            var rng2 = new SeededRandomService(999UL);
            var layout2 = ExplorationLayoutGenerator.Generate(config, rng2);

            var blocked1 = layout1.Grid.Cells.Where(c => !c.IsWalkable).Select(c => c.Coordinate).ToHashSet();
            var blocked2 = layout2.Grid.Cells.Where(c => !c.IsWalkable).Select(c => c.Coordinate).ToHashSet();

            bool isDifferent = layout1.StartingPosition != layout2.StartingPosition || !blocked1.SetEquals(blocked2);
            Assert.That(isDifferent, Is.True, "Different seeds should produce different layouts.");
        }

        [Test]
        public void Generate_SubstreamIsolation_DoesNotDependOnParentRngState()
        {
            var config = new ExplorationGenerationConfiguration(8, 6, 10, 8);

            var parentA = new SeededRandomService(42UL);
            for (int i = 0; i < 100; i++)
            {
                parentA.NextInt(100);
            }
            var layoutA = ExplorationLayoutGenerator.Generate(config, parentA);

            var parentB = new SeededRandomService(42UL);
            var layoutB = ExplorationLayoutGenerator.Generate(config, parentB);

            Assert.That(layoutA.StartingPosition, Is.EqualTo(layoutB.StartingPosition));
            var blockedA = layoutA.Grid.Cells.Where(c => !c.IsWalkable).Select(c => c.Coordinate).ToHashSet();
            var blockedB = layoutB.Grid.Cells.Where(c => !c.IsWalkable).Select(c => c.Coordinate).ToHashSet();
            Assert.That(blockedA.SetEquals(blockedB), Is.True);
        }

        [Test]
        public void Generate_CrossSystemIsolation_DoesNotPerturbOtherSubstreams()
        {
            var config = new ExplorationGenerationConfiguration(8, 6, 10, 8);

            // Scenario A: Generate exploration layout first, then sample battle stream
            var rngA = new SeededRandomService(42UL);
            ExplorationLayoutGenerator.Generate(config, rngA);
            var battleStreamA = rngA.CreateSubstream("battle.starting-hand");
            var samplesA = Enumerable.Range(0, 10).Select(_ => battleStreamA.NextInt(100)).ToList();

            // Scenario B: Do not generate exploration layout, directly sample battle stream
            var rngB = new SeededRandomService(42UL);
            var battleStreamB = rngB.CreateSubstream("battle.starting-hand");
            var samplesB = Enumerable.Range(0, 10).Select(_ => battleStreamB.NextInt(100)).ToList();

            Assert.That(samplesA, Is.EqualTo(samplesB));
        }

        [Test]
        public void Generate_ContentSubstreamIsolation_DoesNotInterfereWithLayout()
        {
            var config = new ExplorationGenerationConfiguration(8, 6, 10, 8);

            // Scenario A: Sample content stream first, then generate layout
            var rngA = new SeededRandomService(42UL);
            var contentStream = rngA.CreateSubstream(ExplorationLayoutGenerator.ContentSubstreamId);
            for (int i = 0; i < 50; i++)
            {
                contentStream.NextInt(1000);
            }
            var layoutA = ExplorationLayoutGenerator.Generate(config, rngA);

            // Scenario B: Generate layout without touching content stream
            var rngB = new SeededRandomService(42UL);
            var layoutB = ExplorationLayoutGenerator.Generate(config, rngB);

            Assert.That(layoutA.StartingPosition, Is.EqualTo(layoutB.StartingPosition));
            var blockedA = layoutA.Grid.Cells.Where(c => !c.IsWalkable).Select(c => c.Coordinate).ToHashSet();
            var blockedB = layoutB.Grid.Cells.Where(c => !c.IsWalkable).Select(c => c.Coordinate).ToHashSet();
            Assert.That(blockedA.SetEquals(blockedB), Is.True);
        }

        [TestCase(1UL)]
        [TestCase(42UL)]
        [TestCase(100UL)]
        [TestCase(777UL)]
        [TestCase(12345UL)]
        [TestCase(999999UL)]
        public void Generate_MultipleFixedSeeds_AllSatisfyReachabilityAndGridInvariants(ulong seed)
        {
            var config = new ExplorationGenerationConfiguration(8, 6, 10, 8);
            var rng = new SeededRandomService(seed);
            var layout = ExplorationLayoutGenerator.Generate(config, rng);

            Assert.That(layout.Grid.TotalCellCount, Is.EqualTo(48));

            var coords = layout.Grid.Cells.Select(c => c.Coordinate).ToHashSet();
            Assert.That(coords.Count, Is.EqualTo(48));

            Assert.That(layout.Grid.IsWalkable(layout.StartingPosition), Is.True);

            int blockedCount = layout.Grid.Cells.Count(c => !c.IsWalkable);
            Assert.That(blockedCount, Is.EqualTo(8));

            int sx = layout.StartingPosition.X;
            int sy = layout.StartingPosition.Y;
            var neighbors = new[]
            {
                new ExplorationCoordinate(sx, sy + 1),
                new ExplorationCoordinate(sx, sy - 1),
                new ExplorationCoordinate(sx - 1, sy),
                new ExplorationCoordinate(sx + 1, sy)
            };

            bool hasWalkableNeighbor = neighbors.Any(n => layout.Grid.Contains(n) && layout.Grid.IsWalkable(n));
            Assert.That(hasWalkableNeighbor, Is.True, $"Seed {seed} must produce at least one walkable neighbor from start.");
        }

        [Test]
        public void Generate_KnownSeedStabilityVector_RemainsStable()
        {
            var config = new ExplorationGenerationConfiguration(8, 6, 10, 8);
            var rng = new SeededRandomService(42UL);
            var layout = ExplorationLayoutGenerator.Generate(config, rng);

            // Locked vector for seed 42
            Assert.That(layout.StartingPosition, Is.EqualTo(new ExplorationCoordinate(7, 1)));
            Assert.That(layout.InitialActionPoints, Is.EqualTo(10));

            var blockedCoords = layout.Grid.Cells
                .Where(c => !c.IsWalkable)
                .Select(c => c.Coordinate)
                .OrderBy(c => c.X)
                .ThenBy(c => c.Y)
                .ToList();

            var expectedBlocked = new[]
            {
                new ExplorationCoordinate(1, 0),
                new ExplorationCoordinate(2, 0),
                new ExplorationCoordinate(2, 3),
                new ExplorationCoordinate(3, 2),
                new ExplorationCoordinate(4, 3),
                new ExplorationCoordinate(4, 5),
                new ExplorationCoordinate(6, 4),
                new ExplorationCoordinate(7, 3)
            };

            Assert.That(blockedCoords, Is.EqualTo(expectedBlocked));
        }
    }
}
