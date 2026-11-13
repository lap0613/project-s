using System;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;

namespace Whiteout.RunFlow
{
    /// <summary>
    /// Makes the beacon parts appear (core-rules sections 1 and 5, tech-decisions 34, R2). On each
    /// <see cref="DayStarted"/> it publishes <see cref="BeaconPartAppeared"/> for every schedule line of that day,
    /// each line once. Where the pickup goes is decided by the subscriber (Gathering).
    /// </summary>
    /// <example>
    /// <code>
    /// using var schedule = new PartSchedule(settings, bus);
    /// bus.Publish(new DayStarted(2)); // BeaconPartAppeared(antenna, Middle) goes out
    /// </code>
    /// </example>
    public sealed class PartSchedule : IDisposable
    {
        private readonly PartScheduleSettings _settings;
        private readonly IEventBus _bus;
        private readonly bool[] _published;
        private readonly IDisposable _daySubscription;

        /// <summary>Creates the schedule and subscribes to <see cref="DayStarted"/>.</summary>
        /// <param name="settings">The part appearances of a run.</param>
        /// <param name="bus">Event channel.</param>
        public PartSchedule(PartScheduleSettings settings, IEventBus bus)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _published = new bool[settings.Appearances.Count];
            _daySubscription = bus.Subscribe<DayStarted>(OnDayStarted);
        }

        /// <summary>Unsubscribes.</summary>
        public void Dispose()
        {
            _daySubscription.Dispose();
        }

        private void OnDayStarted(DayStarted started)
        {
            var appearances = _settings.Appearances;
            for (var i = 0; i < appearances.Count; i++)
            {
                if (_published[i] || appearances[i].Day != started.Day)
                {
                    continue;
                }

                _published[i] = true;
                _bus.Publish(new BeaconPartAppeared(appearances[i].Part, appearances[i].Ring));
            }
        }
    }
}
