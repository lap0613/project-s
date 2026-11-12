using System;

namespace Whiteout.Core.Stats
{
    /// <summary>
    /// Immutable change to a <see cref="Stat"/>, with the source that caused it (e.g. an augment id).
    /// </summary>
    /// <example>
    /// <code>
    /// var plus15Percent = new StatModifier(StatModifierKind.Multiply, 0.15f, "augment.fast_boots");
    /// </code>
    /// </example>
    public sealed class StatModifier
    {
        /// <summary>Creates a modifier.</summary>
        /// <param name="kind">Add or Multiply.</param>
        /// <param name="value">Add: amount added to the base. Multiply: fraction, 0.15 means +15%, -0.1 means -10%.</param>
        /// <param name="source">Where the modifier came from; null becomes an empty string.</param>
        public StatModifier(StatModifierKind kind, float value, string source)
        {
            Kind = kind;
            Value = value;
            Source = source ?? string.Empty;
        }

        /// <summary>Add or Multiply.</summary>
        public StatModifierKind Kind { get; }

        /// <summary>The amount; meaning depends on <see cref="Kind"/>.</summary>
        public float Value { get; }

        /// <summary>Origin of this modifier, for tracing why a value changed.</summary>
        public string Source { get; }

        /// <inheritdoc />
        public override string ToString() => $"{Kind} {Value} from '{Source}'";
    }
}
