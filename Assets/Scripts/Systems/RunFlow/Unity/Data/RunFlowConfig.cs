using System;
using System.Collections.Generic;
using UnityEngine;
using Whiteout.Core.Contracts;
using Whiteout.Core.Unity;

namespace Whiteout.RunFlow.Unity
{
    /// <summary>
    /// Tunable RunFlow values (data asset): day and night lengths and the beacon part schedule (core-rules
    /// sections 1 and 5, tech-decisions 34, R2). Must not reference View code.
    /// </summary>
    /// <example>
    /// <code>
    /// // Assets > Create > Whiteout > RunFlow Config
    /// DayClockSettings settings = config.ToSettings();
    /// PartScheduleSettings parts = config.ToPartScheduleSettings();
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/RunFlow Config", fileName = "RunFlowConfig")]
    public sealed class RunFlowConfig : ScriptableObject
    {
        /// <summary>One beacon part and the morning and map ring it appears in.</summary>
        [Serializable]
        public struct PartAppearanceEntry
        {
            [Tooltip("The part item.")]
            [SerializeField] private ItemIdAsset part;

            [Tooltip("Day number (starts at 1) on whose morning the part appears (core-rules section 1: 2, 4, 6).")]
            [SerializeField] private int day;

            [Tooltip("Map ring the part appears in (core-rules section 5: antenna Middle, battery and transmitter Outer).")]
            [SerializeField] private MapRing ring;

            /// <summary>The part item.</summary>
            public ItemIdAsset Part => part;

            /// <summary>Day number on whose morning the part appears.</summary>
            public int Day => day;

            /// <summary>Map ring the part appears in.</summary>
            public MapRing Ring => ring;
        }

        [Tooltip("Seconds of daytime (core-rules section 1: 120).")]
        [SerializeField] private float dayLengthSeconds = 120f;

        [Tooltip("Seconds of night (core-rules section 1: 60).")]
        [SerializeField] private float nightLengthSeconds = 60f;

        [Header("Beacon parts (core-rules sections 1 and 5)")]
        [Tooltip("Provisional (tech-decisions 34, R2). Which part appears on the morning of which day in which ring.")]
        [SerializeField] private PartAppearanceEntry[] partAppearances;

        /// <summary>Seconds of daytime.</summary>
        public float DayLengthSeconds => dayLengthSeconds;

        /// <summary>Seconds of night.</summary>
        public float NightLengthSeconds => nightLengthSeconds;

        /// <summary>Converts to the settings object the Logic uses.</summary>
        /// <returns>Day and night lengths.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">A length is not greater than 0.</exception>
        public DayClockSettings ToSettings()
        {
            return new DayClockSettings(dayLengthSeconds, nightLengthSeconds);
        }

        /// <summary>Converts the beacon part schedule to the settings object the Logic uses.</summary>
        /// <returns>The schedule; empty when no entries are set.</returns>
        /// <exception cref="InvalidOperationException">An entry has no part assigned.</exception>
        /// <exception cref="ArgumentOutOfRangeException">An entry's day is less than 1.</exception>
        public PartScheduleSettings ToPartScheduleSettings()
        {
            var appearances = new List<PartAppearance>();
            if (partAppearances != null)
            {
                for (var i = 0; i < partAppearances.Length; i++)
                {
                    var entry = partAppearances[i];
                    if (entry.Part == null)
                    {
                        throw new InvalidOperationException($"RunFlowConfig: Part Appearances element {i} has no Part.");
                    }

                    appearances.Add(new PartAppearance(entry.Part.Id, entry.Day, entry.Ring));
                }
            }

            return new PartScheduleSettings(appearances);
        }
    }
}
