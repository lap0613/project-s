using System;

namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// String-backed item identifier shared by Resources and Camp. Not an enum, so new items need no change to
    /// existing code. Equality is ordinal string equality.
    /// </summary>
    /// <example>
    /// <code>
    /// var wood = new ItemId("wood");
    /// </code>
    /// </example>
    public readonly struct ItemId : IEquatable<ItemId>
    {
        /// <summary>Creates an id.</summary>
        /// <param name="value">Non-empty identifier, e.g. "wood".</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is null or empty.</exception>
        public ItemId(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Item id must not be empty.", nameof(value));
            }

            Value = value;
        }

        /// <summary>The identifier text (null for a default-constructed id).</summary>
        public string Value { get; }

        /// <inheritdoc />
        public bool Equals(ItemId other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is ItemId other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value == null ? 0 : StringComparer.Ordinal.GetHashCode(Value);

        /// <inheritdoc />
        public override string ToString() => Value ?? string.Empty;

        /// <summary>Equality operator.</summary>
        public static bool operator ==(ItemId left, ItemId right) => left.Equals(right);

        /// <summary>Inequality operator.</summary>
        public static bool operator !=(ItemId left, ItemId right) => !left.Equals(right);
    }
}
