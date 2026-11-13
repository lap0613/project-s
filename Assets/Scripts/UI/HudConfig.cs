using UnityEngine;

namespace Whiteout.UI
{
    /// <summary>
    /// Tunable HUD values (data asset): notices, bag slot colors, the temperature display, the forecast timeline and
    /// world UI offsets (design/ux/ui-plan.md sections 7.3 and 8). All provisional. Texts are in the UiTextTable.
    /// Sizes are canvas units (the Game canvas scales from 1920x1080; ui-plan sizes are for 1280x720, so x1.5).
    /// </summary>
    /// <example>
    /// <code>
    /// // Assets > Create > Whiteout > HUD Config
    /// float seconds = config.NoticeSeconds;
    /// </code>
    /// </example>
    [CreateAssetMenu(menuName = "Whiteout/HUD Config", fileName = "HudConfig")]
    public sealed class HudConfig : ScriptableObject
    {
        [Header("Notices (ui-plan 7.3 [E])")]
        [Tooltip("Most notices shown at once; the oldest is dropped.")]
        [SerializeField] private int noticeMaxLines = 3;

        [Tooltip("Seconds a notice stays.")]
        [SerializeField] private float noticeSeconds = 3f;

        [Header("Bag slots (ui-plan 7.3 [G])")]
        [Tooltip("Border color of an unselected slot.")]
        [SerializeField] private Color slotBorderColor = new Color(1f, 1f, 1f, 0.35f);

        [Tooltip("Border color of the selected slot.")]
        [SerializeField] private Color slotSelectedBorderColor = new Color(1f, 0.85f, 0.2f, 1f);

        [Header("Shared")]
        [Tooltip("Color of text for something that cannot be done now (missing materials).")]
        [SerializeField] private Color unavailableTextColor = new Color(0.6f, 0.6f, 0.6f, 1f);

        [Tooltip("Normal text color.")]
        [SerializeField] private Color textColor = Color.white;

        [Header("World UI (ui-plan section 8)")]
        [Tooltip("The prompt is drawn this many meters above its target.")]
        [SerializeField] private float promptHeight = 1.5f;

        [Tooltip("The campfire fuel display is drawn this many meters above the fire.")]
        [SerializeField] private float fuelDisplayHeight = 2.2f;

        [Tooltip("Color of a fuel pip that still has fuel.")]
        [SerializeField] private Color fuelPipFilledColor = new Color(0.95f, 0.45f, 0.2f, 1f);

        [Tooltip("Color of an empty fuel pip.")]
        [SerializeField] private Color fuelPipEmptyColor = new Color(0f, 0f, 0f, 0.6f);

        [Header("Temperature display (ui-plan 7.3 [A])")]
        [Tooltip("Size of the panel behind the temperature number (provisional).")]
        [SerializeField] private Vector2 temperaturePanelSize = new Vector2(225f, 96f);

        [Tooltip("Panel color.")]
        [SerializeField] private Color temperaturePanelColor = new Color(0f, 0f, 0f, 0.45f);

        [Tooltip("Font size of the temperature number (about twice the gauge text, provisional).")]
        [SerializeField] private float temperatureFontSize = 60f;

        [Tooltip("Number color at or above the warm threshold (ui-plan assumption: orange).")]
        [SerializeField] private Color temperatureWarmColor = new Color(1f, 0.6f, 0.2f, 1f);

        [Tooltip("Number color between the thresholds (white).")]
        [SerializeField] private Color temperatureColdColor = Color.white;

        [Tooltip("Number color at or below the frozen threshold (sky blue).")]
        [SerializeField] private Color temperatureFrozenColor = new Color(0.55f, 0.85f, 1f, 1f);

        [Tooltip("Degrees C from which the number is warm colored.")]
        [SerializeField] private float temperatureWarmThreshold = 0f;

        [Tooltip("Degrees C up to which the number is frozen colored.")]
        [SerializeField] private float temperatureFrozenThreshold = -20f;

        [Header("Forecast timeline layout (ui-plan 7.3 [B])")]
        [Tooltip("Distance from the top of the screen.")]
        [SerializeField] private float hudTopMargin = 18f;

        [Tooltip("Gap between the temperature panel and the timeline.")]
        [SerializeField] private float timelineGap = 18f;

        [Tooltip("Size of the timeline bar (ui-plan: about 420x40 at 1280x720, so 630x60 here).")]
        [SerializeField] private Vector2 timelineSize = new Vector2(630f, 60f);

        [Tooltip("Game seconds the bar shows from now (T2: 3 days).")]
        [SerializeField] private float timelineRangeSeconds = 540f;

        [Tooltip("Font size of day labels and the ? mark.")]
        [SerializeField] private float timelineFontSize = 20f;

        [Tooltip("Space before the day label inside a day.")]
        [SerializeField] private float dayHeaderPadding = 6f;

