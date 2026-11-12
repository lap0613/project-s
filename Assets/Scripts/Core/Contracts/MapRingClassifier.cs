namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// Implements the ring boundary rule of <see cref="IMapLayout.GetRing"/> once, so every
    /// <see cref="IMapLayout"/> implementation shares it.
    /// </summary>
    /// <example>
    /// <code>
    /// MapRing ring = MapRingClassifier.Classify(75f, 60f, 130f, 200f); // Middle
    /// </code>
    /// </example>
    public static class MapRingClassifier
    {
        /// <summary>Classifies a distance. NaN is reported as OutOfMap.</summary>
        /// <param name="distance">Distance from center (m).</param>
        /// <param name="innerRadius">Inner/Middle boundary (m).</param>
        /// <param name="middleRadius">Middle/Outer boundary (m).</param>
        /// <param name="outerRadius">Map edge (m).</param>
        public static MapRing Classify(float distance, float innerRadius, float middleRadius, float outerRadius)
        {
            if (distance < innerRadius)
            {
                return MapRing.Inner;
            }

            if (distance < middleRadius)
            {
                return MapRing.Middle;
            }

            return distance <= outerRadius ? MapRing.Outer : MapRing.OutOfMap;
        }
    }
}
