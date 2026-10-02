using System;

namespace OverPower.Domain.Exploration
{
    /// <summary>
    /// Pure Domain representation of a generated exploration room layout, including terrain and starting conditions.
    /// </summary>
    public sealed class ExplorationLayout
    {
        public ExplorationGrid Grid { get; }
        public ExplorationCoordinate StartingPosition { get; }
        public int InitialActionPoints { get; }

        public ExplorationLayout(ExplorationGrid grid, ExplorationCoordinate startingPosition, int initialActionPoints)
        {
            Grid = grid ?? throw new ArgumentNullException(nameof(grid), "Exploration grid cannot be null.");

            if (!grid.Contains(startingPosition))
            {
                throw new ArgumentException($"Starting position {startingPosition} is outside the grid bounds ({grid.Width}x{grid.Height}).", nameof(startingPosition));
            }

            if (!grid.IsWalkable(startingPosition))
            {
                throw new ArgumentException($"Starting position {startingPosition} must be on a walkable cell.", nameof(startingPosition));
            }

            if (initialActionPoints < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialActionPoints), "Initial action points cannot be negative.");
            }

            StartingPosition = startingPosition;
            InitialActionPoints = initialActionPoints;
        }

        /// <summary>
        /// Instantiates an authoritative runtime ExplorationState from this layout.
        /// </summary>
        public ExplorationState CreateState()
        {
            return new ExplorationState(Grid, StartingPosition, InitialActionPoints);
        }
    }
}
