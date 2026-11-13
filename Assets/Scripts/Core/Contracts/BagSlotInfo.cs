namespace Whiteout.Core.Contracts
{
    /// <summary>Read-only snapshot of one bag slot, for display.</summary>
    /// <example>
    /// <code>
    /// BagSlotInfo slot = bag.GetSlot(0);
    /// if (!slot.IsEmpty) { label = $"{slot.DisplayName} x{slot.Count}"; }
    /// </code>
    /// </example>
    public readonly struct BagSlotInfo
    {
        /// <summary>Creates the snapshot.</summary>
        /// <param name="item">Item in the slot.</param>
        /// <param name="displayName">Player-facing item name.</param>
        /// <param name="count">How many are stacked; 0 for an empty slot.</param>
        /// <param name="stackLimit">Most the slot can hold of this item.</param>
        public BagSlotInfo(ItemId item, string displayName, int count, int stackLimit)
        {
            Item = item;
            DisplayName = displayName ?? string.Empty;
            Count = count;
            StackLimit = stackLimit;
        }

        /// <summary>Item in the slot (default value when empty).</summary>
        public ItemId Item { get; }

        /// <summary>Player-facing item name; empty for an empty slot.</summary>
        public string DisplayName { get; }

        /// <summary>How many are stacked; 0 for an empty slot.</summary>
        public int Count { get; }

        /// <summary>Most the slot can hold of this item.</summary>
        public int StackLimit { get; }

        /// <summary>True when nothing is in the slot.</summary>
        public bool IsEmpty => Count <= 0;
    }
}
