using Whiteout.Core.Stats;

namespace Whiteout.Gathering
{
    /// <summary>Stat ids owned by the Resources system.</summary>
    /// <example>
    /// <code>
    /// var slots = new Stat(ResourceStats.BagSlots, 6f);
    /// </code>
    /// </example>
    public static class ResourceStats
    {
        /// <summary>Number of bag slots. Add modifiers for a bigger backpack, Multiply for a sled.</summary>
        public static readonly StatId BagSlots = new StatId("resources.bag_slots");

        /// <summary>Gather speed multiplier. Gather time = base time / this value.</summary>
        public static readonly StatId GatherSpeed = new StatId("resources.gather_speed");
    }
}
