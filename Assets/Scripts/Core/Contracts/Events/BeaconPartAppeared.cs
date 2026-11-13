namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// Published by RunFlow on the morning a beacon part appears on the map (docs/architecture/tech-decisions.md 34,
    /// R2). Gathering puts the pickup on a candidate point of the ring and posts the notice.
    /// </summary>
    /// <example>
    /// <code>
    /// bus.Publish(new BeaconPartAppeared(antenna, MapRing.Middle));
    /// </code>
    /// </example>
    public readonly struct BeaconPartAppeared
    {
        /// <summary>Creates the event.</summary>
        /// <param name="part">The part item.</param>
        /// <param name="ring">The map ring the part appears in.</param>
        public BeaconPartAppeared(ItemId part, MapRing ring)
        {
            Part = part;
            Ring = ring;
        }

        /// <summary>The part item.</summary>
        public ItemId Part { get; }

        /// <summary>The map ring the part appears in.</summary>
        public MapRing Ring { get; }
    }
}
