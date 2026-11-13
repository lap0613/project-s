using System;
using UnityEngine;
using Whiteout.Core.Contracts;

namespace Whiteout.Gathering.Unity
{
    /// <summary>
    /// Creates pickup objects on the ground and tracks them in the <see cref="PickupRegistry"/>. Used for the map's
    /// pickups at the start of a run and for items dropped from the bag (ui-plan D5), which are ordinary pickups.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Register&lt;PickupFactory&gt;(Lifetime.Singleton);
    /// factory.Create(wood, 3, position, Quaternion.identity, null);
    /// </code>
    /// </example>
    public sealed class PickupFactory
    {
        private readonly ResourcesConfig _config;
        private readonly PickupRegistry _registry;

        /// <summary>Creates the factory.</summary>
        /// <param name="config">Finds an item's pickup prefab.</param>
        /// <param name="registry">Tracks created pickups.</param>
        public PickupFactory(ResourcesConfig config, PickupRegistry registry)
        {
            _config = config != null ? config : throw new ArgumentNullException(nameof(config));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        /// <summary>Creates one pickup.</summary>
        /// <param name="item">The item it gives.</param>
        /// <param name="amount">How many it gives.</param>
        /// <param name="position">World position.</param>
        /// <param name="rotation">World rotation.</param>
        /// <param name="parent">Optional parent; may be null.</param>
        /// <returns>The created pickup.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the item has no definition or no pickup prefab.</exception>
        public ResourcePickup Create(ItemId item, int amount, Vector3 position, Quaternion rotation, Transform parent)
        {
            if (!_config.TryGetDefinition(item, out var definition) || definition.PickupPrefab == null)
            {
                throw new InvalidOperationException($"PickupFactory: item '{item}' has no definition or no Pickup Prefab.");
            }

            var instance = UnityEngine.Object.Instantiate(definition.PickupPrefab, position, rotation, parent);
            if (!instance.TryGetComponent<ResourcePickup>(out var pickup))
            {
                pickup = instance.AddComponent<ResourcePickup>();
            }

            pickup.Initialize(item, amount);
            _registry.Add(pickup);
            return pickup;
        }
    }
}
