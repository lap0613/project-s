using System;

namespace Whiteout.Core.Stats
{
    /// <summary>
    /// String-backed stat identifier. Not an enum, so new stats need no change to existing code.
    /// Equality is ordinal string equality.
    /// </summary>
    /// <example>
    /// <code>
    /// var id = new StatId("player.move_speed");
    /// </code>
    /// </example>
    public readonly struct StatId : IEquatable<StatId>
    {
        /// <summary>Creates an id.</summary>
        /// <param name="value">Non-empty identifier, e.g. "player.move_speed".</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is null or empty.</exception>
        public StatId(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Stat id must not be empty.", nameof(value));
            }

            Value = value;
        }

        /// <summary>The identifier text (null for a default-constructed id).</summary>
        public string Value { get; }

        /// <inheritdoc />
        public bool Equals(StatId other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is StatId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value == null ? 0 : StringComparer.Ordinal.GetHashCode(Value);

        /// <inheritdoc />
        public override string ToString() => Value ?? string.Empty;

        /// <summary>Equality operator.</summary>
        public static bool operator ==(StatId left, StatId right) => left.Equals(right);

        /// <summary>Inequality operator.</summary>
        public static bool operator !=(StatId left, StatId right) => !left.Equals(right);
    }
}
