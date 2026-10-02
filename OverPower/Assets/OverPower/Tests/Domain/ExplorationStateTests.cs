using System;
using System.Collections.Generic;
using NUnit.Framework;
using OverPower.Domain.Exploration;

namespace OverPower.Tests.Domain
{
    [TestFixture]
    public sealed class ExplorationStateTests
    {
        [Test]
        public void Coordinate_ValueEqualityAndHashing_BehavesCorrectly()
        {
            var a = new ExplorationCoordinate(2, 3);
            var b = new ExplorationCoordinate(2, 3);
            var c = new ExplorationCoordinate(3, 2);

            Assert.That(a, Is.EqualTo(b));
            Assert.That(a == b, Is.True);
            Assert.That(a != c, Is.True);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
            Assert.That(a.ToString(), Is.EqualTo("(2, 3)"));
        }

        [TestCase(2, 3, 2, 4, true)]   // Up
        [TestCase(2, 3, 2, 2, true)]   // Down
        [TestCase(2, 3, 1, 3, true)]   // Left
        [TestCase(2, 3, 3, 3, true)]   // Right
        [TestCase(2, 3, 2, 3, false)]  // Same cell
        [TestCase(2, 3, 3, 4, false)]  // Diagonal
        [TestCase(2, 3, 1, 2, false)]  // Diagonal
        [TestCase(2, 3, 2, 5, false)]  // 2 cells away
        public void Coordinate_IsOrthogonallyAdjacentTo_MatchesFourNeighborRule(int x1, int y1, int x2, int y2, bool expected)
        {
            var from = new ExplorationCoordinate(x1, y1);
            var to = new ExplorationCoordinate(x2, y2);

            Assert.That(from.IsOrthogonallyAdjacentTo(to), Is.EqualTo(expected));
        }

