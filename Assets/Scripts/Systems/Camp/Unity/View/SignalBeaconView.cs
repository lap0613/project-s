using System;
using UnityEngine;
using VContainer;
using Whiteout.Core.Contracts;

namespace Whiteout.Camp.Unity
{
    /// <summary>
    /// The placed signal beacon frame in the scene (core-rules section 1, ui-plan section 8). Registers the beacon
    /// with the rules when placed and shows its state on the model: one part object appears per installed part, and
    /// the requested object (the top light) turns on after rescue is requested. It knows only the object
    /// references, not the model's structure.
    /// </summary>
    /// <example>
    /// <code>
    /// // On the signal beacon prefab root; assign Part Visuals (antenna, battery, transmitter) and Requested Visual.
    /// float distance = beacon.DistanceTo(player);
    /// </code>
    /// </example>
    public sealed class SignalBeaconView : StructureView
    {
        [Tooltip("One object per part, in install order (antenna, battery, transmitter). Shown once that part is installed.")]
        [SerializeField] private GameObject[] partVisuals;

        [Tooltip("Shown after rescue is requested (the light on top).")]
        [SerializeField] private GameObject requestedVisual;

        private SignalBeaconSystem _system;
        private CampRegistry _registry;
        private SignalBeaconDefinition _definition;

        /// <summary>The beacon this view shows; null until placed.</summary>
        public SignalBeacon Beacon { get; private set; }

        /// <summary>The definition the beacon was placed from.</summary>
        public SignalBeaconDefinition Definition => _definition;

        /// <summary>Receives dependencies from the container.</summary>
        /// <param name="system">Beacon rules.</param>
        /// <param name="registry">Tracks placed structures.</param>
        [Inject]
        public void Construct(SignalBeaconSystem system, CampRegistry registry)
        {
            _system = system;
            _registry = registry;
        }

        /// <inheritdoc />
        public override void Initialize(StructureDefinition definition)
        {
            _definition = definition as SignalBeaconDefinition;
            if (_definition == null)
            {
                throw new InvalidOperationException("SignalBeaconView: the prefab must be placed from a SignalBeaconDefinition.");
            }

            var position = transform.position;
            Beacon = _system.Place(position.x, position.y, position.z);
            _registry.SetBeacon(this);
            RefreshVisuals();
        }

        /// <summary>Horizontal distance from the beacon to the player, in meters.</summary>
        /// <param name="player">Player position.</param>
        /// <returns>The distance in meters.</returns>
        public float DistanceTo(IPlayerPosition player)
        {
            var dx = transform.position.x - player.X;
            var dz = transform.position.z - player.Z;
            return Mathf.Sqrt(dx * dx + dz * dz);
        }

        private void Update()
        {
            RefreshVisuals();
        }

        private void RefreshVisuals()
        {
            if (Beacon == null)
            {
                return;
            }

            if (partVisuals != null)
            {
                for (var i = 0; i < partVisuals.Length; i++)
                {
                    var installed = i < Beacon.PartCount && Beacon.IsPartInstalled(i);
                    if (partVisuals[i] != null && partVisuals[i].activeSelf != installed)
                    {
                        partVisuals[i].SetActive(installed);
                    }
                }
            }

            if (requestedVisual != null && requestedVisual.activeSelf != Beacon.RescueRequested)
            {
                requestedVisual.SetActive(Beacon.RescueRequested);
            }
        }

        private void OnDestroy()
        {
            _registry?.ClearBeacon(this);
        }
    }
}
