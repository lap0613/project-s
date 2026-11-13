using System;
using UnityEngine;
using VContainer.Unity;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;

namespace Whiteout.Survival.Unity
{
    /// <summary>
    /// Drives <see cref="SurvivalSystem"/> from the frame loop until the run ends (<see cref="RunEnded"/>, by death
    /// or by rescue; tech-decisions 34, R5). Registered as a VContainer entry point, so no scene object is needed.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Register&lt;SurvivalSystem&gt;(Lifetime.Singleton);
    /// builder.RegisterEntryPoint&lt;SurvivalRunner&gt;();
    /// </code>
    /// </example>
    public sealed class SurvivalRunner : ITickable, IDisposable
    {
        private readonly SurvivalSystem _survival;
        private readonly IDisposable _endedSubscription;
        private bool _runEnded;

        /// <summary>Creates the runner and subscribes to <see cref="RunEnded"/>.</summary>
        /// <param name="survival">The system to drive.</param>
        /// <param name="bus">Event channel.</param>
        public SurvivalRunner(SurvivalSystem survival, IEventBus bus)
        {
            _survival = survival ?? throw new ArgumentNullException(nameof(survival));
            if (bus == null)
            {
                throw new ArgumentNullException(nameof(bus));
            }

            _endedSubscription = bus.Subscribe<RunEnded>(_ => _runEnded = true);
        }

        /// <inheritdoc />
        public void Tick()
        {
            if (_runEnded)
            {
                return;
            }

            _survival.Tick(Time.deltaTime);
        }

        /// <summary>Unsubscribes.</summary>
        public void Dispose()
        {
            _endedSubscription.Dispose();
        }
    }
}
