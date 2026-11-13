using System.Collections.Generic;

namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// The structure list of the placement mode (B key; design/ux/ui-plan.md section 9). Implemented by Camp from its
    /// structure catalog, read by the HUD. While open, number keys select from this list, not bag slots.
    /// </summary>
    /// <example>
    /// <code>
    /// for (var i = 0; i &lt; menu.EntryCount; i++) { Draw(menu.GetEntryName(i), menu.GetEntryCost(i), i == menu.SelectedIndex); }
    /// </code>
    /// </example>
    public interface IPlacementMenu
    {
        /// <summary>True while the placement mode is open.</summary>
        bool IsOpen { get; }

        /// <summary>Number of structures in the list.</summary>
        int EntryCount { get; }

        /// <summary>Index of the selected structure.</summary>
        int SelectedIndex { get; }

        /// <summary>
        /// Frame number (<c>Time.frameCount</c>) in which the placement mode used the left click, or -1. The interaction
        /// director skips a click made in that frame so one click never does two things (ui-plan section 8.1, M1).
        /// </summary>
        int ClickConsumedFrame { get; }

        /// <summary>Player-facing name of a structure.</summary>
        /// <param name="index">Index in [0, <see cref="EntryCount"/>).</param>
        string GetEntryName(int index);

        /// <summary>Items consumed when placing a structure.</summary>
        /// <param name="index">Index in [0, <see cref="EntryCount"/>).</param>
        IReadOnlyList<ItemAmount> GetEntryCost(int index);

        /// <summary>Items that must be in the bag but are not consumed (flint).</summary>
        /// <param name="index">Index in [0, <see cref="EntryCount"/>).</param>
        IReadOnlyList<ItemId> GetEntryRequiredItems(int index);
    }
}
