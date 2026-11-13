namespace Whiteout.Core.Contracts
{
    /// <summary>Published by RunFlow when the night phase of a day begins.</summary>
    /// <example>
    /// <code>
    /// bus.Publish(new NightStarted(1));
    /// </code>
    /// </example>
    public readonly struct NightStarted
    {
        /// <summary>Creates the event.</summary>
        /// <param name="day">Day number, starting at 1.</param>
        public NightStarted(int day)
        {
            Day = day;
        }

        /// <summary>Day number, starting at 1.</summary>
        public int Day { get; }
    }
}
