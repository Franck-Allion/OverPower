using System;

namespace OverPower.Domain.Exploration
{
    /// <summary>
    /// Immutable parameters defining room boundaries, action points, and obstacle density for exploration layout generation.
    /// </summary>
    public sealed class ExplorationGenerationConfiguration
    {
        public int Width { get; }
        public int Height { get; }
        public int InitialActionPoints { get; }
        public int BlockedCellCount { get; }

        public ExplorationGenerationConfiguration(int width, int height, int initialActionPoints, int blockedCellCount)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Grid width must be greater than zero.");
            }
            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Grid height must be greater than zero.");
            }
            if (initialActionPoints < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialActionPoints), "Initial action points cannot be negative.");
            }
            if (blockedCellCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockedCellCount), "Blocked cell count cannot be negative.");
            }

            int totalCells = checked(width * height);
            int maxAllowedBlocked = totalCells > 1 ? totalCells - 2 : 0;
            if (blockedCellCount > maxAllowedBlocked)
            {
                throw new ArgumentException(
                    $"Blocked cell count ({blockedCellCount}) exceeds the maximum allowed ({maxAllowedBlocked}) for a {width}x{height} grid to guarantee a walkable starting position and adjacent movement.",
                    nameof(blockedCellCount));
            }

            Width = width;
            Height = height;
            InitialActionPoints = initialActionPoints;
            BlockedCellCount = blockedCellCount;
        }
    }
}
