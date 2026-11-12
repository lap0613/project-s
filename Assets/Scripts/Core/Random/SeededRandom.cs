using System;

namespace Whiteout.Core.Randomness
{
    /// <summary>
    /// <see cref="IRandom"/> over <see cref="System.Random"/> with an explicit seed.
    /// The same seed yields the same sequence. Not thread-safe.
    /// </summary>
    /// <example>
    /// <code>
    /// IRandom random = new SeededRandom(12345);
    /// </code>
    /// </example>
    public sealed class SeededRandom : IRandom
    {
        private const int FloatBits = 24;
        private const int FloatSteps = 1 << FloatBits;

        private readonly Random _random;

        /// <summary>Creates a generator.</summary>
        /// <param name="seed">Seed value.</param>
        public SeededRandom(int seed)
        {
            Seed = seed;
            _random = new Random(seed);
        }

        /// <summary>The seed this generator was created with.</summary>
        public int Seed { get; }

        /// <inheritdoc />
        public float NextFloat()
        {
            // 24 bits fit a float mantissa exactly, so the result can never round up to 1.0.
            return _random.Next(FloatSteps) / (float)FloatSteps;
        }

        /// <inheritdoc />
        public float NextRange(float min, float max)
        {
            return min + NextFloat() * (max - min);
        }

        /// <inheritdoc />
        public int NextInt(int minInclusive, int maxExclusive)
        {
            return _random.Next(minInclusive, maxExclusive);
        }
    }
}
