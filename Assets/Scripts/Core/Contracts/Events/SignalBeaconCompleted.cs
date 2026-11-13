namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// Published by Camp when the last part is installed on the signal beacon (design/quick-specs/core-rules.md
    /// section 1). It carries no data: there is one beacon per run.
    /// </summary>
    /// <example>
    /// <code>
    /// bus.Publish(new SignalBeaconCompleted());
    /// </code>
    /// </example>
    public readonly struct SignalBeaconCompleted
    {
    }
}
