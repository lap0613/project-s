using Whiteout.Core.Contracts;

namespace Whiteout.Resources
{
    /// <summary>
    /// What using a consumable does (design/quick-specs/items.md section 3). A new kind of effect is one new class
    /// plus a data-side definition (code-structure section 6).
    /// </summary>
    /// <example>
    /// <code>
    /// IUseEffect effect = new RestoreVitalEffect(VitalKind.Hunger, 15f);
    /// effect.Apply(vitals);
    /// </code>
    /// </example>
    public interface IUseEffect
    {
        /// <summary>Applies the effect to the player.</summary>
        /// <param name="vitals">Target gauges.</param>
        void Apply(IVitalsReceiver vitals);
    }
}
