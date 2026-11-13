namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// Read-only view of the continuous game time of day. Implemented by RunFlow, read by UI (the forecast timeline).
    /// The time stands still while the clock is paused, so a reader that derives its picture from these values
    /// stops with it (design/quick-specs/core-rules.md section 1).
    /// </summary>
    /// <example>
    /// <code>
    /// float elapsed = (time.Day - 1) * (time.DayLength + time.NightLength) + time.SecondsIntoDay;
    /// </code>
    /// </example>
    public interface IDayTimeSource
    {
        /// <summary>Current day, starting at 1. 0 before the first day starts.</summary>
        int Day { get; }

        /// <summary>Game seconds since the current day began (0 up to DayLength + NightLength).</summary>
        float SecondsIntoDay { get; }

        /// <summary>Length of the day phase in game seconds.</summary>
        float DayLength { get; }

        /// <summary>Length of the night phase in game seconds.</summary>
        float NightLength { get; }
    }
}
