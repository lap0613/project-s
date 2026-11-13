using System;
using UnityEngine;
using VContainer;
using Whiteout.Core.Contracts;

namespace Whiteout.Camp.Unity
{
    /// <summary>
    /// A placed shelter in the scene (core-rules section 4: radius 3 m, rest only, no warmth). Tells whether the
    /// player is inside its radius; the rest E-action uses that.
    /// </summary>
    /// <example>
    /// <code>
    /// // On the shelter prefab root.
    /// bool inside = shelter.Contains(player);
    /// </code>
    /// </example>
    public sealed class ShelterView : StructureView
    {
        private CampRegistry _registry;
        private ShelterDefinition _definition;

        /// <summary>Receives dependencies from the container.</summary>
        /// <param name="registry">Tracks placed structures.</param>
        [Inject]
        public void Construct(CampRegistry registry)
        {
            _registry = registry;
        }

        /// <inheritdoc />
        public override void Initialize(StructureDefinition definition)
        {
            _definition = definition as ShelterDefinition;
            if (_definition == null)
            {
                throw new InvalidOperationException("ShelterView: the prefab must be placed from a ShelterDefinition.");
            }

            _registry.Add(this);
        }

        /// <summary>True when the player stands inside the shelter's radius (horizontal distance).</summary>
        /// <param name="player">Player position.</param>
        public bool Contains(IPlayerPosition player)
        {
            var dx = transform.position.x - player.X;
            var dz = transform.position.z - player.Z;
            return dx * dx + dz * dz <= _definition.Radius * _definition.Radius;
        }

        private void OnDestroy()
        {
            _registry?.Remove(this);
        }
    }
}
