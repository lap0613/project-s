using UnityEngine;

namespace Whiteout.RunFlow.Unity
{
    /// <summary>Tunable RunFlow values (data asset). Must not reference View code.</summary>
    /// <example>
    /// <code>
    /// // Assets > Create > Whiteout > RunFlow Config
    /// DayClockSettings settings = config.ToSettings();
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/RunFlow Config", fileName = "RunFlowConfig")]
    public sealed class RunFlowConfig : ScriptableObject
    {
        [Tooltip("Seconds of daytime (core-rules section 1: 120).")]
        [SerializeField] private float dayLengthSeconds = 120f;

        [Tooltip("Seconds of night (core-rules section 1: 60).")]
        [SerializeField] private float nightLengthSeconds = 60f;

        /// <summary>Seconds of daytime.</summary>
        public float DayLengthSeconds => dayLengthSeconds;

        /// <summary>Seconds of night.</summary>
        public float NightLengthSeconds => nightLengthSeconds;

        /// <summary>Converts to the settings object the Logic uses.</summary>
        /// <exception cref="System.ArgumentOutOfRangeException">A length is not greater than 0.</exception>
        public DayClockSettings ToSettings()
        {
            return new DayClockSettings(dayLengthSeconds, nightLengthSeconds);
        }
    }
}
