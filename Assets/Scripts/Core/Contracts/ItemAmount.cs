namespace Whiteout.Core.Contracts
{
    /// <summary>An item and how many of it (a cost, a reward, a stack).</summary>
    /// <example>
    /// <code>
    /// var cost = new ItemAmount(new ItemId("wood"), 3);
    /// </code>
    /// </example>
    public readonly struct ItemAmount
    {
        /// <summary>Creates the pair.</summary>
        /// <param name="item">The item.</param>
        /// <param name="amount">How many.</param>
        public ItemAmount(ItemId item, int amount)
        {
            Item = item;
            Amount = amount;
        }

        /// <summary>The item.</summary>
        public ItemId Item { get; }

        /// <summary>How many.</summary>
        public int Amount { get; }
    }
}
