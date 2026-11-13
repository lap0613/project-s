namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// Lets items change the hurdle gauges without Resources knowing Survival. Implemented by Survival.
    /// </summary>
    /// <example>
    /// <code>
    /// vitals.Restore(VitalKind.Hunger, 15f);
    /// </code>
    /// </example>
    public interface IVitalsReceiver
    {
        /// <summary>Raises a gauge, keeping it within its maximum. Does nothing once the player is dead.</summary>
        /// <param name="kind">Which gauge.</param>
        /// <param name="amount">How much to add.</param>
        void Restore(VitalKind kind, float amount);
    }
}
