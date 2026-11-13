namespace Whiteout.Core.Contracts
{
    /// <summary>Published by RunFlow when a full day (day and night) has ended, just before the next DayStarted.</summary>
    /// <example>
    /// <code>
    /// bus.Publish(new DayEnded(1));
    /// </code>
    /// </example>
    public readonly struct DayEnded
    {
        /// <summary>Creates the event.</summary>
        /// <param name="day">Day number, starting at 1.</param>
        public DayEnded(int day)
        {
            Day = day;
        }

        /// <summary>Day number, starting at 1.</summary>
        public int Day { get; }
    }
}
