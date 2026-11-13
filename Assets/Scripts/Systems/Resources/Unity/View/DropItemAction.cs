using System;
using UnityEngine;
using Whiteout.Core.Contracts;

namespace Whiteout.Resources.Unity
{
    /// <summary>
    /// Bag slot menu entry "drop" (ui-plan D5, items.md section 1): offered for every item. The whole stack leaves
    /// the bag and lies at the player's feet as one pickup that can be gathered again with E.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Register&lt;DropItemAction&gt;(Lifetime.Singleton).As&lt;IBagAction&gt;();
    /// </code>
    /// </example>
    public sealed class DropItemAction : IBagAction
    {
        private readonly IBag _bag;
        private readonly IPlayerPosition _player;
        private readonly PickupFactory _factory;
        private readonly ResourcesConfig _config;

        /// <summary>Creates the entry.</summary>
        /// <param name="bag">The bag to take from.</param>
        /// <param name="player">Where the item is put down.</param>
        /// <param name="factory">Creates the pickup.</param>
        /// <param name="config">Menu position.</param>
        public DropItemAction(IBag bag, IPlayerPosition player, PickupFactory factory, ResourcesConfig config)
        {
            _bag = bag ?? throw new ArgumentNullException(nameof(bag));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _config = config != null ? config : throw new ArgumentNullException(nameof(config));
        }

        /// <inheritdoc />
        public string LabelKey => ResourceTextKeys.DropAction;

        /// <inheritdoc />
        public int Order => _config.DropActionOrder;

        /// <inheritdoc />
        public bool AppliesTo(ItemId item)
        {
            return true;
        }

        /// <inheritdoc />
        public void Execute(int slotIndex)
        {
            if (_bag.TryRemoveSlot(slotIndex, out var removed))
            {
                _factory.Create(
                    removed.Item, removed.Amount, new Vector3(_player.X, _player.Y, _player.Z), Quaternion.identity, null);
            }
        }
    }
}
