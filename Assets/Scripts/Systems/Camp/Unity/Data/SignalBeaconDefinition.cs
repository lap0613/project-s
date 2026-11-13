using System;
using System.Collections.Generic;
using UnityEngine;
using Whiteout.Core.Contracts;
using Whiteout.Core.Unity;

namespace Whiteout.Camp.Unity
{
    /// <summary>
    /// Signal beacon frame data (core-rules section 1, items.md section 7, tech-decisions 34): cost wood 4 + metal
    /// scrap 2, one per run (Max Count 1), three parts installed one by one with E, then E requests rescue, which
    /// arrives after the wait. Must not reference View code.
    /// </summary>
    /// <example>
    /// <code>
    /// // Assets > Create > Whiteout > Structure > Signal Beacon
    /// SignalBeaconSpec spec = definition.ToBeaconSpec();
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/Structure/Signal Beacon", fileName = "Structure_SignalBeacon")]
    public sealed class SignalBeaconDefinition : StructureDefinition
    {
        [Header("Parts (core-rules section 1)")]
        [Tooltip("The parts the beacon needs, in install order: antenna, battery, transmitter.")]
        [SerializeField] private ItemIdAsset[] parts;

        [Header("Rescue (core-rules section 1)")]
        [Tooltip("Days (day + night) from the rescue request until rescue arrives (core-rules section 1: 2).")]
        [SerializeField] private float rescueWaitDays = 2f;

        [Header("Interaction")]
        [Tooltip("Provisional. How close the player must be to install a part or request rescue, in meters.")]
        [SerializeField] private float interactRange = 2f;

        [Tooltip("Provisional. Seconds to hold E to install a part or request rescue.")]
        [SerializeField] private float actionHoldSeconds = 0.5f;

        [Tooltip("Provisional. E-action priority of the beacon; the highest wins when several actions are possible (cook 40, fuel 30).")]
        [SerializeField] private int interactPriority = 35;

        /// <summary>Install/request interaction range in meters.</summary>
        public float InteractRange => interactRange;

        /// <summary>Seconds to hold E to install a part or request rescue.</summary>
        public float ActionHoldSeconds => actionHoldSeconds;

        /// <summary>E-action priority of the beacon.</summary>
        public int InteractPriority => interactPriority;

        /// <summary>Converts to the beacon spec the Logic uses.</summary>
        /// <returns>Parts in install order and the rescue wait.</returns>
        /// <exception cref="InvalidOperationException">No parts are assigned, or one of them is empty.</exception>
        public SignalBeaconSpec ToBeaconSpec()
        {
            if (parts == null || parts.Length == 0)
            {
                throw new InvalidOperationException($"SignalBeaconDefinition '{name}': Parts must be assigned.");
            }

            var ids = new List<ItemId>(parts.Length);
            for (var i = 0; i < parts.Length; i++)
            {
                if (parts[i] == null)
                {
                    throw new InvalidOperationException($"SignalBeaconDefinition '{name}': Parts element {i} is not assigned.");
                }

                ids.Add(parts[i].Id);
            }

            return new SignalBeaconSpec(ids, Mathf.Max(rescueWaitDays, 0f));
        }

        private void OnValidate()
        {
            rescueWaitDays = Mathf.Max(rescueWaitDays, 0f);
            interactRange = Mathf.Max(interactRange, 0f);
            actionHoldSeconds = Mathf.Max(actionHoldSeconds, 0f);
        }
    }
}
