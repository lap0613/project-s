namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// Read-only description of the map: center, ring radii, and which ring a distance falls in.
    /// Values come from the implementation (a data asset), not from this interface.
    /// Positions are plain floats on the ground plane (x, z) in meters.
    /// </summary>
    /// <example>
    /// <code>
    /// float d = layout.DistanceFromCenter(player.x, player.z);
    /// if (layout.GetRing(d) == MapRing.Outer) { /* coldest zone */ }
    /// </code>
    /// </example>
    public interface IMapLayout
    {
        /// <summary>Map center X (meters).</summary>
        float CenterX { get; }

        /// <summary>Map center Z (meters).</summary>
        float CenterZ { get; }

        /// <summary>Outer edge radius of the map (meters).</summary>
        float OuterRadius { get; }

        /// <summary>Boundary between the Inner and Middle rings (meters).</summary>
        float InnerRingRadius { get; }

        /// <summary>Boundary between the Middle and Outer rings (meters).</summary>
        float MiddleRingRadius { get; }

        /// <summary>Distance from the map center to the point (x, z).</summary>
        /// <param name="x">World X.</param>
        /// <param name="z">World Z.</param>
        float DistanceFromCenter(float x, float z);

        /// <summary>
        /// Ring for a distance from the center. Boundary rule:
        /// d &lt; inner radius is Inner; inner &lt;= d &lt; middle is Middle;
        /// middle &lt;= d &lt;= outer is Outer; d &gt; outer is OutOfMap.
        /// With 60/130/200: d &lt; 60 Inner, 60..&lt;130 Middle, 130..200 Outer, &gt; 200 OutOfMap.
        /// </summary>
        /// <param name="distance">Distance from the map center in meters.</param>
        MapRing GetRing(float distance);
    }
}
