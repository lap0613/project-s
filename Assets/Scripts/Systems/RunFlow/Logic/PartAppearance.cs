using System;
using Whiteout.Core.Contracts;

namespace Whiteout.RunFlow
{
    /// <summary>
    /// One line of the beacon part schedule: which part appears on the morning of which day in which map ring
    /// (core-rules sections 1 and 5, tech-decisions 34, R2).
    /// </summary>
    /// <example>
    /// <code>
    /// var antenna = new PartAppearance(new ItemId("antenna"), 2, MapRing.Middle);
    /// </code>
    /// </example>
    public readonly struct PartAppearance
    {
        /// <summary>Creates the schedule line.</summary>
        /// <param name="part">The part item.</param>
        /// <param name="day">Day number (starts at 1) on whose morning the part appears.</param>
        /// <param name="ring">The map ring the part appears in.</param>
        public PartAppearance(ItemId part, int day, MapRing ring)
        {
            if (day < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(day), "day must be 1 or more.");
            }

            Part = part;
            Day = day;
            Ring = ring;
        }

        /// <summary>The part item.</summary>
        public ItemId Part { get; }

        /// <summary>Day number (starts at 1) on whose morning the part appears.</summary>
        public int Day { get; }

        /// <summary>The map ring the part appears in.</summary>
        public MapRing Ring { get; }
    }
}
