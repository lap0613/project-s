namespace Whiteout.Core.Contracts
{
    /// <summary>Why the player died (design/quick-specs/core-rules.md section 2).</summary>
    public enum DeathCause
    {
        /// <summary>Body temperature drained the health (동사).</summary>
        Freezing,

        /// <summary>Hunger drained the health (아사).</summary>
        Starvation,
    }
}
