using System;
using System.Collections.Generic;
using Whiteout.Core.Contracts;

namespace Whiteout.Gathering
{
    /// <summary>Rules for one item kind, built from a data asset (design/quick-specs/items.md).</summary>
    /// <example>
    /// <code>
    /// var berry = new ItemSpec(new ItemId("berry"), "열매", 3, new IUseEffect[] { hungerEffect }, null);
    /// </code>
    /// </example>
    public sealed class ItemSpec
    {
        private static readonly IUseEffect[] NoEffects = new IUseEffect[0];

        /// <summary>Creates the spec.</summary>
        /// <param name="id">Item id.</param>
        /// <param name="displayName">Player-facing name.</param>
        /// <param name="stackLimit">Most that fit in one bag slot; at least 1.</param>
        /// <param name="useEffects">What using one does; null or empty means not usable.</param>
        /// <param name="cooksInto">Item produced by cooking this one at a lit fire; null if it cannot be cooked.</param>
        public ItemSpec(ItemId id, string displayName, int stackLimit, IReadOnlyList<IUseEffect> useEffects, ItemId? cooksInto)
        {
            if (stackLimit < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(stackLimit), "Stack limit must be at least 1.");
            }

            Id = id;
            DisplayName = displayName ?? string.Empty;
            StackLimit = stackLimit;
            UseEffects = useEffects ?? NoEffects;
            CooksInto = cooksInto;
        }

        /// <summary>Item id.</summary>
        public ItemId Id { get; }

        /// <summary>Player-facing name.</summary>
        public string DisplayName { get; }

        /// <summary>Most that fit in one bag slot.</summary>
        public int StackLimit { get; }

        /// <summary>What using one does.</summary>
        public IReadOnlyList<IUseEffect> UseEffects { get; }

        /// <summary>Item produced by cooking this one, or null.</summary>
        public ItemId? CooksInto { get; }

        /// <summary>True when the item has at least one use effect.</summary>
        public bool IsConsumable => UseEffects.Count > 0;
    }
}
