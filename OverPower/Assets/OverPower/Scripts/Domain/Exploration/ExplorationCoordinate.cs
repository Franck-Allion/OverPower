using System;

namespace OverPower.Domain.Exploration
{
    /// <summary>
    /// Immutable 2D integer coordinate representing a cell position in exploration grid space.
    /// </summary>
    public readonly struct ExplorationCoordinate : IEquatable<ExplorationCoordinate>
    {
        public int X { get; }
        public int Y { get; }

        public ExplorationCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Determines whether another coordinate is an immediate orthogonal 4-neighbor (up, down, left, right).
        /// </summary>
        public bool IsOrthogonallyAdjacentTo(ExplorationCoordinate other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) == 1;
        }

        public bool Equals(ExplorationCoordinate other) => X == other.X && Y == other.Y;

        public override bool Equals(object? obj) => obj is ExplorationCoordinate other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public override string ToString() => $"({X}, {Y})";

        public static bool operator ==(ExplorationCoordinate left, ExplorationCoordinate right) => left.Equals(right);

        public static bool operator !=(ExplorationCoordinate left, ExplorationCoordinate right) => !left.Equals(right);
    }
}
