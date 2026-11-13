using System;
using System.Collections.Generic;
using Whiteout.Core.Contracts;

namespace Whiteout.Camp
{
    /// <summary>
    /// What placing a structure needs, built from a data asset (design/quick-specs/items.md section 7).
    /// Any structure (campfire, shelter, later the signal beacon) is placed through the same spec.
    /// </summary>
    /// <example>
    /// <code>
    /// var campfire = new StructureSpec("campfire", "모닥불",
    ///     new[] { new ItemAmount(wood, 3), new ItemAmount(tinder, 1) }, new[] { flint });
    /// </code>
    /// </example>
    public sealed class StructureSpec
    {
        /// <summary>Creates the spec.</summary>
        /// <param name="id">Structure id.</param>
        /// <param name="displayName">Player-facing name.</param>
        /// <param name="cost">Items consumed on placement.</param>
        /// <param name="requiredItems">Items that must be in the bag but are not consumed (flint).</param>
        public StructureSpec(string id, string displayName, IReadOnlyList<ItemAmount> cost, IReadOnlyList<ItemId> requiredItems)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            DisplayName = displayName ?? string.Empty;
            Cost = cost ?? throw new ArgumentNullException(nameof(cost));
            RequiredItems = requiredItems ?? throw new ArgumentNullException(nameof(requiredItems));
        }

        /// <summary>Structure id.</summary>
        public string Id { get; }

        /// <summary>Player-facing name.</summary>
        public string DisplayName { get; }

        /// <summary>Items consumed on placement.</summary>
        public IReadOnlyList<ItemAmount> Cost { get; }

        /// <summary>Items that must be in the bag but are not consumed.</summary>
        public IReadOnlyList<ItemId> RequiredItems { get; }
    }
}
