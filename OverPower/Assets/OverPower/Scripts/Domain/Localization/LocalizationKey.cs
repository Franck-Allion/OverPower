using System;

namespace OverPower.Domain.Localization
{
    /// <summary>
    /// An immutable value object representing a stable, Unity-independent reference to a localized string key.
    /// </summary>
    public readonly struct LocalizationKey : IEquatable<LocalizationKey>
    {
        public string Value { get; }

        public LocalizationKey(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "Localization key cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Localization key cannot be empty or whitespace.", nameof(value));
            }

            Value = value;
        }

        public bool Equals(LocalizationKey other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            return obj is LocalizationKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode(StringComparison.Ordinal);
        }

        public override string ToString() => Value;

        public static bool operator ==(LocalizationKey left, LocalizationKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LocalizationKey left, LocalizationKey right)
        {
            return !left.Equals(right);
        }
    }
}
