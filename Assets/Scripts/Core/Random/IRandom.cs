namespace Whiteout.Core.Randomness
{
    /// <summary>
    /// Source of randomness for gameplay rules. Inject this instead of using a global random,
    /// so a seed reproduces a run.
    /// </summary>
    /// <example>
    /// <code>
    /// float roll = random.NextFloat();
    /// int pick = random.NextInt(0, items.Count);
    /// </code>
    /// </example>
    public interface IRandom
    {
        /// <summary>Returns a float in [0, 1) (never 1).</summary>
        float NextFloat();

        /// <summary>Returns a float in [<paramref name="min"/>, <paramref name="max"/>).</summary>
        /// <param name="min">Lower bound (inclusive).</param>
        /// <param name="max">Upper bound.</param>
        float NextRange(float min, float max);

        /// <summary>Returns an int in [<paramref name="minInclusive"/>, <paramref name="maxExclusive"/>).</summary>
        /// <param name="minInclusive">Lower bound (inclusive).</param>
        /// <param name="maxExclusive">Upper bound (exclusive). Must be >= minInclusive.</param>
        int NextInt(int minInclusive, int maxExclusive);
    }
}
