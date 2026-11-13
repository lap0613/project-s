namespace Whiteout.Core.Contracts
{
    /// <summary>Published by Resources when the player picks something up from the map.</summary>
    /// <example>
    /// <code>
    /// using var sub = bus.Subscribe&lt;ResourceGathered&gt;(e =&gt; Log(e.Item, e.Amount));
    /// </code>
    /// </example>
    public readonly struct ResourceGathered
    {
        /// <summary>Creates the event.</summary>
        /// <param name="item">What was gathered.</param>
        /// <param name="amount">How many.</param>
        public ResourceGathered(ItemId item, int amount)
        {
            Item = item;
            Amount = amount;
        }

        /// <summary>What was gathered.</summary>
        public ItemId Item { get; }

        /// <summary>How many.</summary>
        public int Amount { get; }
    }
}
