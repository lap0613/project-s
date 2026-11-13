namespace Whiteout.Core.Contracts
{
    /// <summary>
    /// Current ambient temperature at the player. Implemented by Environment, read by Survival.
    /// Ambient temperature = base + weather + distance ring + campfire warmth + augment bonus
    /// (design/quick-specs/core-rules.md section 3).
    /// </summary>
    /// <example>
    /// <code>
    /// float perSecond = temperatureSource.CurrentTemperature * 0.1f;
    /// </code>
    /// </example>
    public interface IAmbientTemperatureSource
    {
        /// <summary>Ambient temperature in degrees C at the player's current position.</summary>
        float CurrentTemperature { get; }
    }
}
