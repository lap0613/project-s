using System;
using System.Collections.Generic;
using Whiteout.Core.Contracts;
using Whiteout.Core.Stats;

namespace Whiteout.Gathering
{
    /// <summary>
    /// The player's bag (design/quick-specs/items.md section 1): a number of slots, each holding one kind of item
    /// up to its stack limit. The slot count is a <see cref="Stat"/> so backpacks and sleds attach as modifiers.
    /// Used slots are kept packed at the front: an emptied slot disappears and the ones behind it move up.
    /// If the capacity drops below the used slots, nothing is lost; the bag just accepts nothing new.
    /// </summary>
    /// <example>
    /// <code>
    /// var bag = new Bag(settings);
    /// int added = bag.Add(wood, 7);          // 5 in one slot, 2 in the next
    /// bool ok = bag.TryRemove(wood, 3);
    /// </code>
    /// </example>
    public sealed class Bag : IBag
    {
        private readonly ResourceSettings _settings;
        private readonly List<Stack> _stacks = new List<Stack>();

        /// <summary>Creates the bag and puts the starting items in it.</summary>
        /// <param name="settings">Item rules, slot count and starting items.</param>
        public Bag(ResourceSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            SlotCapacityStat = new Stat(ResourceStats.BagSlots, settings.BaseBagSlots);

            for (var i = 0; i < settings.StartingItems.Count; i++)
            {
                Add(settings.StartingItems[i].Item, settings.StartingItems[i].Amount);
            }
        }

        /// <summary>Slot count stat: attach modifiers here (big backpack +3, sled x2).</summary>
        public Stat SlotCapacityStat { get; }

        /// <inheritdoc />
        public int Version { get; private set; }

        /// <inheritdoc />
        public int SlotCapacity => Math.Max(0, (int)Math.Round(SlotCapacityStat.Value));

        /// <inheritdoc />
        public int SlotCount => Math.Max(SlotCapacity, _stacks.Count);

        /// <inheritdoc />
        public int SelectedSlot { get; private set; }

        /// <inheritdoc />
        public void SelectSlot(int index)
        {
            if (index < 0 || index >= SlotCount || index == SelectedSlot)
            {
                return;
            }

            SelectedSlot = index;
            Version++;
        }

        /// <inheritdoc />
        public BagSlotInfo GetSlot(int index)
        {
            if (index < 0 || index >= _stacks.Count)
            {
                return new BagSlotInfo(default, string.Empty, 0, 0);
            }

            var stack = _stacks[index];
            return new BagSlotInfo(stack.Spec.Id, stack.Spec.DisplayName, stack.Count, stack.Spec.StackLimit);
        }

        /// <inheritdoc />
        public int Count(ItemId item)
        {
            var total = 0;
            for (var i = 0; i < _stacks.Count; i++)
            {
                if (_stacks[i].Spec.Id == item)
                {
                    total += _stacks[i].Count;
                }
            }

            return total;
        }

        /// <inheritdoc />
        public bool CanAdd(ItemId item, int amount)
        {
            if (amount <= 0)
            {
                return true;
            }

            return _settings.TryGetItem(item, out var spec) && Room(spec) >= amount;
        }

        /// <inheritdoc />
        public int Add(ItemId item, int amount)
        {
            if (amount <= 0 || !_settings.TryGetItem(item, out var spec))
            {
                return 0;
            }

            var remaining = amount;
            for (var i = 0; i < _stacks.Count && remaining > 0; i++)
            {
                if (_stacks[i].Spec.Id != item)
                {
                    continue;
                }

                var moved = Math.Min(remaining, spec.StackLimit - _stacks[i].Count);
                _stacks[i].Count += moved;
                remaining -= moved;
            }

            while (remaining > 0 && _stacks.Count < SlotCapacity)
            {
                var moved = Math.Min(remaining, spec.StackLimit);
                _stacks.Add(new Stack(spec, moved));
                remaining -= moved;
            }

            var added = amount - remaining;
            if (added > 0)
            {
                Version++;
            }

            return added;
        }

        /// <inheritdoc />
        public bool TryRemove(ItemId item, int amount)
        {
            if (amount <= 0)
            {
                return true;
            }

            if (Count(item) < amount)
            {
                return false;
            }

            var remaining = amount;
            for (var i = _stacks.Count - 1; i >= 0 && remaining > 0; i--)
            {
                if (_stacks[i].Spec.Id != item)
                {
                    continue;
                }

                var taken = Math.Min(remaining, _stacks[i].Count);
                _stacks[i].Count -= taken;
                remaining -= taken;
                if (_stacks[i].Count == 0)
                {
                    _stacks.RemoveAt(i);
                }
            }

            Version++;
            return true;
        }

        /// <inheritdoc />
        public bool TryRemoveAll(IReadOnlyList<ItemAmount> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            for (var i = 0; i < items.Count; i++)
            {
                var needed = 0;
                for (var j = 0; j < items.Count; j++)
                {
                    if (items[j].Item == items[i].Item)
                    {
                        needed += items[j].Amount;
                    }
                }

                if (Count(items[i].Item) < needed)
                {
                    return false;
                }
            }

            for (var i = 0; i < items.Count; i++)
            {
                TryRemove(items[i].Item, items[i].Amount);
            }

            return true;
        }

        /// <inheritdoc />
        public bool TryRemoveSlot(int index, out ItemAmount removed)
        {
            if (index < 0 || index >= _stacks.Count)
            {
                removed = default;
                return false;
            }

            removed = new ItemAmount(_stacks[index].Spec.Id, _stacks[index].Count);
            _stacks.RemoveAt(index);
            Version++;
            return true;
        }

        private int Room(ItemSpec spec)
        {
            var room = 0;
            for (var i = 0; i < _stacks.Count; i++)
            {
                if (_stacks[i].Spec.Id == spec.Id)
                {
                    room += spec.StackLimit - _stacks[i].Count;
                }
            }

            var freeSlots = Math.Max(0, SlotCapacity - _stacks.Count);
            return room + freeSlots * spec.StackLimit;
        }

        private sealed class Stack
        {
            public Stack(ItemSpec spec, int count)
            {
                Spec = spec;
                Count = count;
            }

            public ItemSpec Spec { get; }

            public int Count { get; set; }
        }
    }
}
