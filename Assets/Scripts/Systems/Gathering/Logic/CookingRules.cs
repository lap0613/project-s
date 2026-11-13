using System;
using Whiteout.Core.Contracts;

namespace Whiteout.Gathering
{
    /// <summary>
    /// Cooking at a lit fire: one cookable item becomes its cooked item (raw meat to cooked meat;
    /// core-rules section 4). Whether a lit fire is in range is decided by the caller (View, tech-decisions item 13).
    /// </summary>
    /// <example>
    /// <code>
    /// if (cooking.TryFindCookable(out var raw)) { cooking.TryCook(raw); }
    /// </code>
    /// </example>
    public sealed class CookingRules
    {
        private readonly IBag _bag;
        private readonly ResourceSettings _settings;

        /// <summary>Creates the rules.</summary>
        /// <param name="bag">The bag holding raw and cooked items.</param>
        /// <param name="settings">Item rules (which item cooks into which).</param>
        public CookingRules(IBag bag, ResourceSettings settings)
        {
            _bag = bag ?? throw new ArgumentNullException(nameof(bag));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>Finds the first bag item that can be cooked.</summary>
        /// <param name="item">The cookable item when found.</param>
        /// <returns>True when the bag holds something cookable.</returns>
        public bool TryFindCookable(out ItemId item)
        {
            for (var i = 0; i < _bag.SlotCount; i++)
            {
                var slot = _bag.GetSlot(i);
                if (!slot.IsEmpty && _settings.TryGetItem(slot.Item, out var spec) && spec.CooksInto.HasValue)
                {
                    item = slot.Item;
                    return true;
                }
            }

            item = default;
            return false;
        }

        /// <summary>Turns one <paramref name="item"/> into its cooked item.</summary>
        /// <param name="item">The raw item.</param>
        /// <returns>False (bag unchanged) when the item is missing, cannot be cooked, or the result does not fit.</returns>
        public bool TryCook(ItemId item)
        {
            if (!_settings.TryGetItem(item, out var spec) || !spec.CooksInto.HasValue)
            {
                return false;
            }

            if (!_bag.TryRemove(item, 1))
            {
                return false;
            }

            if (!_bag.CanAdd(spec.CooksInto.Value, 1))
            {
                _bag.Add(item, 1);
                return false;
            }

            _bag.Add(spec.CooksInto.Value, 1);
            return true;
        }
    }
}
