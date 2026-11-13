namespace Whiteout.Core.Contracts
{
    /// <summary>Published by Environment each morning with that day's weather.</summary>
    /// <example>
    /// <code>
    /// using var sub = bus.Subscribe&lt;WeatherChanged&gt;(e =&gt; fogOn = e.Weather.HasFog);
    /// </code>
    /// </example>
    public readonly struct WeatherChanged
    {
        /// <summary>Creates the event.</summary>
        /// <param name="day">Day number the weather applies to.</param>
        /// <param name="weather">Today's weather.</param>
        public WeatherChanged(int day, WeatherInfo weather)
        {
            Day = day;
            Weather = weather;
        }

        /// <summary>Day number the weather applies to.</summary>
        public int Day { get; }

        /// <summary>Today's weather.</summary>
        public WeatherInfo Weather { get; }
    }
}
