using System;
using System.Collections.Generic;
using Whiteout.Core.Contracts;

namespace Whiteout.Camp
{
    /// <summary>
    /// Rule values of the signal beacon, built from a data asset (core-rules section 1, tech-decisions 34): the
    /// parts in install order and how many days rescue takes after the request.
    /// </summary>
    /// <example>
    /// <code>
    /// var spec = new SignalBeaconSpec(new[] { antenna, battery, transmitter }, 2f);
    /// </code>
    /// </example>
    public sealed class SignalBeaconSpec
    {
        /// <summary>Creates the spec.</summary>
        /// <param name="parts">The parts the beacon needs, in install order. At least one.</param>
        /// <param name="rescueWaitDays">Days (day + night) from the rescue request until rescue arrives.</param>
        public SignalBeaconSpec(IReadOnlyList<ItemId> parts, float rescueWaitDays)
        {
            Parts = parts ?? throw new ArgumentNullException(nameof(parts));
            if (parts.Count == 0)
            {
                throw new ArgumentException("The signal beacon needs at least one part.", nameof(parts));
            }

            if (!(rescueWaitDays >= 0f) || float.IsInfinity(rescueWaitDays))
            {
                throw new ArgumentOutOfRangeException(nameof(rescueWaitDays), "rescueWaitDays must be 0 or more.");
            }

            RescueWaitDays = rescueWaitDays;
        }

        /// <summary>The parts the beacon needs, in install order.</summary>
        public IReadOnlyList<ItemId> Parts { get; }

        /// <summary>Days (day + night) from the rescue request until rescue arrives.</summary>
        public float RescueWaitDays { get; }
    }
}
