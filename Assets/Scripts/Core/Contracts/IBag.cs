using System.Collections.Generic;

namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// The player's bag (design/quick-specs/items.md section 1). Implemented by Resources; Camp and the HUD use it
    /// through this interface. The same item stacks in one slot up to its stack limit. One slot is selected with the
    /// number keys (ui-plan D3).
    /// </summary>
    /// <example>
    /// <code>
    /// if (bag.CanAdd(wood, 1)) { bag.Add(wood, 1); }
    /// bool paid = bag.TryRemoveAll(new[] { new ItemAmount(wood, 3), new ItemAmount(tinder, 1) });
    /// </code>
    /// </example>
    public interface IBag
    {
        /// <summary>Increases every time the contents, the slot count or the selection change; lets a HUD redraw only when needed.</summary>
        int Version { get; }

        /// <summary>How many slots the bag has right now (base slots plus modifiers).</summary>
        int SlotCapacity { get; }

        /// <summary>
        /// How many slots to show: <see cref="SlotCapacity"/>, or the number of used slots when a lowered capacity
        /// leaves more slots in use than the capacity.
        /// </summary>
        int SlotCount { get; }

        /// <summary>Zero-based index of the selected slot.</summary>
        int SelectedSlot { get; }

        /// <summary>Selects a slot; an index outside [0, <see cref="SlotCount"/>) is ignored.</summary>
        /// <param name="index">Zero-based slot index.</param>
        void SelectSlot(int index);

        /// <summary>Snapshot of a slot.</summary>
        /// <param name="index">Slot index in [0, <see cref="SlotCount"/>); anything else returns an empty slot.</param>
        BagSlotInfo GetSlot(int index);

        /// <summary>Total of <paramref name="item"/> across all slots.</summary>
        /// <param name="item">The item.</param>
        int Count(ItemId item);

        /// <summary>True when <paramref name="amount"/> of <paramref name="item"/> fit entirely. Unknown items never fit.</summary>
        /// <param name="item">The item.</param>
        /// <param name="amount">How many.</param>
        bool CanAdd(ItemId item, int amount);

        /// <summary>Adds as many as fit.</summary>
        /// <param name="item">The item.</param>
        /// <param name="amount">How many to add.</param>
        /// <returns>How many were actually added (0 when full or the item is unknown).</returns>
        int Add(ItemId item, int amount);

        /// <summary>Removes exactly <paramref name="amount"/>, or nothing.</summary>
        /// <param name="item">The item.</param>
        /// <param name="amount">How many.</param>
        /// <returns>True when removed.</returns>
        bool TryRemove(ItemId item, int amount);

        /// <summary>Removes every listed amount, or nothing at all.</summary>
        /// <param name="items">What to remove.</param>
        /// <returns>True when everything was available and removed.</returns>
        bool TryRemoveAll(IReadOnlyList<ItemAmount> items);

        /// <summary>Empties one slot (the whole stack).</summary>
        /// <param name="index">Zero-based slot index.</param>
        /// <param name="removed">What was in the slot.</param>
        /// <returns>False when the slot was empty.</returns>
        bool TryRemoveSlot(int index, out ItemAmount removed);
    }
}