        [Tooltip("Width reserved for the day label (the current day reads e.g. '3 day').")]
        [SerializeField] private float dayLabelWidth = 60f;

        [Tooltip("Side of the square weather icon box.")]
        [SerializeField] private float weatherIconSize = 30f;

        [Tooltip("Thickness of the danger border around an icon.")]
        [SerializeField] private float iconBorderThickness = 3f;

        [Header("Forecast timeline colors")]
        [SerializeField] private Color timelineDayColor = new Color(0.78f, 0.84f, 0.92f, 0.95f);
        [SerializeField] private Color timelineNightColor = new Color(0.08f, 0.12f, 0.3f, 0.95f);
        [SerializeField] private Color timelineUnknownColor = new Color(0.35f, 0.35f, 0.38f, 0.55f);
        [SerializeField] private Color timelineDayTextColor = new Color(0.08f, 0.1f, 0.2f, 1f);

        [Tooltip("Day label color while the label sits over the night part.")]
        [SerializeField] private Color timelineNightTextColor = new Color(0.9f, 0.92f, 1f, 1f);
        [SerializeField] private Color timelineUnknownTextColor = new Color(0.85f, 0.85f, 0.88f, 0.9f);
        [SerializeField] private Color timelineTickColor = new Color(0.05f, 0.07f, 0.15f, 0.9f);
        [SerializeField] private float bigTickWidth = 3f;
        [SerializeField] private float smallTickWidth = 2f;

        [Tooltip("Height of the night-start tick as a fraction of the bar height.")]
        [SerializeField] private float smallTickHeightRatio = 0.5f;

        [SerializeField] private Color nowMarkerColor = new Color(1f, 0.85f, 0.2f, 1f);
        [SerializeField] private float nowMarkerWidth = 4f;
        [SerializeField] private float nowMarkerDiamondSize = 14f;
        [SerializeField] private Color iconBackColor = new Color(0.1f, 0.14f, 0.25f, 0.9f);
        [SerializeField] private Color dangerBorderColor = new Color(0.95f, 0.25f, 0.2f, 1f);

        [Header("Rescue flag (T7)")]
        [SerializeField] private Color rescueFlagColor = new Color(0.4f, 0.95f, 0.45f, 1f);
        [SerializeField] private float rescueFlagPoleWidth = 3f;
        [SerializeField] private Vector2 rescueFlagSize = new Vector2(20f, 14f);

        [Tooltip("How far the flag pole rises above the bar.")]
        [SerializeField] private float rescueFlagRise = 16f;

        [Header("Weather icon tooltip")]
        [SerializeField] private Color tooltipPanelColor = new Color(0f, 0f, 0f, 0.85f);
        [SerializeField] private float tooltipFontSize = 22f;
        [SerializeField] private Vector2 tooltipPadding = new Vector2(12f, 6f);

        [Tooltip("Gap between the bar and the tooltip.")]
        [SerializeField] private float tooltipOffset = 8f;

        [Header("Weather icon styles")]
        [SerializeField] private WeatherStyle[] weatherStyles =
        {
            new WeatherStyle("clear", WeatherIconShape.Sun, new Color(1f, 0.85f, 0.3f, 1f), false),
            new WeatherStyle("snow", WeatherIconShape.Snowflake, new Color(0.85f, 0.93f, 1f, 1f), false),
            new WeatherStyle("blizzard", WeatherIconShape.Wind, new Color(0.9f, 0.95f, 1f, 1f), true),
            new WeatherStyle("cold_snap", WeatherIconShape.Thermometer, new Color(0.5f, 0.85f, 1f, 1f), true)
        };

        /// <summary>Most notices shown at once.</summary>
        public int NoticeMaxLines => noticeMaxLines;

        /// <summary>Seconds a notice stays.</summary>
        public float NoticeSeconds => noticeSeconds;

        /// <summary>Border color of an unselected slot.</summary>
        public Color SlotBorderColor => slotBorderColor;

        /// <summary>Border color of the selected slot.</summary>
        public Color SlotSelectedBorderColor => slotSelectedBorderColor;

        /// <summary>Text color for something that cannot be done now.</summary>
        public Color UnavailableTextColor => unavailableTextColor;

        /// <summary>Normal text color.</summary>
        public Color TextColor => textColor;

        /// <summary>Height of the prompt above its target, in meters.</summary>
        public float PromptHeight => promptHeight;

        /// <summary>Height of the fuel display above the fire, in meters.</summary>
        public float FuelDisplayHeight => fuelDisplayHeight;

        /// <summary>Color of a fuel pip that still has fuel.</summary>
        public Color FuelPipFilledColor => fuelPipFilledColor;

        /// <summary>Color of an empty fuel pip.</summary>
        public Color FuelPipEmptyColor => fuelPipEmptyColor;

        /// <summary>Size of the temperature panel.</summary>
        public Vector2 TemperaturePanelSize => temperaturePanelSize;

