namespace Whiteout.Survival
{
    /// <summary>
    /// Values <see cref="SurvivalSystem"/> needs, built from a data asset
    /// (design/quick-specs/core-rules.md section 2).
    /// </summary>
    /// <example>
    /// <code>
    /// var settings = new SurvivalSettings(100f, 100f, 0.1f, 1.5f, 0.4f, 30f, 60f, 0.5f, 2f, 1f, 0.25f, 0.3f);
    /// </code>
    /// </example>
    public sealed class SurvivalSettings
    {
        /// <summary>Creates settings.</summary>
        /// <param name="maxValue">Upper bound of every gauge.</param>
        /// <param name="startValue">Starting value of every gauge.</param>
        /// <param name="temperaturePerDegree">Body temperature change per second per degree of ambient temperature.</param>
        /// <param name="coldHungerMultiplier">Multiplier on body temperature loss while hunger is low.</param>
        /// <param name="hungerDrainPerSecond">Hunger lost per second.</param>
        /// <param name="lowThreshold">Below this, a gauge counts as low (penalties).</param>
        /// <param name="regenThreshold">Body temperature and hunger must both be at least this for health to regenerate.</param>
        /// <param name="healthRegenPerSecond">Health gained per second while regenerating.</param>
        /// <param name="freezingDamagePerSecond">Health lost per second while body temperature is 0.</param>
        /// <param name="starvationDamagePerSecond">Health lost per second while hunger is 0.</param>
        /// <param name="moveSpeedPenalty">Move speed reduction while body temperature is low (0.25 = -25%).</param>
        /// <param name="gatherSpeedPenalty">Gather speed reduction while hunger is low (0.3 = -30%); applied by story 004.</param>
        public SurvivalSettings(
            float maxValue,
            float startValue,
            float temperaturePerDegree,
            float coldHungerMultiplier,
            float hungerDrainPerSecond,
            float lowThreshold,
            float regenThreshold,
            float healthRegenPerSecond,
            float freezingDamagePerSecond,
            float starvationDamagePerSecond,
            float moveSpeedPenalty,
            float gatherSpeedPenalty)
        {
            MaxValue = maxValue;
            StartValue = startValue;
            TemperaturePerDegree = temperaturePerDegree;
            ColdHungerMultiplier = coldHungerMultiplier;
            HungerDrainPerSecond = hungerDrainPerSecond;
            LowThreshold = lowThreshold;
            RegenThreshold = regenThreshold;
            HealthRegenPerSecond = healthRegenPerSecond;
            FreezingDamagePerSecond = freezingDamagePerSecond;
            StarvationDamagePerSecond = starvationDamagePerSecond;
            MoveSpeedPenalty = moveSpeedPenalty;
            GatherSpeedPenalty = gatherSpeedPenalty;
        }

        /// <summary>Upper bound of every gauge.</summary>
        public float MaxValue { get; }

        /// <summary>Starting value of every gauge.</summary>
        public float StartValue { get; }

        /// <summary>Body temperature change per second per degree of ambient temperature.</summary>
        public float TemperaturePerDegree { get; }

        /// <summary>Multiplier on body temperature loss while hunger is low.</summary>
        public float ColdHungerMultiplier { get; }

        /// <summary>Hunger lost per second.</summary>
        public float HungerDrainPerSecond { get; }

        /// <summary>Below this, a gauge counts as low.</summary>
        public float LowThreshold { get; }

        /// <summary>Body temperature and hunger must both be at least this for health to regenerate.</summary>
        public float RegenThreshold { get; }

        /// <summary>Health gained per second while regenerating.</summary>
        public float HealthRegenPerSecond { get; }

        /// <summary>Health lost per second while body temperature is 0.</summary>
        public float FreezingDamagePerSecond { get; }

        /// <summary>Health lost per second while hunger is 0.</summary>
        public float StarvationDamagePerSecond { get; }

        /// <summary>Move speed reduction while body temperature is low (0.25 = -25%).</summary>
        public float MoveSpeedPenalty { get; }

        /// <summary>Gather speed reduction while hunger is low (0.3 = -30%).</summary>
        public float GatherSpeedPenalty { get; }
    }
}
