using Whiteout.Core.Contracts;
using UnityEngine;

namespace Whiteout.Core.Unity
{
    /// <summary>
    /// Editor-pickable wrapper around an <see cref="ItemId"/>, so data assets in different systems
    /// (item definitions in Resources, costs in Camp) point at the same item without referencing each other.
    /// </summary>
    /// <example>
    /// <code>
    /// [SerializeField] private ItemIdAsset item;
    /// ItemId id = item.Id;
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/Item Id", fileName = "ItemId")]
    public sealed class ItemIdAsset : ScriptableObject
    {
        [SerializeField] private string id;

        /// <summary>The item id. Throws <see cref="System.ArgumentException"/> if the asset's id text is empty.</summary>
        public ItemId Id => new ItemId(id);
    }
}
