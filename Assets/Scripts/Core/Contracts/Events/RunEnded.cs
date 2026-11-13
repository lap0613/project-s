namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// Published once by RunFlow when the run ends, by death or by rescue, whichever happens first
    /// (docs/architecture/tech-decisions.md 34, R5). Everything that must stop at the end of a run listens to this,
    /// not to <see cref="PlayerDied"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// using var sub = bus.Subscribe&lt;RunEnded&gt;(e =&gt; ShowResult(e.Rescued, e.Cause, e.Day));
    /// </code>
    /// </example>
    public readonly struct RunEnded
    {
        /// <summary>Creates the event.</summary>
        /// <param name="rescued">True when the player was rescued, false when the player died.</param>
        /// <param name="cause">Why the player died. Meaningful only when <paramref name="rescued"/> is false.</param>
        /// <param name="day">The day number on which the run ended (starts at 1).</param>
        public RunEnded(bool rescued, DeathCause cause, int day)
        {
            Rescued = rescued;
            Cause = cause;
            Day = day;
        }

        /// <summary>True when the player was rescued, false when the player died.</summary>
        public bool Rescued { get; }

        /// <summary>Why the player died. Meaningful only when <see cref="Rescued"/> is false.</summary>
        public DeathCause Cause { get; }

        /// <summary>The day number on which the run ended (starts at 1).</summary>
        public int Day { get; }
    }
}
