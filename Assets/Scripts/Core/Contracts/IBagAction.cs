namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// One entry of the bag slot's right-click menu (design/ux/ui-plan.md D3, D5): use, drop, later equip.
    /// A new entry is a new class registered as <see cref="IBagAction"/>; the menu and the other entries do not change.
    /// The entry with the lowest <see cref="Order"/> is listed first and is the default action.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Register&lt;DropItemAction&gt;(Lifetime.Singleton).As&lt;IBagAction&gt;();
    /// if (action.AppliesTo(slot.Item)) { action.Execute(slotIndex); }
    /// </code>
    /// </example>
    public interface IBagAction
    {
        /// <summary>Key of the entry's label in the UI text table.</summary>
        string LabelKey { get; }

        /// <summary>Position in the menu; lower comes first.</summary>
        int Order { get; }

        /// <summary>True when the entry is offered for this item.</summary>
        /// <param name="item">Item in the clicked slot.</param>
        bool AppliesTo(ItemId item);

        /// <summary>Runs the entry on a bag slot.</summary>
        /// <param name="slotIndex">Zero-based slot index.</param>
        void Execute(int slotIndex);
    }
}
