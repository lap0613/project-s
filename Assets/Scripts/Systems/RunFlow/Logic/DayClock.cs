using System;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;

namespace Whiteout.RunFlow
{
    /// <summary>
    /// Day/night clock (design/quick-specs/core-rules.md section 1). A day is a day phase followed by a night phase
    /// (120 s + 60 s by default, from settings). Advances only through <see cref="Tick"/>.
    /// Publishes <see cref="DayStarted"/>, <see cref="NightStarted"/> and <see cref="DayEnded"/>.
    /// At a day boundary <see cref="DayEnded"/> is published first, then <see cref="DayStarted"/> for the next day.
    /// </summary>
    /// <example>
    /// <code>
    /// var clock = new DayClock(new DayClockSettings(120f, 60f), bus);
    /// clock.Start();      // publishes DayStarted(1)
    /// clock.Tick(120f);   // publishes NightStarted(1)
    /// clock.IsPaused = true; // Tick now does nothing
    /// </code>
    /// </example>
    public sealed class DayClock : IDayTimeSource
    {
        private readonly DayClockSettings _settings;
        private readonly IEventBus _bus;

        /// <summary>Creates a clock. Call <see cref="Start"/> to begin day 1.</summary>
        /// <param name="settings">Phase lengths.</param>
        /// <param name="bus">Event channel.</param>
        public DayClock(DayClockSettings settings, IEventBus bus)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        /// <summary>Current day, starting at 1. 0 before <see cref="Start"/>.</summary>
        public int Day { get; private set; }

        /// <summary>Current phase.</summary>
        public DayPhase Phase { get; private set; }

        /// <summary>Seconds spent in the current phase.</summary>
        public float PhaseElapsed { get; private set; }

        /// <inheritdoc />
        public float SecondsIntoDay => Phase == DayPhase.Day ? PhaseElapsed : _settings.DayLength + PhaseElapsed;

        /// <inheritdoc />
        public float DayLength => _settings.DayLength;

        /// <inheritdoc />
        public float NightLength => _settings.NightLength;

        /// <summary>When true, <see cref="Tick"/> does nothing. Setting it from an event handler stops the current Tick at once.</summary>
        public bool IsPaused { get; set; }

        /// <summary>Begins day 1 and publishes <see cref="DayStarted"/>. Does nothing if already started.</summary>
        public void Start()
        {
            if (Day != 0)
            {
                return;
            }

            Day = 1;
            Phase = DayPhase.Day;
            PhaseElapsed = 0f;
            _bus.Publish(new DayStarted(Day));
        }

        /// <summary>
        /// Advances the clock. Does nothing before <see cref="Start"/>, while paused, or for a non-positive,
        /// NaN or infinite <paramref name="dt"/>. A large step may cross several phases; each publishes its event.
        /// </summary>
        /// <param name="dt">Seconds to advance.</param>
        public void Tick(float dt)
        {
            if (Day == 0 || IsPaused || !(dt > 0f) || float.IsInfinity(dt))
            {
                return;
            }

            var remaining = dt;
            while (true)
            {
                var length = Phase == DayPhase.Day ? _settings.DayLength : _settings.NightLength;
                var space = length - PhaseElapsed;
                if (remaining < space)
                {
                    PhaseElapsed += remaining;
                    return;
                }

                remaining -= space;
                PhaseElapsed = 0f;
                if (Phase == DayPhase.Day)
                {
                    Phase = DayPhase.Night;
                    _bus.Publish(new NightStarted(Day));
                }
                else
                {
                    _bus.Publish(new DayEnded(Day));
                    Day++;
                    Phase = DayPhase.Day;
                    _bus.Publish(new DayStarted(Day));
                }

                if (IsPaused)
                {
                    return;
                }
            }
        }
    }
}
