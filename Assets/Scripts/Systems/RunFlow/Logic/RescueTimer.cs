using System;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;

namespace Whiteout.RunFlow
{
    /// <summary>
    /// The rescue timer (core-rules section 1, tech-decisions 34). The first <see cref="RescueRequested"/> starts
    /// it with the event's delay; later requests are ignored. <see cref="Tick"/> counts it down and publishes
    /// <see cref="RescueArrived"/> once when it reaches zero. The caller does not tick it while the clock is paused.
    /// </summary>
    /// <example>
    /// <code>
    /// using var timer = new RescueTimer(bus);
    /// bus.Publish(new RescueRequested(360f));
    /// timer.Tick(0.016f);
    /// float left = timer.RemainingSeconds;
    /// </code>
    /// </example>
    public sealed class RescueTimer : IRescueCountdown, IDisposable
    {
        private readonly IEventBus _bus;
        private readonly IDisposable _requestSubscription;
        private bool _arrived;

        /// <summary>Creates the timer, not running, and subscribes to <see cref="RescueRequested"/>.</summary>
        /// <param name="bus">Event channel.</param>
        public RescueTimer(IEventBus bus)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _requestSubscription = bus.Subscribe<RescueRequested>(OnRequested);
        }

        /// <inheritdoc />
        public bool IsActive { get; private set; }

        /// <inheritdoc />
        public float RemainingSeconds { get; private set; }

        /// <summary>Counts the timer down; publishes <see cref="RescueArrived"/> once when it reaches zero.</summary>
        /// <param name="dt">Game seconds since the last tick.</param>
        public void Tick(float dt)
        {
            if (!IsActive || _arrived || !(dt > 0f) || float.IsInfinity(dt))
            {
                return;
            }

            RemainingSeconds = Math.Max(RemainingSeconds - dt, 0f);
            if (RemainingSeconds <= 0f)
            {
                _arrived = true;
                _bus.Publish(new RescueArrived());
            }
        }

        /// <summary>Unsubscribes.</summary>
        public void Dispose()
        {
            _requestSubscription.Dispose();
        }

        private void OnRequested(RescueRequested request)
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;
            RemainingSeconds = Math.Max(request.ArrivalDelaySeconds, 0f);
        }
    }
}
