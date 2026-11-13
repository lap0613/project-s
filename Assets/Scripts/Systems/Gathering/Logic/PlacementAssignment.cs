using Whiteout.Core.Contracts;

namespace Whiteout.Gathering
{
    /// <summary>One chosen candidate point and the item that goes there.</summary>
    /// <example>
    /// <code>
    /// foreach (var a in plan) { Spawn(points[a.PointIndex], a.Item); }
    /// </code>
    /// </example>
    public readonly struct PlacementAssignment
    {
        /// <summary>Creates the assignment.</summary>
        /// <param name="pointIndex">Index into the candidate point list.</param>
        /// <param name="item">The item placed there.</param>
        public PlacementAssignment(int pointIndex, ItemId item)
        {
            PointIndex = pointIndex;
            Item = item;
        }

        /// <summary>Index into the candidate point list.</summary>
        public int PointIndex { get; }

        /// <summary>The item placed there.</summary>
        public ItemId Item { get; }
    }
}
