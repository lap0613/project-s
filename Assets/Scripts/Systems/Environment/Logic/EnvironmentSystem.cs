using System;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;
using Whiteout.Core.Randomness;
using Whiteout.Core.Stats;

namespace Whiteout.Environment
{
    /// <summary>
    /// Weather forecast and ambient temperature (design/quick-specs/core-rules.md section 3).
    /// Weather is drawn once per morning with <see cref="IRandom"/>, independent of the day number.
    /// Today, tomorrow and the day after are drawn ahead, so the forecast always equals the actual weather.
    /// On each <see cref="DayStarted"/> after the first the forecast moves one day and one new day is drawn;
    /// then <see cref="WeatherChanged"/> and <see cref="WeatherForecastUpdated"/> are published (in that order).
    /// Ambient temperature = base + weather + ring + campfire warmth + augment bonus.
    /// There are no location zones: place only matters through the ring passed to <see cref="SetPlayerRing"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// var env = new EnvironmentSystem(settings, random, bus, warmth);
    /// env.SetPlayerRing(MapRing.Middle);
    /// bus.Publish(new DayStarted(1));
    /// float t = env.CurrentTemperature; // -10 + weather + (-5) + warmth + augment
    /// </code>
    /// </example>
    public sealed class EnvironmentSystem : IAmbientTemperatureSource, IDisposable
    {
        private const int ForecastDays = 3;

        private readonly EnvironmentSettings _settings;
        private readonly IRandom _random;
        private readonly IEventBus _bus;
        private readonly IWarmthSource _warmth;
        private readonly WeatherInfo[] _forecast = new WeatherInfo[ForecastDays];
        private readonly Stat _augmentTemperature = new Stat(EnvironmentStats.AugmentTemperature, 0f);
        private readonly IDisposable _dayStartedSubscription;
        private MapRing _ring = MapRing.Inner;
        private int _day;

        /// <summary>Creates the system, draws the first three days and subscribes to <see cref="DayStarted"/>.</summary>
        /// <param name="settings">Temperature values and the weather list.</param>
        /// <param name="random">Random source for weather draws.</param>
        /// <param name="bus">Event channel.</param>
        /// <param name="warmth">Fire warmth at the player.</param>
        public EnvironmentSystem(EnvironmentSettings settings, IRandom random, IEventBus bus, IWarmthSource warmth)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _warmth = warmth ?? throw new ArgumentNullException(nameof(warmth));

            for (var i = 0; i < ForecastDays; i++)
            {
                _forecast[i] = DrawWeather();
            }

            _dayStartedSubscription = _bus.Subscribe<DayStarted>(OnDayStarted);
        }

        /// <summary>
        /// Slot for augment temperature bonuses: attach Add modifiers (degrees C). Multiply modifiers have no
        /// effect because the base value is 0.
        /// </summary>
        public Stat AugmentTemperature => _augmentTemperature;

        /// <inheritdoc />
        public float CurrentTemperature =>
            _settings.BaseTemperature
            + _forecast[0].TemperatureOffset
            + RingOffset(_ring)
            + _warmth.CurrentWarmth
            + _augmentTemperature.Value;

        /// <summary>
        /// Sets the ring the player stands in; the View computes it from the player position via IMapLayout.
        /// <see cref="MapRing.OutOfMap"/> uses the Outer ring offset.
        /// </summary>
        /// <param name="ring">The player's current ring.</param>
        public void SetPlayerRing(MapRing ring)
        {
            _ring = ring;
        }

        /// <summary>Stops listening for <see cref="DayStarted"/>.</summary>
        public void Dispose()
        {
            _dayStartedSubscription.Dispose();
        }

        private void OnDayStarted(DayStarted e)
        {
            if (e.Day <= _day)
            {
                return;
            }

            if (_day != 0)
            {
                _forecast[0] = _forecast[1];
                _forecast[1] = _forecast[2];
                _forecast[2] = DrawWeather();
            }

            _day = e.Day;
            _bus.Publish(new WeatherChanged(_day, _forecast[0]));
            _bus.Publish(new WeatherForecastUpdated(_day, _forecast[0], _forecast[1], _forecast[2]));
        }

        private WeatherInfo DrawWeather()
        {
            var weathers = _settings.Weathers;
            var roll = _random.NextFloat() * _settings.TotalWeight;
            var cumulative = 0f;
            var last = weathers[0];
            for (var i = 0; i < weathers.Count; i++)
            {
                if (weathers[i].Weight <= 0f)
                {
                    continue;
                }

                last = weathers[i];
                cumulative += weathers[i].Weight;
                if (roll < cumulative)
                {
                    return weathers[i].Info;
                }
            }

            return last.Info;
        }

        private float RingOffset(MapRing ring)
        {
            switch (ring)
            {
                case MapRing.Inner:
                    return _settings.InnerRingOffset;
                case MapRing.Middle:
                    return _settings.MiddleRingOffset;
                default:
                    return _settings.OuterRingOffset;
            }
        }
    }
}
