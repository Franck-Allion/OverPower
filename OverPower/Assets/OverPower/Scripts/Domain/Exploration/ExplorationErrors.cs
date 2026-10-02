using OverPower.Domain.Primitives;

namespace OverPower.Domain.Exploration
{
    /// <summary>
    /// Deterministic DomainError definitions for exploration validation and movement.
    /// </summary>
    public static class ExplorationErrors
    {
        public static readonly DomainError ExplorationCompleted =
            new("exploration.completed", "Exploration is already completed.");

        public static readonly DomainError AlreadyCompleted =
            new("exploration.already_completed", "Exploration has already ended.");

        public static readonly DomainError NoActionPoints =
            new("exploration.no_action_points", "No remaining action points to move.");

        public static readonly DomainError OutOfBounds =
            new("exploration.out_of_bounds", "Destination coordinate is outside the grid bounds.");

        public static readonly DomainError NotAdjacent =
            new("exploration.not_adjacent", "Destination must be orthogonally adjacent to current position.");

        public static readonly DomainError NonWalkable =
            new("exploration.non_walkable", "Destination cell is not walkable.");
    }
}
