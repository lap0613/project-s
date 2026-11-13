using System;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;

namespace Whiteout.RunFlow
{
    /// <summary>
    /// Decides how the run ends (core-rules section 1, tech-decisions 34, R5 and R6). Of <see cref="PlayerDied"/>
    /// and <see cref="RescueArrived"/> only the first one counts: it publishes <see cref="RunEnded"/> with the day
    /// number of that moment; anything after it is ignored.
    /// </summary>
    /// <example>
    /// <code>
    /// using var outcome = new RunOutcome(bus, clock);
    /// bus.Publish(new RescueArrived()); // RunEnded(rescued: true, ...) goes out
    /// bool ended = outcome.HasEnded;
    /// </code>
    /// </example>
    public sealed class RunOutcome : IDisposable
    {
        private readonly IEventBus _bus;
        private readonly IDayTimeSource _clock;
        private readonly IDisposable _diedSubscription;
        private readonly IDisposable _rescuedSubscription;

        /// <summary>Creates the outcome and subscribes to <see cref="PlayerDied"/> and <see cref="RescueArrived"/>.</summary>
        /// <param name="bus">Event channel.</param>
        /// <param name="clock">The current day number.</param>
        public RunOutcome(IEventBus bus, IDayTimeSource clock)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _diedSubscription = bus.Subscribe<PlayerDied>(e => End(false, e.Cause));
            _rescuedSubscription = bus.Subscribe<RescueArrived>(_ => End(true, default));
        }

        /// <summary>True once the run has ended.</summary>
        public bool HasEnded { get; private set; }

        /// <summary>Unsubscribes.</summary>
        public void Dispose()
        {
            _diedSubscription.Dispose();
            _rescuedSubscription.Dispose();
        }

        private void End(bool rescued, DeathCause cause)
        {
            if (HasEnded)
            {
                return;
            }

            HasEnded = true;
            _bus.Publish(new RunEnded(rescued, cause, _clock.Day));
        }
    }
}
