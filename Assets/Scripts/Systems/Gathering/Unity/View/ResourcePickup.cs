using UnityEngine;
using Whiteout.Core.Contracts;

namespace Whiteout.Gathering.Unity
{
    /// <summary>
    /// An item lying on the map. Created by <see cref="ResourceSpawner"/>; removed once gathered, and never
    /// respawned (items.md section 1).
    /// </summary>
    /// <example>
    /// <code>
    /// pickup.Initialize(wood, 1);
    /// </code>
    /// </example>
    public sealed class ResourcePickup : MonoBehaviour
    {
        /// <summary>The item this pickup gives.</summary>
        public ItemId Item { get; private set; }

        /// <summary>How many items it gives.</summary>
        public int Amount { get; private set; }

        /// <summary>Sets what the pickup gives.</summary>
        /// <param name="item">The item.</param>
        /// <param name="amount">How many.</param>
        public void Initialize(ItemId item, int amount)
        {
            Item = item;
            Amount = amount;
        }
    }
}
