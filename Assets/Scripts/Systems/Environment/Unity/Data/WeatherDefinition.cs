using UnityEngine;
using Whiteout.Core.Contracts;

namespace Whiteout.Environment.Unity
{
    /// <summary>
    /// Data asset for one weather. Adding a weather means adding an asset and listing it in
    /// <see cref="EnvironmentConfig"/>; no code changes. Must not reference View code.
    /// </summary>
    /// <example>
    /// <code>
    /// // Assets > Create > Whiteout > Weather Definition
    /// // Blizzard: id "blizzard", weight 20, temperature offset -15, has fog on.
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/Weather Definition", fileName = "Weather")]
    public sealed class WeatherDefinition : ScriptableObject
    {
        [Tooltip("Stable identifier, e.g. blizzard.")]
        [SerializeField] private string id;

        [Tooltip("Name shown on the HUD.")]
        [SerializeField] private string displayName;

        [Tooltip("Relative probability weight. Probability = weight / sum of all weights in the config.")]
        [SerializeField] private float weight = 1f;

        [Tooltip("Change to the ambient temperature in degrees C.")]
        [SerializeField] private float temperatureOffset;

        [Tooltip("Fog limits sight beyond the blizzard fog distance while this weather is active.")]
        [SerializeField] private bool hasFog;

        /// <summary>Stable identifier.</summary>
        public string Id => id;

        /// <summary>Name shown on the HUD.</summary>
        public string DisplayName => displayName;

        /// <summary>Relative probability weight.</summary>
        public float Weight => weight;

        /// <summary>Change to the ambient temperature in degrees C.</summary>
        public float TemperatureOffset => temperatureOffset;

        /// <summary>True when this weather limits sight with fog.</summary>
        public bool HasFog => hasFog;

        /// <summary>Converts to the spec the Logic draws from.</summary>
        public WeatherSpec ToSpec()
        {
            return new WeatherSpec(new WeatherInfo(id, displayName, temperatureOffset, hasFog), weight);
        }
    }
}
