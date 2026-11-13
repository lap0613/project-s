using System;

namespace Whiteout.RunFlow
{
    /// <summary>Phase lengths for <see cref="DayClock"/>, built from a data asset.</summary>
    /// <example>
    /// <code>
    /// var settings = new DayClockSettings(120f, 60f);
    /// </code>
    /// </example>
    public sealed class DayClockSettings
    {
        /// <summary>Creates settings.</summary>
        /// <param name="dayLength">Seconds of daytime. Must be greater than 0.</param>
        /// <param name="nightLength">Seconds of night. Must be greater than 0.</param>
        /// <exception cref="ArgumentOutOfRangeException">A length is 0, negative or NaN.</exception>
        public DayClockSettings(float dayLength, float nightLength)
        {
            if (!(dayLength > 0f))
            {
                throw new ArgumentOutOfRangeException(nameof(dayLength), "Day length must be greater than 0.");
            }

            if (!(nightLength > 0f))
            {
                throw new ArgumentOutOfRangeException(nameof(nightLength), "Night length must be greater than 0.");
            }

            DayLength = dayLength;
            NightLength = nightLength;
        }

        /// <summary>Seconds of daytime.</summary>
        public float DayLength { get; }

        /// <summary>Seconds of night.</summary>
        public float NightLength { get; }
    }
}
