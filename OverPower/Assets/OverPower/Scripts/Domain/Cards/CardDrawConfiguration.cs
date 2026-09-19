using System;

namespace OverPower.Domain.Cards
{
    /// <summary>
    /// Immutable configuration of battle card flow settings.
    /// </summary>
    public sealed class CardDrawConfiguration
    {
        public int StartingHandSize { get; }
        public int MaximumHandSize { get; }
        public int DrawCountPerTurn { get; }

        public static CardDrawConfiguration MvpDefault { get; } = new CardDrawConfiguration(3, 4, 1);

        public CardDrawConfiguration(int startingHandSize, int maximumHandSize, int drawCountPerTurn)
        {
            if (maximumHandSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumHandSize), "Maximum hand size must be greater than zero.");
            }
            if (startingHandSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startingHandSize), "Starting hand size cannot be negative.");
            }
            if (startingHandSize > maximumHandSize)
            {
                throw new ArgumentException("Starting hand size cannot exceed maximum hand size.");
            }
            if (drawCountPerTurn < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(drawCountPerTurn), "Draw count per turn cannot be negative.");
            }

            StartingHandSize = startingHandSize;
            MaximumHandSize = maximumHandSize;
            DrawCountPerTurn = drawCountPerTurn;
        }
    }
}
