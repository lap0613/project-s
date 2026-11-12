using System;
using System.Collections.Generic;

namespace Whiteout.Core.Stats
{
    /// <summary>
    /// A numeric stat: base value plus modifiers.
    /// Final value = (base + sum(Add)) * max(1 + sum(Multiply), minMultiplier).
    /// Multiply modifiers on the same stat are summed first, then applied once
    /// (+15% and -10% give x1.05). Probability stats are not handled here. Not thread-safe.
    /// </summary>
    /// <example>
    /// <code>
    /// var speed = new Stat(new StatId("player.move_speed"), 4f);
    /// using var boost = speed.AddModifier(new StatModifier(StatModifierKind.Multiply, 0.25f, "augment.boots"));
    /// float v = speed.Value; // 5
    /// </code>
    /// </example>
    public sealed class Stat
    {
        /// <summary>Provisional floor for the multiplier (code-structure.md section 5).</summary>
        public const float DefaultMinMultiplier = 0.1f;

        private readonly List<StatModifier> _modifiers = new List<StatModifier>();
        private readonly float _minMultiplier;

        /// <summary>Creates a stat.</summary>
        /// <param name="id">Stat identifier.</param>
        /// <param name="baseValue">Value before modifiers.</param>
        /// <param name="minMultiplier">Lowest allowed multiplier.</param>
        public Stat(StatId id, float baseValue, float minMultiplier = DefaultMinMultiplier)
        {
            Id = id;
            BaseValue = baseValue;
            _minMultiplier = minMultiplier;
        }

        /// <summary>Stat identifier.</summary>
        public StatId Id { get; }

        /// <summary>Value before modifiers.</summary>
        public float BaseValue { get; set; }

        /// <summary>Active modifiers, in insertion order. Read-only view; do not hold across changes.</summary>
        public IReadOnlyList<StatModifier> Modifiers => _modifiers;

        /// <summary>Final value with all modifiers applied.</summary>
        public float Value
        {
            get
            {
                var add = 0f;
                var multiply = 0f;
                for (var i = 0; i < _modifiers.Count; i++)
                {
                    var modifier = _modifiers[i];
                    if (modifier.Kind == StatModifierKind.Add)
                    {
                        add += modifier.Value;
                    }
                    else
                    {
                        multiply += modifier.Value;
                    }
                }

                return (BaseValue + add) * Math.Max(1f + multiply, _minMultiplier);
            }
        }

        /// <summary>Adds a modifier.</summary>
        /// <param name="modifier">The modifier.</param>
        /// <returns>A handle; dispose it to remove this modifier. Disposing twice is safe.</returns>
        public IDisposable AddModifier(StatModifier modifier)
        {
            if (modifier == null)
            {
                throw new ArgumentNullException(nameof(modifier));
            }

            _modifiers.Add(modifier);
            return new Handle(this, modifier);
        }

        /// <summary>Removes every modifier whose source equals <paramref name="source"/>.</summary>
        /// <param name="source">Source text (ordinal comparison).</param>
        /// <returns>How many modifiers were removed.</returns>
        public int RemoveBySource(string source)
        {
            source = source ?? string.Empty;
            return _modifiers.RemoveAll(m => string.Equals(m.Source, source, StringComparison.Ordinal));
        }

        private sealed class Handle : IDisposable
        {
            private Stat _owner;
            private readonly StatModifier _modifier;

            public Handle(Stat owner, StatModifier modifier)
            {
                _owner = owner;
                _modifier = modifier;
            }

            public void Dispose()
            {
                if (_owner == null)
                {
                    return;
                }

                _owner._modifiers.Remove(_modifier);
                _owner = null;
            }
        }
    }
}
