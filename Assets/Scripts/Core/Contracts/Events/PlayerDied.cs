namespace Whiteout.Core.Contracts
{
    /// <summary>Published once by Survival when the player's health reaches 0.</summary>
    /// <example>
    /// <code>
    /// using var sub = bus.Subscribe&lt;PlayerDied&gt;(e =&gt; ShowResult(e.Cause));
    /// </code>
    /// </example>
    public readonly struct PlayerDied
    {
        /// <summary>Creates the event.</summary>
        /// <param name="cause">What drained the last health.</param>
        public PlayerDied(DeathCause cause)
        {
            Cause = cause;
        }

        /// <summary>What drained the last health.</summary>
        public DeathCause Cause { get; }
    }
}
