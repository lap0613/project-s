using System;
using UnityEngine;
using VContainer.Unity;
using Whiteout.Core.Contracts;

namespace Whiteout.Camp.Unity
{
    /// <summary>
    /// Drives <see cref="CampSystem"/> from the frame loop. Before each tick it measures every campfire's distance
    /// to the player and hands Logic only the results: inside the warmth radius, close enough to show status
    /// (tech-decisions item 13). Registered as a VContainer entry point.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.RegisterEntryPoint&lt;CampRunner&gt;();
    /// </code>
    /// </example>
    public sealed class CampRunner : ITickable
    {
        private readonly CampSystem _camp;
        private readonly CampRegistry _registry;
        private readonly IPlayerPosition _player;

        /// <summary>Creates the runner.</summary>
        /// <param name="camp">The system to drive.</param>
        /// <param name="registry">Placed campfires.</param>
        /// <param name="player">Player position.</param>
        public CampRunner(CampSystem camp, CampRegistry registry, IPlayerPosition player)
        {
            _camp = camp ?? throw new ArgumentNullException(nameof(camp));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _player = player ?? throw new ArgumentNullException(nameof(player));
        }

        /// <inheritdoc />
        public void Tick()
        {
            var campfires = _registry.Campfires;
            for (var i = 0; i < campfires.Count; i++)
            {
                var view = campfires[i];
                var distance = view.DistanceTo(_player);
                view.Fire.SetPlayerProximity(
                    distance <= view.Definition.WarmthRadius, distance <= view.Definition.StatusRange, distance);
            }

            _camp.Tick(Time.deltaTime);
        }
    }
}
