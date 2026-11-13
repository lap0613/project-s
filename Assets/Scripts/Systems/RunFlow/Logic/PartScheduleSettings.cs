using System;
using System.Collections.Generic;

namespace Whiteout.RunFlow
{
    /// <summary>
    /// The beacon part schedule as data for <see cref="PartSchedule"/>, built from the RunFlow config asset
    /// (tech-decisions 34, R2).
    /// </summary>
    /// <example>
    /// <code>
    /// var settings = new PartScheduleSettings(new[] { new PartAppearance(antenna, 2, MapRing.Middle) });
    /// </code>
    /// </example>
    public sealed class PartScheduleSettings
    {
        /// <summary>Creates the settings.</summary>
        /// <param name="appearances">Every part appearance of a run. May be empty.</param>
        public PartScheduleSettings(IReadOnlyList<PartAppearance> appearances)
        {
            Appearances = appearances ?? throw new ArgumentNullException(nameof(appearances));
        }

        /// <summary>Every part appearance of a run.</summary>
        public IReadOnlyList<PartAppearance> Appearances { get; }
    }
}
