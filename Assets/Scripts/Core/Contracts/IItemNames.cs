namespace Whiteout.Core.Contracts
{
    /// <summary>Player-facing item names, so Camp and the HUD can name items without referencing Resources.</summary>
    /// <example>
    /// <code>
    /// string label = names.GetDisplayName(new ItemId("wood")); // "나무"
    /// </code>
    /// </example>
    public interface IItemNames
    {
        /// <summary>Name of an item; the id text when the item is unknown.</summary>
        /// <param name="item">The item.</param>
        string GetDisplayName(ItemId item);
    }
}
