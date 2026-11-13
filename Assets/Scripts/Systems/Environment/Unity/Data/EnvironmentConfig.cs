using System;
using UnityEngine;

namespace Whiteout.Environment.Unity
{
    /// <summary>
    /// Tunable Environment values (data asset): temperatures, the weather catalog, vision and the
    /// provisional look of night and blizzard fog. Must not reference View code.
    /// Rules: design/quick-specs/core-rules.md section 3.
    /// </summary>
    /// <example>
    /// <code>
    /// // Assets > Create > Whiteout > Environment Config; list the WeatherDefinition assets.
    /// EnvironmentSettings settings = config.ToSettings();
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/Environment Config", fileName = "EnvironmentConfig")]
    public sealed class EnvironmentConfig : ScriptableObject
    {
        [Header("Temperature (core-rules section 3)")]
        [Tooltip("Base ambient temperature in degrees C.")]
        [SerializeField] private float baseTemperature = -10f;

        [Tooltip("Offset in the Inner ring (0-60 m).")]
        [SerializeField] private float innerRingOffset = 5f;

        [Tooltip("Offset in the Middle ring (60-130 m).")]
        [SerializeField] private float middleRingOffset = -5f;

        [Tooltip("Offset in the Outer ring (130-200 m). Also used beyond the map edge.")]
        [SerializeField] private float outerRingOffset = -15f;

        [Tooltip("Every drawable weather. Adding a weather = adding an asset here.")]
        [SerializeField] private WeatherDefinition[] weathers;

        [Header("Vision (core-rules section 3)")]
        [Tooltip("At night only this radius around the player is lit, in meters.")]
        [SerializeField] private float nightVisionRadius = 12f;

        [Tooltip("In fog weather everything beyond this distance from the player starts to fade, in meters.")]
        [SerializeField] private float blizzardFogDistance = 15f;

        [Header("Look (PLACEHOLDER values, tune after first playtest)")]
        [Tooltip("Placeholder. Meters over which fog goes from clear to fully opaque, starting at the fog distance.")]
        [SerializeField] private float blizzardFogFalloff = 15f;

        [Tooltip("Placeholder. Fog color.")]
        [SerializeField] private Color blizzardFogColor = new Color(0.82f, 0.87f, 0.92f, 1f);

        [Tooltip("Placeholder. Night multiplier on the sun light intensity (0 = no sun light).")]
        [SerializeField] private float nightSunIntensityScale = 0.05f;

        [Tooltip("Placeholder. Night multiplier on the ambient light intensity (RenderSettings.ambientIntensity).")]
        [SerializeField] private float nightAmbientIntensityScale = 0.1f;

        [Tooltip("Placeholder. Seconds to blend between day and night, and fog on or off. 0 = instant.")]
        [SerializeField] private float visionFadeSeconds = 1f;

        /// <summary>Night lit radius around the player (m).</summary>
        public float NightVisionRadius => nightVisionRadius;

        /// <summary>Distance from the player where fog starts (m).</summary>
        public float BlizzardFogDistance => blizzardFogDistance;

        /// <summary>Placeholder: meters from clear to fully opaque fog.</summary>
        public float BlizzardFogFalloff => blizzardFogFalloff;

        /// <summary>Placeholder: fog color.</summary>
        public Color BlizzardFogColor => blizzardFogColor;

        /// <summary>Placeholder: night multiplier on the sun intensity.</summary>
        public float NightSunIntensityScale => nightSunIntensityScale;

        /// <summary>Placeholder: night multiplier on the ambient intensity.</summary>
        public float NightAmbientIntensityScale => nightAmbientIntensityScale;

        /// <summary>Placeholder: blend time in seconds.</summary>
        public float VisionFadeSeconds => visionFadeSeconds;

        /// <summary>Converts to the settings object the Logic uses.</summary>
        /// <exception cref="InvalidOperationException">The weather list has an empty slot.</exception>
        /// <exception cref="ArgumentException">The weather list is empty or its weights are invalid.</exception>
        public EnvironmentSettings ToSettings()
        {
            var specs = new WeatherSpec[weathers == null ? 0 : weathers.Length];
            for (var i = 0; i < specs.Length; i++)
            {
                if (weathers[i] == null)
                {
                    throw new InvalidOperationException("EnvironmentConfig: weather slot " + i + " is empty.");
                }

                specs[i] = weathers[i].ToSpec();
            }

            return new EnvironmentSettings(baseTemperature, innerRingOffset, middleRingOffset, outerRingOffset, specs);
        }
    }
}
