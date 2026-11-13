using System;
using UnityEngine;
using VContainer;
using Whiteout.Core.Contracts;

namespace Whiteout.Camp.Unity
{
    /// <summary>
    /// A placed campfire in the scene. Registers its <see cref="Campfire"/> state with <see cref="CampSystem"/>,
    /// measures the distance to the player for it, and shows the flame object only while lit.
    /// </summary>
    /// <example>
    /// <code>
    /// // On the campfire prefab root; assign Lit Visual (flame mesh + light). No collider on the mask used by placement
    /// // is needed unless campfires should block other placements.
    /// </code>
    /// </example>
    public sealed class CampfireView : StructureView
    {
        [Tooltip("Shown only while the fire burns (flame, light).")]
        [SerializeField] private GameObject litVisual;

        private CampSystem _camp;
        private CampRegistry _registry;
        private CampfireDefinition _definition;

        /// <summary>Logic state of this fire; null before <see cref="Initialize"/>.</summary>
        public Campfire Fire { get; private set; }

        /// <summary>The definition this fire was placed from.</summary>
        public CampfireDefinition Definition => _definition;

        /// <summary>Receives dependencies from the container.</summary>
        /// <param name="camp">Camp rules.</param>
        /// <param name="registry">Tracks placed structures.</param>
        [Inject]
        public void Construct(CampSystem camp, CampRegistry registry)
        {
            _camp = camp;
            _registry = registry;
        }

        /// <inheritdoc />
        public override void Initialize(StructureDefinition definition)
        {
            _definition = definition as CampfireDefinition;
            if (_definition == null)
            {
                throw new InvalidOperationException("CampfireView: the prefab must be placed from a CampfireDefinition.");
            }

            var position = transform.position;
            Fire = _camp.AddCampfire(_definition.ToCampfireSpec(), position.x, position.y, position.z);
            _registry.Add(this);
            RefreshVisual();
        }

        /// <summary>Horizontal distance from this fire to the player, in meters.</summary>
        /// <param name="player">Player position.</param>
        public float DistanceTo(IPlayerPosition player)
        {
            var dx = transform.position.x - player.X;
            var dz = transform.position.z - player.Z;
            return Mathf.Sqrt(dx * dx + dz * dz);
        }

        private void Update()
        {
            RefreshVisual();
        }

        private void RefreshVisual()
        {
            if (litVisual != null && Fire != null && litVisual.activeSelf != Fire.IsLit)
            {
                litVisual.SetActive(Fire.IsLit);
            }
        }

        private void OnDestroy()
        {
            _registry?.Remove(this);
        }
    }
}
