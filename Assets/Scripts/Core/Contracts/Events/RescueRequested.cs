namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// Published by Camp when the player requests rescue at the finished signal beacon. RunFlow starts the rescue
    /// timer, UI shows the arrival flag (design/quick-specs/core-rules.md section 1).
    /// </summary>
    /// <example>
    /// <code>
    /// bus.Publish(new RescueRequested(360f));
    /// </code>
    /// </example>
    public readonly struct RescueRequested
    {
        /// <summary>Creates the event.</summary>
        /// <param name="arrivalDelaySeconds">Game seconds from the request until rescue arrives.</param>
        public RescueRequested(float arrivalDelaySeconds)
        {
            ArrivalDelaySeconds = arrivalDelaySeconds;
        }

        /// <summary>Game seconds from the request until rescue arrives.</summary>
        public float ArrivalDelaySeconds { get; }
    }
}
