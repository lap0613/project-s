using System;
using Whiteout.Core.Stats;

namespace Whiteout.Gathering
{
    /// <summary>
    /// How long gathering takes (core-rules section 4: E held 1 second). Speed is a <see cref="Stat"/>, so the
    /// hunger penalty (-30%, core-rules section 2), fast hands and gloves attach as modifiers.
    /// </summary>
    /// <example>
    /// <code>
    /// var gather = new GatherRules(settings);
    /// using var slow = gather.Speed.AddModifier(new StatModifier(StatModifierKind.Multiply, -0.3f, "survival.hunger"));
    /// float seconds = gather.HoldSeconds; // 1 / 0.7
    /// </code>
    /// </example>
    public sealed class GatherRules
    {
        private readonly ResourceSettings _settings;

        /// <summary>Creates the rules.</summary>
        /// <param name="settings">Supplies the base time and base speed.</param>
        public GatherRules(ResourceSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Speed = new Stat(ResourceStats.GatherSpeed, settings.BaseGatherSpeed);
        }

        /// <summary>Gather speed stat: attach modifiers here.</summary>
        public Stat Speed { get; }

        /// <summary>Seconds E must be held to gather now; infinite if the speed is 0 or less.</summary>
        public float HoldSeconds => Speed.Value <= 0f ? float.PositiveInfinity : _settings.GatherSeconds / Speed.Value;
    }
}