        [Test]
        public void Grid_Construction_ValidatesDimensionsAndCells()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ExplorationGrid(0, 5, Array.Empty<ExplorationCell>()));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ExplorationGrid(5, 0, Array.Empty<ExplorationCell>()));
            Assert.Throws<ArgumentNullException>(() => new ExplorationGrid(2, 2, null!));

            // Duplicate coordinates
            var duplicates = new[]
            {
                new ExplorationCell(0, 0, true),
                new ExplorationCell(0, 0, false),
                new ExplorationCell(0, 1, true),
                new ExplorationCell(1, 0, true)
            };
            Assert.Throws<ArgumentException>(() => new ExplorationGrid(2, 2, duplicates));

            // Coordinates out of bounds
            var outOfBounds = new[]
            {
                new ExplorationCell(0, 0, true),
                new ExplorationCell(0, 1, true),
                new ExplorationCell(1, 0, true),
                new ExplorationCell(2, 0, true) // x=2 is out of [0..1]
            };
            Assert.Throws<ArgumentOutOfRangeException>(() => new ExplorationGrid(2, 2, outOfBounds));

            // Incomplete cells count (3 cells instead of 4)
            var incomplete = new[]
            {
                new ExplorationCell(0, 0, true),
                new ExplorationCell(0, 1, true),
                new ExplorationCell(1, 0, true)
            };
            Assert.Throws<ArgumentException>(() => new ExplorationGrid(2, 2, incomplete));
        }

        [Test]
        public void Grid_Queries_ReturnExpectedWalkabilityAndBounds()
        {
            var grid = ExplorationGrid.Create(3, 3, c => !(c.X == 1 && c.Y == 1)); // (1,1) is blocked

            Assert.That(grid.Width, Is.EqualTo(3));
            Assert.That(grid.Height, Is.EqualTo(3));
            Assert.That(grid.TotalCellCount, Is.EqualTo(9));

            Assert.That(grid.Contains(0, 0), Is.True);
            Assert.That(grid.Contains(2, 2), Is.True);
            Assert.That(grid.Contains(-1, 0), Is.False);
            Assert.That(grid.Contains(0, 3), Is.False);

            Assert.That(grid.IsWalkable(0, 0), Is.True);
            Assert.That(grid.IsWalkable(1, 1), Is.False);
            Assert.That(grid.IsWalkable(99, 99), Is.False);

            Assert.That(grid.TryGetCell(new ExplorationCoordinate(1, 1), out var cell), Is.True);
            Assert.That(cell.IsWalkable, Is.False);
        }

        [Test]
        public void State_InitialValidation_RejectsInvalidStartingConditions()
        {
            var grid = ExplorationGrid.Create(3, 3, c => !(c.X == 0 && c.Y == 0)); // (0,0) blocked

            // Null grid
            Assert.Throws<ArgumentNullException>(() => new ExplorationState(null!, new ExplorationCoordinate(1, 1), 5));

            // Start position out of bounds
            Assert.Throws<ArgumentException>(() => new ExplorationState(grid, new ExplorationCoordinate(5, 5), 5));

            // Start position non-walkable
            Assert.Throws<ArgumentException>(() => new ExplorationState(grid, new ExplorationCoordinate(0, 0), 5));

            // Negative AP
            Assert.Throws<ArgumentOutOfRangeException>(() => new ExplorationState(grid, new ExplorationCoordinate(1, 1), -1));
        }

        [Test]
        public void State_InitialZeroActionPoints_CompletesImmediatelyAsExhausted()
        {
            var grid = ExplorationGrid.Create(3, 3, _ => true);
            var state = new ExplorationState(grid, new ExplorationCoordinate(1, 1), 0);

            Assert.That(state.RemainingActionPoints, Is.EqualTo(0));
            Assert.That(state.IsCompleted, Is.True);
            Assert.That(state.CompletionReason, Is.EqualTo(ExplorationCompletionReason.ActionPointsExhausted));

            var moveResult = state.TryMove(1, 2);
            Assert.That(moveResult.IsFailure, Is.True);
            Assert.That(moveResult.Error.Code, Is.EqualTo(ExplorationErrors.ExplorationCompleted.Code));
        }

        [Test]
        public void State_LegalMovesInAllFourDirections_UpdatesPositionAndConsumesActionPoints()
        {
            var grid = ExplorationGrid.Create(5, 5, _ => true);
            var state = new ExplorationState(grid, new ExplorationCoordinate(2, 2), 10);

            // Move Up -> (2, 3)
            var up = state.TryMove(2, 3);
            Assert.That(up.IsSuccess, Is.True);
            Assert.That(state.PlayerPosition, Is.EqualTo(new ExplorationCoordinate(2, 3)));
            Assert.That(state.RemainingActionPoints, Is.EqualTo(9));
            Assert.That(state.IsCompleted, Is.False);

            // Move Right -> (3, 3)
            var right = state.TryMove(3, 3);
            Assert.That(right.IsSuccess, Is.True);
            Assert.That(state.PlayerPosition, Is.EqualTo(new ExplorationCoordinate(3, 3)));
            Assert.That(state.RemainingActionPoints, Is.EqualTo(8));

            // Move Down -> (3, 2)
            var down = state.TryMove(3, 2);
            Assert.That(down.IsSuccess, Is.True);
            Assert.That(state.PlayerPosition, Is.EqualTo(new ExplorationCoordinate(3, 2)));
            Assert.That(state.RemainingActionPoints, Is.EqualTo(7));

            // Move Left -> (2, 2)
            var left = state.TryMove(2, 2);
            Assert.That(left.IsSuccess, Is.True);
            Assert.That(state.PlayerPosition, Is.EqualTo(new ExplorationCoordinate(2, 2)));
            Assert.That(state.RemainingActionPoints, Is.EqualTo(6));
        }

        [Test]
        public void State_IllegalMoves_AreAtomicAndDoNotMutateState()
        {
            var grid = ExplorationGrid.Create(3, 3, c => !(c.X == 2 && c.Y == 1)); // (2,1) blocked
            var state = new ExplorationState(grid, new ExplorationCoordinate(1, 1), 3);

            // 1. Same cell
            var same = state.TryMove(1, 1);
            Assert.That(same.IsFailure, Is.True);
            Assert.That(same.Error.Code, Is.EqualTo(ExplorationErrors.NotAdjacent.Code));
            Assert.That(state.PlayerPosition, Is.EqualTo(new ExplorationCoordinate(1, 1)));
            Assert.That(state.RemainingActionPoints, Is.EqualTo(3));

            // 2. Diagonal move
            var diagonal = state.TryMove(2, 2);
            Assert.That(diagonal.IsFailure, Is.True);
            Assert.That(diagonal.Error.Code, Is.EqualTo(ExplorationErrors.NotAdjacent.Code));
            Assert.That(state.PlayerPosition, Is.EqualTo(new ExplorationCoordinate(1, 1)));
            Assert.That(state.RemainingActionPoints, Is.EqualTo(3));

            // 3. Multi-cell distance
            var multi = state.TryMove(1, 3); // y=3 out of bounds, multi-distance
            Assert.That(multi.IsFailure, Is.True);
            Assert.That(state.PlayerPosition, Is.EqualTo(new ExplorationCoordinate(1, 1)));
            Assert.That(state.RemainingActionPoints, Is.EqualTo(3));

            // 4. Non-walkable destination
            var blocked = state.TryMove(2, 1);
            Assert.That(blocked.IsFailure, Is.True);
            Assert.That(blocked.Error.Code, Is.EqualTo(ExplorationErrors.NonWalkable.Code));
            Assert.That(state.PlayerPosition, Is.EqualTo(new ExplorationCoordinate(1, 1)));
            Assert.That(state.RemainingActionPoints, Is.EqualTo(3));

            // 5. Out of bounds adjacent
            var oobGrid = ExplorationGrid.Create(2, 2, _ => true);
            var oobState = new ExplorationState(oobGrid, new ExplorationCoordinate(0, 0), 2);
            var oobMove = oobState.TryMove(-1, 0);
            Assert.That(oobMove.IsFailure, Is.True);
            Assert.That(oobMove.Error.Code, Is.EqualTo(ExplorationErrors.OutOfBounds.Code));
            Assert.That(oobState.PlayerPosition, Is.EqualTo(new ExplorationCoordinate(0, 0)));
            Assert.That(oobState.RemainingActionPoints, Is.EqualTo(2));
        }

        [Test]
        public void State_ExhaustingActionPoints_CompletesSessionAndRejectsFurtherMovement()
        {
            var grid = ExplorationGrid.Create(3, 3, _ => true);
            var state = new ExplorationState(grid, new ExplorationCoordinate(0, 0), 1);

            Assert.That(state.IsCompleted, Is.False);
            Assert.That(state.RemainingActionPoints, Is.EqualTo(1));

            var move = state.TryMove(0, 1);
            Assert.That(move.IsSuccess, Is.True);
            Assert.That(state.RemainingActionPoints, Is.EqualTo(0));
            Assert.That(state.IsCompleted, Is.True);
            Assert.That(state.CompletionReason, Is.EqualTo(ExplorationCompletionReason.ActionPointsExhausted));

            // Subsequent move attempt
            var nextMove = state.TryMove(0, 2);
            Assert.That(nextMove.IsFailure, Is.True);
            Assert.That(nextMove.Error.Code, Is.EqualTo(ExplorationErrors.ExplorationCompleted.Code));
            Assert.That(state.PlayerPosition, Is.EqualTo(new ExplorationCoordinate(0, 1)));
            Assert.That(state.RemainingActionPoints, Is.EqualTo(0));
        }

        [Test]
        public void State_EndExploration_ConcludesSessionExplicitlyWithoutMutatingPositionOrActionPoints()
        {
            var grid = ExplorationGrid.Create(3, 3, _ => true);
            var state = new ExplorationState(grid, new ExplorationCoordinate(1, 1), 5);

            var endResult = state.EndExploration();
            Assert.That(endResult.IsSuccess, Is.True);
            Assert.That(state.IsCompleted, Is.True);
            Assert.That(state.CompletionReason, Is.EqualTo(ExplorationCompletionReason.EndedExplicitly));
            Assert.That(state.PlayerPosition, Is.EqualTo(new ExplorationCoordinate(1, 1)));
            Assert.That(state.RemainingActionPoints, Is.EqualTo(5));

            // Repeated call returns already completed failure
            var repeatEnd = state.EndExploration();
            Assert.That(repeatEnd.IsFailure, Is.True);
            Assert.That(repeatEnd.Error.Code, Is.EqualTo(ExplorationErrors.AlreadyCompleted.Code));
            Assert.That(state.CompletionReason, Is.EqualTo(ExplorationCompletionReason.EndedExplicitly));

            // Movement after explicit end is rejected
            var moveResult = state.TryMove(1, 2);
            Assert.That(moveResult.IsFailure, Is.True);
            Assert.That(moveResult.Error.Code, Is.EqualTo(ExplorationErrors.ExplorationCompleted.Code));
            Assert.That(state.PlayerPosition, Is.EqualTo(new ExplorationCoordinate(1, 1)));
            Assert.That(state.RemainingActionPoints, Is.EqualTo(5));
        }
    }
}
