using Whiteout.Core.Contracts;

namespace Whiteout.Gathering
{
    /// <summary>"Place this many pickups of this item on the map."</summary>
    /// <example>
    /// <code>
    /// var wood = new PlacementRequest(new ItemId("wood"), 50);
    /// </code>
    /// </example>
    public readonly struct PlacementRequest
    {
        /// <summary>Creates the request.</summary>
        /// <param name="item">The item.</param>
        /// <param name="count">How many pickups.</param>
        public PlacementRequest(ItemId item, int count)
        {
            Item = item;
            Count = count;
        }

        /// <summary>The item.</summary>
        public ItemId Item { get; }

        /// <summary>How many pickups.</summary>
        public int Count { get; }
    }
}
