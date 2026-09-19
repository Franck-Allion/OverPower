using System;
using System.Globalization;

namespace OverPower.Domain.Cards
{
    /// <summary>
    /// Identifies a physical card, independently of its authored content.
    /// The creator supplies deterministic IDs; zero (including default) is invalid.
    /// </summary>
    public readonly struct CardInstanceId : IEquatable<CardInstanceId>
    {
        public ulong Value { get; }

        public CardInstanceId(ulong value)
        {
            if (value == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Card instance ID must be greater than zero.");
            }

            Value = value;
        }

        public bool Equals(CardInstanceId other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is CardInstanceId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
        public static bool operator ==(CardInstanceId left, CardInstanceId right) => left.Equals(right);
        public static bool operator !=(CardInstanceId left, CardInstanceId right) => !left.Equals(right);
    }
}
