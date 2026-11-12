using System;
using Whiteout.Core.Contracts;
using Whiteout.Core.Randomness;

namespace Whiteout.Player
{
    /// <summary>
    /// Picks a random start point inside the Inner ring (0 to inner ring radius around the map center),
    /// uniform by area. <see cref="PickFree"/> skips points the caller reports as blocked (tech-decisions 19).
    /// </summary>
    /// <example>
    /// <code>
    /// var picker = new PlayerStartPicker(random, mapLayout);
    /// PlanarVector start = picker.PickFree(p => !IsBlocked(p), 30);
    /// </code>
    /// </example>
    public sealed class PlayerStartPicker
    {
        private const float TwoPi = (float)(Math.PI * 2.0);

        private readonly IRandom _random;
        private readonly IMapLayout _layout;

        /// <summary>Creates a picker.</summary>
        /// <param name="random">Random source.</param>
        /// <param name="layout">Map description (center and inner ring radius).</param>
        public PlayerStartPicker(IRandom random, IMapLayout layout)
        {
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
        }

        /// <summary>Returns a start point (x, z) with distance from center in [0, inner ring radius).</summary>
        public PlanarVector Pick()
        {
            // sqrt of a uniform value makes the radius distribution uniform by area.
            var radius = _layout.InnerRingRadius * (float)Math.Sqrt(_random.NextFloat());
            var angle = _random.NextFloat() * TwoPi;
            var x = _layout.CenterX + radius * (float)Math.Cos(angle);
            var z = _layout.CenterZ + radius * (float)Math.Sin(angle);
            return new PlanarVector(x, z);
        }

        /// <summary>
        /// Returns the first random start point that <paramref name="isFree"/> accepts, trying up to
        /// <paramref name="maxAttempts"/> times. If none is free, returns the map center, which the map keeps clear
        /// of obstacles (core-rules §5).
        /// </summary>
        /// <param name="isFree">True when nothing blocks the player at the point.</param>
        /// <param name="maxAttempts">Number of random points to try.</param>
        public PlanarVector PickFree(Func<PlanarVector, bool> isFree, int maxAttempts)
        {
            if (isFree == null) throw new ArgumentNullException(nameof(isFree));

            for (var i = 0; i < maxAttempts; i++)
            {
                var candidate = Pick();
                if (isFree(candidate))
                {
                    return candidate;
                }
            }

            return new PlanarVector(_layout.CenterX, _layout.CenterZ);
        }
    }
}
