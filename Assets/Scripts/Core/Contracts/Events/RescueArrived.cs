namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// Published by RunFlow when the rescue timer reaches zero (design/quick-specs/core-rules.md section 1). The run
    /// outcome turns it into <see cref="RunEnded"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// bus.Publish(new RescueArrived());
    /// </code>
    /// </example>
    public readonly struct RescueArrived
    {
    }
}
