using System;

namespace OverPower.Domain.Exploration
{
    /// <summary>
    /// Immutable value representing a cell and its terrain walkability in the exploration grid.
    /// </summary>
    public readonly struct ExplorationCell : IEquatable<ExplorationCell>
    {
        public ExplorationCoordinate Coordinate { get; }
        public bool IsWalkable { get; }

        public ExplorationCell(ExplorationCoordinate coordinate, bool isWalkable)
        {
            Coordinate = coordinate;
            IsWalkable = isWalkable;
        }

        public ExplorationCell(int x, int y, bool isWalkable)
            : this(new ExplorationCoordinate(x, y), isWalkable)
        {
        }

        public bool Equals(ExplorationCell other) => Coordinate == other.Coordinate && IsWalkable == other.IsWalkable;

        public override bool Equals(object? obj) => obj is ExplorationCell other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Coordinate, IsWalkable);

        public override string ToString() => $"{Coordinate} [{(IsWalkable ? "Walkable" : "Blocked")}]";

        public static bool operator ==(ExplorationCell left, ExplorationCell right) => left.Equals(right);

        public static bool operator !=(ExplorationCell left, ExplorationCell right) => !left.Equals(right);
    }
}
