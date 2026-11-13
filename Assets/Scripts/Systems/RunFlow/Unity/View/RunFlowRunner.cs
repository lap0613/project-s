using System;
using UnityEngine;
using VContainer.Unity;

namespace Whiteout.RunFlow.Unity
{
    /// <summary>
    /// Drives the RunFlow logic from Unity's frame loop: starts the clock on the first frame, then advances the
    /// clock and the rescue timer every frame. The rescue timer stands still while the clock is paused, and both
    /// stop once the run has ended (tech-decisions 34, R5). Taking the part schedule and the run outcome here makes
    /// the container create them before the first <c>DayStarted</c> goes out.
    /// Registered as a VContainer entry point.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.RegisterEntryPoint&lt;RunFlowRunner&gt;();
    /// </code>
    /// </example>
    public sealed class RunFlowRunner : IStartable, ITickable
    {
        private readonly DayClock _clock;
        private readonly RescueTimer _rescueTimer;
        private readonly RunOutcome _outcome;

        /// <summary>Creates the runner.</summary>
        /// <param name="clock">The day/night clock to drive.</param>
        /// <param name="rescueTimer">The rescue timer to drive.</param>
        /// <param name="partSchedule">The part schedule; only needs to exist before the clock starts.</param>
        /// <param name="outcome">Tells when the run has ended.</param>
        public RunFlowRunner(DayClock clock, RescueTimer rescueTimer, PartSchedule partSchedule, RunOutcome outcome)
        {
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _rescueTimer = rescueTimer ?? throw new ArgumentNullException(nameof(rescueTimer));
            _outcome = outcome ?? throw new ArgumentNullException(nameof(outcome));
            if (partSchedule == null)
            {
                throw new ArgumentNullException(nameof(partSchedule));
            }
        }

        /// <inheritdoc />
        public void Start()
        {
            _clock.Start();
        }

        /// <inheritdoc />
        public void Tick()
        {
            if (_outcome.HasEnded)
            {
                return;
            }

            _clock.Tick(Time.deltaTime);
            if (!_clock.IsPaused)
            {
                _rescueTimer.Tick(Time.deltaTime);
            }
        }
    }
}
