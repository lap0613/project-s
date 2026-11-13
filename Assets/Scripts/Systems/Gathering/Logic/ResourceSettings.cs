using System;
using System.Collections.Generic;
using Whiteout.Core.Contracts;

namespace Whiteout.Gathering
{
    /// <summary>
    /// Values the Resources Logic needs, built from a data asset (design/quick-specs/items.md,
    /// core-rules.md section 4).
    /// </summary>
    /// <example>
    /// <code>
    /// var settings = new ResourceSettings(items, 6f, 1f, 1f, 3f, startingItems);
    /// bool known = settings.TryGetItem(new ItemId("wood"), out var wood);
    /// </code>
    /// </example>
    public sealed class ResourceSettings : IItemNames
    {
        private readonly Dictionary<ItemId, ItemSpec> _items = new Dictionary<ItemId, ItemSpec>();

        /// <summary>Creates settings.</summary>
        /// <param name="items">Every item kind in the game.</param>
        /// <param name="baseBagSlots">Bag slots before modifiers.</param>
        /// <param name="baseGatherSpeed">Gather speed before modifiers (1 = normal).</param>
        /// <param name="gatherSeconds">Seconds to hold E to gather at normal speed.</param>
        /// <param name="cookSeconds">Seconds to hold E to cook.</param>
        /// <param name="startingItems">What the bag holds at the start of a run.</param>
        public ResourceSettings(
            IReadOnlyList<ItemSpec> items,
            float baseBagSlots,
            float baseGatherSpeed,
            float gatherSeconds,
            float cookSeconds,
            IReadOnlyList<ItemAmount> startingItems)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            for (var i = 0; i < items.Count; i++)
            {
                _items[items[i].Id] = items[i];
            }

            BaseBagSlots = baseBagSlots;
            BaseGatherSpeed = baseGatherSpeed;
            GatherSeconds = gatherSeconds;
            CookSeconds = cookSeconds;
            StartingItems = startingItems ?? new ItemAmount[0];
        }

        /// <summary>Bag slots before modifiers.</summary>
        public float BaseBagSlots { get; }

        /// <summary>Gather speed before modifiers (1 = normal).</summary>
        public float BaseGatherSpeed { get; }

        /// <summary>Seconds to hold E to gather at normal speed.</summary>
        public float GatherSeconds { get; }

        /// <summary>Seconds to hold E to cook.</summary>
        public float CookSeconds { get; }

        /// <summary>What the bag holds at the start of a run.</summary>
        public IReadOnlyList<ItemAmount> StartingItems { get; }

        /// <summary>Looks up an item kind.</summary>
        /// <param name="id">Item id.</param>
        /// <param name="spec">The spec when found.</param>
        /// <returns>True when the item is known.</returns>
        public bool TryGetItem(ItemId id, out ItemSpec spec)
        {
            return _items.TryGetValue(id, out spec);
        }

        /// <inheritdoc />
        public string GetDisplayName(ItemId item)
        {
            return _items.TryGetValue(item, out var spec) ? spec.DisplayName : item.ToString();
        }
    }
}
