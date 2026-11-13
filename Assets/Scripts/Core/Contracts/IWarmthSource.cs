namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// Warmth the player gets from fire. Implemented by Camp, read by Environment.
    /// Only the single warmest lit fire counts; overlapping fires are not summed (core-rules section 3).
    /// </summary>
    /// <example>
    /// <code>
    /// float warmth = warmthSource.CurrentWarmth; // 35 inside a lit campfire's radius, otherwise 0
    /// </code>
    /// </example>
    public interface IWarmthSource
    {
        /// <summary>Temperature bonus in degrees C from the warmest lit fire at the player; 0 when none.</summary>
        float CurrentWarmth { get; }
    }
}
