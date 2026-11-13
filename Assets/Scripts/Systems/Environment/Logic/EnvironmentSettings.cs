using System;
using System.Collections.Generic;

namespace Whiteout.Environment
{
    /// <summary>
    /// Values <see cref="EnvironmentSystem"/> needs, built from a data asset
    /// (design/quick-specs/core-rules.md section 3). Validates the weather list once, here.
    /// </summary>
    /// <example>
    /// <code>
    /// var settings = new EnvironmentSettings(-10f, 5f, -5f, -15f, specs);
    /// </code>
    /// </example>
    public sealed class EnvironmentSettings
    {
        /// <summary>Creates settings.</summary>
        /// <param name="baseTemperature">Base ambient temperature in degrees C.</param>
        /// <param name="innerRingOffset">Offset in the Inner ring.</param>
        /// <param name="middleRingOffset">Offset in the Middle ring.</param>
        /// <param name="outerRingOffset">Offset in the Outer ring (also used beyond the map edge).</param>
        /// <param name="weathers">Drawable weathers. Needs at least one, no negative weight, a total weight above 0.</param>
        /// <exception cref="ArgumentException">The weather list is empty or its weights are invalid.</exception>
        public EnvironmentSettings(
            float baseTemperature,
            float innerRingOffset,
            float middleRingOffset,
            float outerRingOffset,
            IReadOnlyList<WeatherSpec> weathers)
        {
            if (weathers == null || weathers.Count == 0)
            {
                throw new ArgumentException("At least one weather is required.", nameof(weathers));
            }

            var total = 0f;
            for (var i = 0; i < weathers.Count; i++)
            {
                var weight = weathers[i].Weight;
                if (!(weight >= 0f))
                {
                    throw new ArgumentException("Weather weights must be 0 or more.", nameof(weathers));
                }

                total += weight;
            }

            if (!(total > 0f))
            {
                throw new ArgumentException("Total weather weight must be greater than 0.", nameof(weathers));
            }

            BaseTemperature = baseTemperature;
            InnerRingOffset = innerRingOffset;
            MiddleRingOffset = middleRingOffset;
            OuterRingOffset = outerRingOffset;
            Weathers = weathers;
            TotalWeight = total;
        }

        /// <summary>Base ambient temperature in degrees C.</summary>
        public float BaseTemperature { get; }

        /// <summary>Offset in the Inner ring.</summary>
        public float InnerRingOffset { get; }

        /// <summary>Offset in the Middle ring.</summary>
        public float MiddleRingOffset { get; }

        /// <summary>Offset in the Outer ring.</summary>
        public float OuterRingOffset { get; }

        /// <summary>Drawable weathers.</summary>
        public IReadOnlyList<WeatherSpec> Weathers { get; }

        /// <summary>Sum of all weights.</summary>
        public float TotalWeight { get; }
    }
}
