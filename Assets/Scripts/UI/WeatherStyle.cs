using System;
using UnityEngine;

namespace Whiteout.UI
{
    /// <summary>Shape drawn for a weather icon (generated at runtime, see <see cref="WeatherIconSprites"/>).</summary>
    public enum WeatherIconShape
    {
        /// <summary>Disc with rays.</summary>
        Sun,

        /// <summary>Six-armed snowflake.</summary>
        Snowflake,

        /// <summary>Three wind streaks.</summary>
        Wind,

        /// <summary>Thermometer.</summary>
        Thermometer
    }

    /// <summary>How one weather is drawn on the forecast timeline: icon shape, tint and whether it is a danger.</summary>
    /// <example>
    /// <code>
    /// var style = new WeatherStyle("blizzard", WeatherIconShape.Wind, Color.white, true);
    /// </code>
    /// </example>
    [Serializable]
    public sealed class WeatherStyle
    {
        [Tooltip("Weather id as in WeatherInfo.Id, e.g. cold_snap.")]
        [SerializeField] private string weatherId;

        [Tooltip("Icon shape.")]
        [SerializeField] private WeatherIconShape shape;

        [Tooltip("Icon tint.")]
        [SerializeField] private Color color = Color.white;

        [Tooltip("Draw the icon with the danger border (ui-plan 7.3 [B], T5).")]
        [SerializeField] private bool isDanger;

        /// <summary>Creates a style.</summary>
        /// <param name="weatherId">Weather id.</param>
        /// <param name="shape">Icon shape.</param>
        /// <param name="color">Icon tint.</param>
        /// <param name="isDanger">True for a danger weather.</param>
        public WeatherStyle(string weatherId, WeatherIconShape shape, Color color, bool isDanger)
        {
            this.weatherId = weatherId;
            this.shape = shape;
            this.color = color;
            this.isDanger = isDanger;
        }

        /// <summary>Weather id.</summary>
        public string WeatherId => weatherId;

        /// <summary>Icon shape.</summary>
        public WeatherIconShape Shape => shape;

        /// <summary>Icon tint.</summary>
        public Color Color => color;

        /// <summary>True for a danger weather.</summary>
        public bool IsDanger => isDanger;
    }
}
