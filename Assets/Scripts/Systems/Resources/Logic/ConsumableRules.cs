using System;
using Whiteout.Core.Contracts;

namespace Whiteout.Resources
{
    /// <summary>
    /// Using a consumable from a bag slot: removes one and applies its effects
    /// (berry +15 hunger, raw meat +10, cooked meat +40, herb +20 health; design/quick-specs/items.md section 3).
    /// </summary>
    /// <example>
    /// <code>
    /// var rules = new ConsumableRules(bag, settings, vitals);
    /// bool used = rules.TryUseSlot(0);
    /// </code>
    /// </example>
    public sealed class ConsumableRules
    {
        private readonly IBag _bag;
        private readonly ResourceSettings _settings;
        private readonly IVitalsReceiver _vitals;

        /// <summary>Creates the rules.</summary>
        /// <param name="bag">The bag to take from.</param>
        /// <param name="settings">Item rules.</param>
        /// <param name="vitals">Gauges the effects change.</param>
        public ConsumableRules(IBag bag, ResourceSettings settings, IVitalsReceiver vitals)
        {
            _bag = bag ?? throw new ArgumentNullException(nameof(bag));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _vitals = vitals ?? throw new ArgumentNullException(nameof(vitals));
        }

        /// <summary>Uses one item from a slot.</summary>
        /// <param name="slotIndex">Zero-based slot index.</param>
        /// <returns>True when an item was used; false for an empty slot or an item that cannot be used.</returns>
        public bool TryUseSlot(int slotIndex)
        {
            var slot = _bag.GetSlot(slotIndex);
            if (slot.IsEmpty || !_settings.TryGetItem(slot.Item, out var spec) || !spec.IsConsumable)
            {
                return false;
            }

            if (!_bag.TryRemove(slot.Item, 1))
            {
                return false;
            }

            for (var i = 0; i < spec.UseEffects.Count; i++)
            {
                spec.UseEffects[i].Apply(_vitals);
            }

            return true;
        }
    }
}
