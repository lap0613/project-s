namespace Whiteout.Core.Contracts
{
    /// <summary>Distance ring around the map center. See <see cref="IMapLayout.GetRing"/> for boundaries.</summary>
    public enum MapRing
    {
        /// <summary>Closest ring (0 to inner radius).</summary>
        Inner,

        /// <summary>Middle ring.</summary>
        Middle,

        /// <summary>Outer ring, up to the map edge.</summary>
        Outer,

        /// <summary>Beyond the map edge.</summary>
        OutOfMap,
    }
}
