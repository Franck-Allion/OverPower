using System;

namespace OverPower.Domain.Run
{
    /// <summary>
    /// An immutable value object representing the authoritative root seed of a game Run.
    /// </summary>
    /// <remarks>
    /// One Run owns exactly one immutable root RunSeed. Subsystems derive their own 
    /// deterministic random streams exclusively from this root seed.
    /// </remarks>
    public readonly struct RunSeed : IEquatable<RunSeed>
    {
        public ulong Value { get; }

        public RunSeed(ulong value)
        {
            Value = value;
        }

        public bool Equals(RunSeed other) => Value == other.Value;

        public override bool Equals(object? obj) => obj is RunSeed other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();

        public static bool operator ==(RunSeed left, RunSeed right) => left.Equals(right);

        public static bool operator !=(RunSeed left, RunSeed right) => !left.Equals(right);
    }
}
