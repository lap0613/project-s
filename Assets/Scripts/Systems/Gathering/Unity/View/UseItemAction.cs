using System;
using Whiteout.Core.Contracts;

namespace Whiteout.Gathering.Unity
{
    /// <summary>
    /// Bag slot menu entry "use" (ui-plan D3): offered for items that have a use effect, uses one from the slot.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Register&lt;UseItemAction&gt;(Lifetime.Singleton).As&lt;IBagAction&gt;();
    /// </code>
    /// </example>
    public sealed class UseItemAction : IBagAction
    {
        private readonly ConsumableRules _rules;
        private readonly ResourceSettings _settings;
        private readonly ResourcesConfig _config;

        /// <summary>Creates the entry.</summary>
        /// <param name="rules">Consumable use rule.</param>
        /// <param name="settings">Tells which items are consumables.</param>
        /// <param name="config">Menu position.</param>
        public UseItemAction(ConsumableRules rules, ResourceSettings settings, ResourcesConfig config)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _config = config != null ? config : throw new ArgumentNullException(nameof(config));
        }

        /// <inheritdoc />
        public string LabelKey => ResourceTextKeys.UseAction;

        /// <inheritdoc />
        public int Order => _config.UseActionOrder;

        /// <inheritdoc />
        public bool AppliesTo(ItemId item)
        {
            return _settings.TryGetItem(item, out var spec) && spec.IsConsumable;
        }

        /// <inheritdoc />
        public void Execute(int slotIndex)
        {
            _rules.TryUseSlot(slotIndex);
        }
    }
}
