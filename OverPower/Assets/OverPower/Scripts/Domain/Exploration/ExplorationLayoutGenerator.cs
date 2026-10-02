using System;
using System.Collections.Generic;
using OverPower.Domain.Random;

namespace OverPower.Domain.Exploration
{
    /// <summary>
    /// Deterministic generator creating exploration room layouts from an isolated RNG substream.
    /// </summary>
    public static class ExplorationLayoutGenerator
    {
        /// <summary>
        /// Semantic substream ID for exploration terrain and starting position generation.
        /// </summary>
        public const string LayoutSubstreamId = "exploration.layout";

        /// <summary>
        /// Semantic substream ID reserved for future exploration pickup and encounter generation.
        /// </summary>
        public const string ContentSubstreamId = "exploration.content";

        /// <summary>
        /// Deterministically generates an ExplorationLayout using the configuration and provided random service.
        /// Derives the isolated "exploration.layout" substream to preserve root and cross-system RNG sequences.
        /// </summary>
        public static ExplorationLayout Generate(
            ExplorationGenerationConfiguration configuration,
            IRandomService random)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            var layoutRng = random.CreateSubstream(LayoutSubstreamId);

            // 1. Deterministically select starting position
            int startX = layoutRng.NextInt(configuration.Width);
            int startY = layoutRng.NextInt(configuration.Height);
            var startPos = new ExplorationCoordinate(startX, startY);

            // 2. Identify available orthogonal neighbors of starting position
            var neighbors = new List<ExplorationCoordinate>(4);
            if (startY + 1 < configuration.Height) neighbors.Add(new ExplorationCoordinate(startX, startY + 1));
            if (startY - 1 >= 0) neighbors.Add(new ExplorationCoordinate(startX, startY - 1));
            if (startX - 1 >= 0) neighbors.Add(new ExplorationCoordinate(startX - 1, startY));
            if (startX + 1 < configuration.Width) neighbors.Add(new ExplorationCoordinate(startX + 1, startY));

            // 3. Guarantee at least one walkable neighbor if neighbors exist
            ExplorationCoordinate? guaranteedWalkableNeighbor = null;
            if (neighbors.Count > 0)
            {
                int neighborIndex = layoutRng.NextInt(neighbors.Count);
                guaranteedWalkableNeighbor = neighbors[neighborIndex];
            }

            // 4. Gather candidate coordinates for obstacle placement (excluding start and guaranteed neighbor)
            var candidateCoords = new List<ExplorationCoordinate>(configuration.Width * configuration.Height);
            for (int y = 0; y < configuration.Height; y++)
            {
                for (int x = 0; x < configuration.Width; x++)
                {
                    var coord = new ExplorationCoordinate(x, y);
                    if (coord != startPos && coord != guaranteedWalkableNeighbor)
                    {
                        candidateCoords.Add(coord);
                    }
                }
            }

            // 5. Deterministically shuffle candidates and pick blocked cells
            layoutRng.Shuffle(candidateCoords);

            var blockedSet = new HashSet<ExplorationCoordinate>();
            for (int i = 0; i < configuration.BlockedCellCount; i++)
            {
                blockedSet.Add(candidateCoords[i]);
            }

            // 6. Build the exploration grid
            var grid = ExplorationGrid.Create(
                configuration.Width,
                configuration.Height,
                coord => !blockedSet.Contains(coord));

            return new ExplorationLayout(grid, startPos, configuration.InitialActionPoints);
        }
    }
}
