namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// Published by Environment each morning with the three-day forecast.
    /// The forecast always equals the weather that later becomes actual.
    /// </summary>
    /// <example>
    /// <code>
    /// using var sub = bus.Subscribe&lt;WeatherForecastUpdated&gt;(e =&gt; label = e.Tomorrow.DisplayName);
    /// </code>
    /// </example>
    public readonly struct WeatherForecastUpdated
    {
        /// <summary>Creates the event.</summary>
        /// <param name="day">Day number of "today".</param>
        /// <param name="today">Weather of day <paramref name="day"/>.</param>
        /// <param name="tomorrow">Weather of the next day.</param>
        /// <param name="dayAfter">Weather of the day after tomorrow.</param>
        public WeatherForecastUpdated(int day, WeatherInfo today, WeatherInfo tomorrow, WeatherInfo dayAfter)
        {
            Day = day;
            Today = today;
            Tomorrow = tomorrow;
            DayAfter = dayAfter;
        }

        /// <summary>Day number of "today".</summary>
        public int Day { get; }

        /// <summary>Today's weather.</summary>
        public WeatherInfo Today { get; }

        /// <summary>Tomorrow's weather.</summary>
        public WeatherInfo Tomorrow { get; }

        /// <summary>The weather of the day after tomorrow.</summary>
        public WeatherInfo DayAfter { get; }
    }
}