        /// <summary>Temperature panel color.</summary>
        public Color TemperaturePanelColor => temperaturePanelColor;

        /// <summary>Font size of the temperature number.</summary>
        public float TemperatureFontSize => temperatureFontSize;

        /// <summary>Number color when warm.</summary>
        public Color TemperatureWarmColor => temperatureWarmColor;

        /// <summary>Number color when cold.</summary>
        public Color TemperatureColdColor => temperatureColdColor;

        /// <summary>Number color when frozen.</summary>
        public Color TemperatureFrozenColor => temperatureFrozenColor;

        /// <summary>Degrees C from which the number is warm colored.</summary>
        public float TemperatureWarmThreshold => temperatureWarmThreshold;

        /// <summary>Degrees C up to which the number is frozen colored.</summary>
        public float TemperatureFrozenThreshold => temperatureFrozenThreshold;

        /// <summary>Distance from the top of the screen.</summary>
        public float HudTopMargin => hudTopMargin;

        /// <summary>Gap between the temperature panel and the timeline.</summary>
        public float TimelineGap => timelineGap;

        /// <summary>Size of the timeline bar.</summary>
        public Vector2 TimelineSize => timelineSize;

        /// <summary>Game seconds the bar shows from now.</summary>
        public float TimelineRangeSeconds => timelineRangeSeconds;

        /// <summary>Font size of timeline labels.</summary>
        public float TimelineFontSize => timelineFontSize;

        /// <summary>Space before the day label.</summary>
        public float DayHeaderPadding => dayHeaderPadding;

        /// <summary>Width reserved for the day label.</summary>
        public float DayLabelWidth => dayLabelWidth;

        /// <summary>Side of the weather icon box.</summary>
        public float WeatherIconSize => weatherIconSize;

        /// <summary>Thickness of the danger border.</summary>
        public float IconBorderThickness => iconBorderThickness;

        /// <summary>Day segment color.</summary>
        public Color TimelineDayColor => timelineDayColor;

        /// <summary>Night segment color.</summary>
        public Color TimelineNightColor => timelineNightColor;

        /// <summary>Color of the no-forecast segment.</summary>
        public Color TimelineUnknownColor => timelineUnknownColor;

        /// <summary>Day label color.</summary>
        public Color TimelineDayTextColor => timelineDayTextColor;

        /// <summary>Day label color while the label sits over the night part.</summary>
        public Color TimelineNightTextColor => timelineNightTextColor;

        /// <summary>Color of the ? mark.</summary>
        public Color TimelineUnknownTextColor => timelineUnknownTextColor;

        /// <summary>Tick color.</summary>
        public Color TimelineTickColor => timelineTickColor;

        /// <summary>Width of the day-start tick.</summary>
        public float BigTickWidth => bigTickWidth;

        /// <summary>Width of the night-start tick.</summary>
        public float SmallTickWidth => smallTickWidth;

        /// <summary>Night tick height as a fraction of the bar.</summary>
        public float SmallTickHeightRatio => smallTickHeightRatio;

        /// <summary>Color of the fixed now marker.</summary>
        public Color NowMarkerColor => nowMarkerColor;

        /// <summary>Width of the now line.</summary>
        public float NowMarkerWidth => nowMarkerWidth;

        /// <summary>Size of the now marker diamond.</summary>
        public float NowMarkerDiamondSize => nowMarkerDiamondSize;

        /// <summary>Background of a weather icon.</summary>
        public Color IconBackColor => iconBackColor;

        /// <summary>Border color of danger weather.</summary>
        public Color DangerBorderColor => dangerBorderColor;

        /// <summary>Rescue flag color.</summary>
        public Color RescueFlagColor => rescueFlagColor;

        /// <summary>Rescue flag pole width.</summary>
        public float RescueFlagPoleWidth => rescueFlagPoleWidth;

        /// <summary>Rescue flag pennant size.</summary>
        public Vector2 RescueFlagSize => rescueFlagSize;

        /// <summary>How far the flag pole rises above the bar.</summary>
        public float RescueFlagRise => rescueFlagRise;

        /// <summary>Tooltip background.</summary>
        public Color TooltipPanelColor => tooltipPanelColor;

        /// <summary>Tooltip font size.</summary>
        public float TooltipFontSize => tooltipFontSize;

        /// <summary>Tooltip padding.</summary>
        public Vector2 TooltipPadding => tooltipPadding;

        /// <summary>Gap between the bar and the tooltip.</summary>
        public float TooltipOffset => tooltipOffset;

        /// <summary>Icon style per weather id.</summary>
        public WeatherStyle[] WeatherStyles => weatherStyles;

        private void OnValidate()
        {
            noticeMaxLines = Mathf.Max(noticeMaxLines, 1);
            noticeSeconds = Mathf.Max(noticeSeconds, 0f);
            timelineRangeSeconds = Mathf.Max(timelineRangeSeconds, 1f);
        }
    }
}
