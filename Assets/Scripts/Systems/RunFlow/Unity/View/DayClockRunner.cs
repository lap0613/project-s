using System;
using UnityEngine;
using VContainer.Unity;

namespace Whiteout.RunFlow.Unity
{
    /// <summary>
    /// Drives <see cref="DayClock"/> from the frame loop: starts day 1 and ticks with the frame time.
    /// Registered as a VContainer entry point, so no scene object is needed.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Register&lt;DayClock&gt;(Lifetime.Singleton);
    /// builder.RegisterEntryPoint&lt;DayClockRunner&gt;();
    /// </code>
    /// </example>
    public sealed class DayClockRunner : IStartable, ITickable
    {
        private readonly DayClock _clock;

        /// <summary>Creates the runner.</summary>
        /// <param name="clock">The clock to drive.</param>
        public DayClockRunner(DayClock clock)
        {
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        /// <inheritdoc />
        public void Start()
        {
            _clock.Start();
        }

        /// <inheritdoc />
        public void Tick()
        {
            _clock.Tick(Time.deltaTime);
        }
    }
}
