using System;
using UnityEngine;
using VContainer.Unity;

namespace Whiteout.Survival.Unity
{
    /// <summary>
    /// Drives <see cref="SurvivalSystem"/> from the frame loop. Registered as a VContainer entry point,
    /// so no scene object is needed.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Register&lt;SurvivalSystem&gt;(Lifetime.Singleton);
    /// builder.RegisterEntryPoint&lt;SurvivalRunner&gt;();
    /// </code>
    /// </example>
    public sealed class SurvivalRunner : ITickable
    {
        private readonly SurvivalSystem _survival;

        /// <summary>Creates the runner.</summary>
        /// <param name="survival">The system to drive.</param>
        public SurvivalRunner(SurvivalSystem survival)
        {
            _survival = survival ?? throw new ArgumentNullException(nameof(survival));
        }

        /// <inheritdoc />
        public void Tick()
        {
            _survival.Tick(Time.deltaTime);
        }
    }
}
