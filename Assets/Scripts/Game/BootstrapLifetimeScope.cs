using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;
using Whiteout.Core.Randomness;
using Whiteout.Core.Unity;

namespace Whiteout.Game
{
    /// <summary>
    /// Root scope in the Bootstrap scene. Holds services shared by every Game scene load:
    /// <see cref="IEventBus"/>, <see cref="IRandom"/> and <see cref="IMapLayout"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// // Add to a GameObject in the Bootstrap scene, assign the Map Layout asset.
    /// </code>
    /// </example>
    public sealed class BootstrapLifetimeScope : LifetimeScope
    {
        [Tooltip("Random seed. 0 = derive from the current time.")]
        [SerializeField] private int seed;

        [SerializeField] private MapLayoutDefinition mapLayout;

        /// <summary>The seed actually used (resolved from time when the serialized seed is 0). Valid after the scope is built.</summary>
        public int ActiveSeed { get; private set; }

        /// <inheritdoc />
        protected override void Configure(IContainerBuilder builder)
        {
            if (mapLayout == null)
            {
                throw new InvalidOperationException("BootstrapLifetimeScope: Map Layout is not assigned.");
            }

            ActiveSeed = seed != 0 ? seed : DeriveSeedFromTime();

            builder.RegisterInstance<IEventBus>(new EventBus(Debug.LogException));
            builder.RegisterInstance<IRandom>(new SeededRandom(ActiveSeed));
            builder.RegisterInstance<IMapLayout>(mapLayout);
        }

        private static int DeriveSeedFromTime()
        {
            var derived = (int)(DateTime.UtcNow.Ticks & 0x7FFFFFFF);
            return derived != 0 ? derived : 1;
        }
    }
}
