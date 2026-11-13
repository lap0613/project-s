namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// The player took a hit (an attack, not the slow cold or hunger drain). Cancels a click walk (ui-plan section 8.1, M4).
    /// Nothing publishes it yet: the combat story (story-007) does.
    /// </summary>
    /// <example>
    /// <code>
    /// bus.Publish(new PlayerDamaged(8f));
    /// </code>
    /// </example>
    public readonly struct PlayerDamaged
    {
        /// <summary>Creates the event.</summary>
        /// <param name="amount">Health lost.</param>
        public PlayerDamaged(float amount)
        {
            Amount = amount;
        }

        /// <summary>Health lost.</summary>
        public float Amount { get; }
    }
}
