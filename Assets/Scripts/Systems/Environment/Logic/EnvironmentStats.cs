using Whiteout.Core.Stats;

namespace Whiteout.Environment
{
    /// <summary>Stat ids owned by the Environment system.</summary>
    /// <example>
    /// <code>
    /// environment.AugmentTemperature.AddModifier(new StatModifier(StatModifierKind.Add, 5f, "augment.warm_coat"));
    /// </code>
    /// </example>
    public static class EnvironmentStats
    {
        /// <summary>Temperature bonus (degrees C) from augments; the "augment" term of the ambient temperature.</summary>
        public static readonly StatId AugmentTemperature = new StatId("environment.augment_temperature");
    }
}
