using System;
using OverPower.Domain.Primitives;

namespace OverPower.Domain.Exploration
{
    /// <summary>
    /// Authoritative pure Domain runtime state for one exploration room session.
    /// Manages player position, remaining action points and room completion semantics.
    /// </summary>
    public sealed class ExplorationState
    {
        public ExplorationGrid Grid { get; }
        public ExplorationCoordinate PlayerPosition { get; private set; }
        public int RemainingActionPoints { get; private set; }
        public ExplorationCompletionReason CompletionReason { get; private set; }

        public bool IsCompleted => CompletionReason != ExplorationCompletionReason.None;

        /// <summary>
        /// Creates a new ExplorationState with validated grid, starting position and initial action points.
        /// </summary>
        public ExplorationState(ExplorationGrid grid, ExplorationCoordinate startingPosition, int startingActionPoints)
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

            if (startingActionPoints < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startingActionPoints), "Starting action points cannot be negative.");
            }

            PlayerPosition = startingPosition;
            RemainingActionPoints = startingActionPoints;

            if (startingActionPoints == 0)
            {
                CompletionReason = ExplorationCompletionReason.ActionPointsExhausted;
            }
            else
            {
                CompletionReason = ExplorationCompletionReason.None;
            }
        }

        /// <summary>
        /// Attempts to move the player to a target coordinate.
        /// Consumes 1 action point and updates position if legal.
        /// Reaching 0 action points marks exploration as completed.
        /// </summary>
        public Result TryMove(ExplorationCoordinate destination)
        {
            if (IsCompleted)
            {
                return Result.Failure(ExplorationErrors.ExplorationCompleted);
            }

            if (RemainingActionPoints <= 0)
            {
                return Result.Failure(ExplorationErrors.NoActionPoints);
            }

            if (!Grid.Contains(destination))
            {
                return Result.Failure(ExplorationErrors.OutOfBounds);
            }

            if (!destination.IsOrthogonallyAdjacentTo(PlayerPosition))
            {
                return Result.Failure(ExplorationErrors.NotAdjacent);
            }

            if (!Grid.IsWalkable(destination))
            {
                return Result.Failure(ExplorationErrors.NonWalkable);
            }

            PlayerPosition = destination;
            RemainingActionPoints--;

            if (RemainingActionPoints == 0)
            {
                CompletionReason = ExplorationCompletionReason.ActionPointsExhausted;
            }

            return Result.Success();
        }

        /// <summary>
        /// Convenience overload accepting (x, y) coordinates.
        /// </summary>
        public Result TryMove(int x, int y) => TryMove(new ExplorationCoordinate(x, y));

        /// <summary>
        /// Explicitly concludes the exploration session before exhausting action points.
        /// Marks exploration as completed with EndedExplicitly.
        /// </summary>
        public Result EndExploration()
        {
            if (IsCompleted)
            {
                return Result.Failure(ExplorationErrors.AlreadyCompleted);
            }

            CompletionReason = ExplorationCompletionReason.EndedExplicitly;
            return Result.Success();
        }
    }
}
