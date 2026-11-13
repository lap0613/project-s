using System.Collections.Generic;
using UnityEngine;

namespace Whiteout.Resources.Unity
{
    /// <summary>
    /// The pickups currently on the map, so the gather action can find the nearest one without searching the scene
    /// (code-structure section 7). Distances are horizontal (x, z).
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Register&lt;PickupRegistry&gt;(Lifetime.Singleton);
    /// if (registry.TryFindNearest(x, z, 2f, out var pickup)) { /* gather it */ }
    /// </code>
    /// </example>
    public sealed class PickupRegistry
    {
        private readonly List<ResourcePickup> _pickups = new List<ResourcePickup>();

        /// <summary>Starts tracking a pickup.</summary>
        /// <param name="pickup">The pickup.</param>
        public void Add(ResourcePickup pickup)
        {
            _pickups.Add(pickup);
        }

        /// <summary>Stops tracking a pickup.</summary>
        /// <param name="pickup">The pickup.</param>
        public void Remove(ResourcePickup pickup)
        {
            _pickups.Remove(pickup);
        }

        /// <summary>Horizontal distance from (x, z) to a pickup.</summary>
        /// <param name="pickup">The pickup.</param>
        /// <param name="x">World X.</param>
        /// <param name="z">World Z.</param>
        public static float Distance(ResourcePickup pickup, float x, float z)
        {
            var position = pickup.transform.position;
            var dx = position.x - x;
            var dz = position.z - z;
            return Mathf.Sqrt(dx * dx + dz * dz);
        }

        /// <summary>Finds the nearest tracked pickup within range.</summary>
        /// <param name="x">World X.</param>
        /// <param name="z">World Z.</param>
        /// <param name="range">Largest horizontal distance in meters.</param>
        /// <param name="nearest">The pickup when found.</param>
        /// <returns>True when one is within range.</returns>
        public bool TryFindNearest(float x, float z, float range, out ResourcePickup nearest)
        {
            nearest = null;
            var best = range;
            for (var i = 0; i < _pickups.Count; i++)
            {
                var pickup = _pickups[i];
                if (pickup == null)
                {
                    continue;
                }

                var distance = Distance(pickup, x, z);
                if (distance <= best)
                {
                    best = distance;
                    nearest = pickup;
                }
            }

            return nearest != null;
        }
    }
}
