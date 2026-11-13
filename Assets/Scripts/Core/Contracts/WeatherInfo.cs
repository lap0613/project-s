namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// Plain-data description of one weather, carried by weather events so subscribers need no reference
    /// to the Environment system (design/quick-specs/core-rules.md section 3).
    /// </summary>
    /// <example>
    /// <code>
    /// var blizzard = new WeatherInfo("blizzard", "Blizzard", -15f, true);
    /// </code>
    /// </example>
    public readonly struct WeatherInfo
    {
        /// <summary>Creates a weather description.</summary>
        /// <param name="id">Stable identifier, e.g. "blizzard".</param>
        /// <param name="displayName">Text shown to the player.</param>
        /// <param name="temperatureOffset">Change to the ambient temperature in degrees C.</param>
        /// <param name="hasFog">True when this weather limits sight with fog.</param>
        public WeatherInfo(string id, string displayName, float temperatureOffset, bool hasFog)
        {
            Id = id;
            DisplayName = displayName;
            TemperatureOffset = temperatureOffset;
            HasFog = hasFog;
        }

        /// <summary>Stable identifier.</summary>
        public string Id { get; }

        /// <summary>Text shown to the player.</summary>
        public string DisplayName { get; }

        /// <summary>Change to the ambient temperature in degrees C.</summary>
        public float TemperatureOffset { get; }

        /// <summary>True when this weather limits sight with fog.</summary>
        public bool HasFog { get; }
    }
}
