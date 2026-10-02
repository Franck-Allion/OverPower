using System;
using System.Collections.Generic;

namespace OverPower.Domain.Exploration
{
    /// <summary>
    /// Pure Domain grid representation managing coordinate boundaries and cell walkability.
    /// </summary>
    public sealed class ExplorationGrid
    {
        private readonly Dictionary<ExplorationCoordinate, ExplorationCell> _cells;

        public int Width { get; }
        public int Height { get; }
        public int TotalCellCount => _cells.Count;

        public IReadOnlyCollection<ExplorationCell> Cells => _cells.Values;

        /// <summary>
        /// Creates an ExplorationGrid from explicit width, height and a complete collection of cells.
        /// </summary>
        public ExplorationGrid(int width, int height, IEnumerable<ExplorationCell> cells)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Grid width must be greater than zero.");
            }
            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Grid height must be greater than zero.");
            }
            if (cells == null)
            {
                throw new ArgumentNullException(nameof(cells), "Cells collection cannot be null.");
            }

            Width = width;
            Height = height;

            int expectedCount = checked(width * height);
            _cells = new Dictionary<ExplorationCoordinate, ExplorationCell>(expectedCount);

            foreach (var cell in cells)
            {
                var coord = cell.Coordinate;
                if (coord.X < 0 || coord.X >= width || coord.Y < 0 || coord.Y >= height)
                {
                    throw new ArgumentOutOfRangeException(nameof(cells), $"Cell coordinate {coord} is outside the grid bounds ({width}x{height}).");
                }
                if (_cells.ContainsKey(coord))
                {
                    throw new ArgumentException($"Duplicate cell coordinate {coord} encountered during grid construction.", nameof(cells));
                }
                _cells[coord] = cell;
            }

            if (_cells.Count != expectedCount)
            {
                throw new ArgumentException($"Incomplete cell map: expected {expectedCount} cells ({width}x{height}), but received {_cells.Count}.", nameof(cells));
            }
        }

        /// <summary>
        /// Factory creating an ExplorationGrid with a walkability predicate for all coordinates in [0..width-1, 0..height-1].
        /// </summary>
        public static ExplorationGrid Create(int width, int height, Func<ExplorationCoordinate, bool> walkabilityPredicate)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Grid width must be greater than zero.");
            }
            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Grid height must be greater than zero.");
            }
            if (walkabilityPredicate == null)
            {
                throw new ArgumentNullException(nameof(walkabilityPredicate), "Walkability predicate cannot be null.");
            }

            var cells = new List<ExplorationCell>(width * height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var coord = new ExplorationCoordinate(x, y);
                    cells.Add(new ExplorationCell(coord, walkabilityPredicate(coord)));
                }
            }

            return new ExplorationGrid(width, height, cells);
        }

        public bool Contains(ExplorationCoordinate coordinate)
        {
            return coordinate.X >= 0 && coordinate.X < Width && coordinate.Y >= 0 && coordinate.Y < Height;
        }

        public bool Contains(int x, int y) => Contains(new ExplorationCoordinate(x, y));

        public bool IsWalkable(ExplorationCoordinate coordinate)
        {
            return _cells.TryGetValue(coordinate, out var cell) && cell.IsWalkable;
        }

        public bool IsWalkable(int x, int y) => IsWalkable(new ExplorationCoordinate(x, y));

        public bool TryGetCell(ExplorationCoordinate coordinate, out ExplorationCell cell)
        {
            return _cells.TryGetValue(coordinate, out cell);
        }

        public ExplorationCell GetCell(ExplorationCoordinate coordinate)
        {
            if (_cells.TryGetValue(coordinate, out var cell))
            {
                return cell;
            }
            throw new KeyNotFoundException($"No cell found at coordinate {coordinate}.");
        }
    }
}
