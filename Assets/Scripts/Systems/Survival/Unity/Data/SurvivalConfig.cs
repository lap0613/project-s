using UnityEngine;

namespace Whiteout.Survival.Unity
{
    /// <summary>
    /// Tunable Survival values (data asset). Rules: design/quick-specs/core-rules.md section 2.
    /// Start value 100 is provisional (tech-decisions.md item 23). Must not reference View code.
    /// </summary>
    /// <example>
    /// <code>
    /// // Assets > Create > Whiteout > Survival Config
    /// SurvivalSettings settings = config.ToSettings();
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/Survival Config", fileName = "SurvivalConfig")]
    public sealed class SurvivalConfig : ScriptableObject
    {
        /// <summary>Lowest allowed gauge maximum; the HUD divides by the maximum, so it must stay above 0.</summary>
        private const float MinMaxValue = 1f;

        [Header("Gauges")]
        [Tooltip("Upper bound of body temperature, hunger and health.")]
        [SerializeField] private float maxValue = 100f;

        [Tooltip("Provisional. Starting value of every gauge.")]
        [SerializeField] private float startValue = 100f;

        [Header("Body temperature")]
        [Tooltip("Body temperature change per second per degree of ambient temperature.")]
        [SerializeField] private float temperaturePerDegree = 0.1f;

        [Tooltip("Multiplier on body temperature loss (not gain) while hunger is low.")]
        [SerializeField] private float coldHungerMultiplier = 1.5f;

        [Header("Hunger")]
        [Tooltip("Hunger lost per second.")]
        [SerializeField] private float hungerDrainPerSecond = 0.4f;

        [Header("Health")]
        [Tooltip("Body temperature and hunger must both be at least this for health to regenerate.")]
        [SerializeField] private float regenThreshold = 60f;

        [Tooltip("Health gained per second while regenerating.")]
        [SerializeField] private float healthRegenPerSecond = 0.5f;

        [Tooltip("Health lost per second while body temperature is 0.")]
        [SerializeField] private float freezingDamagePerSecond = 2f;

        [Tooltip("Health lost per second while hunger is 0.")]
        [SerializeField] private float starvationDamagePerSecond = 1f;

        [Header("Shelter rest (core-rules section 2)")]
        [Tooltip("While resting, body temperature and hunger need only be at least this for health to regenerate.")]
        [SerializeField] private float restRegenThreshold = 30f;

        [Tooltip("While resting, health regeneration is multiplied by this (3 = +1.5 per second).")]
        [SerializeField] private float restRegenMultiplier = 3f;

        [Header("Low penalties")]
        [Tooltip("A gauge below this counts as low.")]
        [SerializeField] private float lowThreshold = 30f;

        [Tooltip("Move speed reduction while body temperature is low (0.25 = -25%).")]
        [SerializeField] private float moveSpeedPenalty = 0.25f;

        [Tooltip("Gather speed reduction while hunger is low (0.3 = -30%). Applied by story 004.")]
        [SerializeField] private float gatherSpeedPenalty = 0.3f;

        private void OnValidate()
        {
            maxValue = Mathf.Max(maxValue, MinMaxValue);
            startValue = Mathf.Clamp(startValue, 0f, maxValue);
        }

        /// <summary>Converts to the settings object the Logic uses.</summary>
        public SurvivalSettings ToSettings()
        {
            return new SurvivalSettings(
                maxValue,
                startValue,
                temperaturePerDegree,
                coldHungerMultiplier,
                hungerDrainPerSecond,
                lowThreshold,
                regenThreshold,
                healthRegenPerSecond,
                freezingDamagePerSecond,
                starvationDamagePerSecond,
                moveSpeedPenalty,
                gatherSpeedPenalty,
                restRegenThreshold,
                restRegenMultiplier);
        }
    }
}
