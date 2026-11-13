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

        [Header("Goal progress [C] (ui-plan 7.3 [C], N1)")]
        [Tooltip("Gap between the timeline bar and the goal progress row.")]
        [SerializeField] private float goalGapBelowTimeline = 10f;

        [Tooltip("Side of the beacon and part icons.")]
        [SerializeField] private float goalIconSize = 48f;

        [Tooltip("Space between the icons and the count.")]
        [SerializeField] private float goalIconSpacing = 8f;

        [Tooltip("Font size of the 0/3 count.")]
        [SerializeField] private float goalCountFontSize = 34f;

        [Tooltip("Color of a part icon the player has.")]
        [SerializeField] private Color goalPartHaveColor = new Color(1f, 0.85f, 0.3f, 1f);

        [Tooltip("Color of a part icon the player lacks (faint silhouette).")]
        [SerializeField] private Color goalPartMissingColor = new Color(1f, 1f, 1f, 0.22f);

        [Tooltip("Color of the beacon icon.")]
        [SerializeField] private Color goalBeaconColor = new Color(0.85f, 0.93f, 1f, 1f);

        [Tooltip("Panel behind the progress row.")]
        [SerializeField] private Color goalBackColor = new Color(0f, 0f, 0f, 0.4f);

        [Tooltip("Provisional. Color of the check mark on a part that is installed on the beacon.")]
        [SerializeField] private Color goalPartInstalledCheckColor = new Color(0.4f, 0.95f, 0.45f, 1f);

        [Tooltip("Provisional. Side of the check mark on an installed part.")]
        [SerializeField] private float goalCheckSize = 24f;

        [Tooltip("Provisional. Font size of the rescue timer that replaces the part row after rescue is requested.")]
        [SerializeField] private float rescueTimerFontSize = 34f;

        [Tooltip("Provisional. Color of the rescue timer text.")]
        [SerializeField] private Color rescueTimerColor = new Color(0.4f, 0.95f, 0.45f, 1f);

        [Header("Results screen (ui-plan S10)")]
        [Tooltip("Provisional. Seconds the results screen takes to fade in (unscaled time).")]
        [SerializeField] private float resultFadeSeconds = 1f;

        [Tooltip("Provisional. Full-screen color behind the results panel.")]
        [SerializeField] private Color resultBackdropColor = new Color(0f, 0f, 0f, 0.75f);

        [Tooltip("Provisional. Color of the results panel.")]
        [SerializeField] private Color resultPanelColor = new Color(0.08f, 0.1f, 0.16f, 0.95f);

        [Tooltip("Provisional. Size of the results panel.")]
        [SerializeField] private Vector2 resultPanelSize = new Vector2(760f, 560f);

        [Tooltip("Provisional. Space between the panel edge and its content.")]
        [SerializeField] private float resultPadding = 48f;

        [Tooltip("Provisional. Font size of the results title.")]
        [SerializeField] private float resultTitleFontSize = 64f;

        [Tooltip("Provisional. Font size of the results rows (days, cause, augments).")]
        [SerializeField] private float resultBodyFontSize = 36f;

        [Tooltip("Provisional. Space between the results rows.")]
        [SerializeField] private float resultRowSpacing = 16f;

        [Tooltip("Provisional. Size of the restart button.")]
        [SerializeField] private Vector2 resultButtonSize = new Vector2(320f, 84f);

        [Tooltip("Provisional. Color of the restart button.")]
        [SerializeField] private Color resultButtonColor = new Color(0.85f, 0.93f, 1f, 1f);

        [Tooltip("Provisional. Color of the restart button under the cursor.")]
        [SerializeField] private Color resultButtonHighlightColor = new Color(1f, 0.85f, 0.3f, 1f);

        [Tooltip("Provisional. Color of the restart button after it was clicked.")]
        [SerializeField] private Color resultButtonDisabledColor = new Color(0.6f, 0.6f, 0.6f, 1f);

        [Tooltip("Provisional. Color of the restart button text.")]
        [SerializeField] private Color resultButtonTextColor = new Color(0.08f, 0.1f, 0.2f, 1f);

        [Header("Start guide (ui-plan 7.6, N2)")]
        [Tooltip("Seconds the start guide stays at full size.")]
        [SerializeField] private float guideSeconds = 6f;

        [Tooltip("Seconds it takes to shrink into the goal progress row.")]
        [SerializeField] private float guideShrinkSeconds = 0.7f;

        [Tooltip("Gap between the goal progress row and the start guide.")]
        [SerializeField] private float guideTopOffset = 16f;

        [Tooltip("Font size of the guide phrase.")]
        [SerializeField] private float guideFontSize = 34f;

        [Tooltip("Side of an icon in the guide's picture row.")]
        [SerializeField] private float guideIconSize = 54f;

        [Tooltip("Color of the arrows between the icons.")]
        [SerializeField] private Color guideArrowColor = new Color(1f, 1f, 1f, 0.75f);

        [Tooltip("Panel behind the guide.")]
        [SerializeField] private Color guideBackColor = new Color(0f, 0f, 0f, 0.5f);

        [Header("Warmth vignette (ui-plan 7.6, N3)")]
        [Tooltip("Orange of the edge glow (alpha is set by the max opacity).")]
        [SerializeField] private Color warmthVignetteColor = new Color(1f, 0.55f, 0.15f, 1f);

        [Tooltip("Opacity at the edge while inside a lit fire's warmth radius.")]
        [SerializeField] private float warmthVignetteMaxOpacity = 0.25f;

        [Tooltip("Seconds to turn on.")]
        [SerializeField] private float warmthFadeInSeconds = 0.5f;

        [Tooltip("Seconds to turn off.")]
        [SerializeField] private float warmthFadeOutSeconds = 1f;

        [Tooltip("Distance from the screen centre (0 centre, 1 edge middle) where the vignette starts; inside stays clear.")]
        [SerializeField] private float vignetteInnerRadius = 0.6f;

        [Tooltip("Distance where the vignette reaches full strength (corners are 1.41).")]
        [SerializeField] private float vignetteOuterRadius = 1.3f;

        [Header("Morning (ui-plan 7.6, N4)")]
        [Tooltip("Seconds the sun icon and day text show.")]
        [SerializeField] private float morningBannerSeconds = 1.5f;

        [Tooltip("Seconds of fade in and fade out inside that time.")]
        [SerializeField] private float morningBannerFadeSeconds = 0.25f;

        [Tooltip("Vertical anchor of the banner (1 = top of the screen).")]
        [SerializeField] private float morningBannerTopFraction = 0.78f;

        [Tooltip("Font size of the day text.")]
        [SerializeField] private float morningBannerFontSize = 96f;

        [Tooltip("Side of the sun icon.")]
        [SerializeField] private float morningBannerIconSize = 96f;

        [Tooltip("Color of the day text.")]
        [SerializeField] private Color morningBannerColor = new Color(1f, 0.93f, 0.7f, 1f);

        [Tooltip("Color of the sun icon.")]
        [SerializeField] private Color morningSunColor = new Color(1f, 0.85f, 0.3f, 1f);

        [Tooltip("Seconds of the golden edge glow (up and down).")]
        [SerializeField] private float morningGlowSeconds = 0.8f;

        [Tooltip("Gold of the edge glow.")]
        [SerializeField] private Color morningGlowColor = new Color(1f, 0.82f, 0.3f, 1f);

        [Tooltip("Peak opacity of the glow (provisional, the plan gives none).")]
        [SerializeField] private float morningGlowMaxOpacity = 0.55f;

        [Header("Sound (tech-decisions 33, N5)")]
        [Tooltip("One-shot played when the player enters a lit fire's warmth radius.")]
        [SerializeField] private AudioClip warmthEnterClip = null;

        [Tooltip("Looping fire sound, louder when closer.")]
        [SerializeField] private AudioClip fireLoopClip = null;

        [Tooltip("One-shot played at the morning effect.")]
        [SerializeField] private AudioClip morningClip = null;

        [Tooltip("Volume of the enter sound (0..1).")]
        [SerializeField] private float warmthEnterVolume = 0.8f;

        [Tooltip("Volume of the fire loop at the fire (0..1).")]
        [SerializeField] private float fireLoopMaxVolume = 0.5f;

        [Tooltip("Volume of the morning sound (0..1).")]
        [SerializeField] private float morningVolume = 0.8f;

        [Tooltip("The enter sound of the same fire does not play again within this time.")]
        [SerializeField] private float warmthEnterCooldownSeconds = 10f;

        [Tooltip("Distance from a lit fire where its loop falls to silence (the fire status only reports fires within its nearby range).")]
        [SerializeField] private float fireLoopHearDistance = 8f;

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


        /// <summary>Gap between the timeline bar and the goal progress row.</summary>
        public float GoalGapBelowTimeline => goalGapBelowTimeline;

        /// <summary>Side of the beacon and part icons.</summary>
        public float GoalIconSize => goalIconSize;

        /// <summary>Space between the icons and the count.</summary>
        public float GoalIconSpacing => goalIconSpacing;

        /// <summary>Font size of the 0/3 count.</summary>
        public float GoalCountFontSize => goalCountFontSize;

        /// <summary>Color of a part icon the player has.</summary>
        public Color GoalPartHaveColor => goalPartHaveColor;

        /// <summary>Color of a part icon the player lacks (faint silhouette).</summary>
        public Color GoalPartMissingColor => goalPartMissingColor;

        /// <summary>Color of the beacon icon.</summary>
        public Color GoalBeaconColor => goalBeaconColor;

        /// <summary>Panel behind the progress row.</summary>
        public Color GoalBackColor => goalBackColor;

        /// <summary>Color of the check mark on a part that is installed on the beacon.</summary>
        public Color GoalPartInstalledCheckColor => goalPartInstalledCheckColor;

        /// <summary>Side of the check mark on an installed part.</summary>
        public float GoalCheckSize => goalCheckSize;

        /// <summary>Font size of the rescue timer.</summary>
        public float RescueTimerFontSize => rescueTimerFontSize;

        /// <summary>Color of the rescue timer text.</summary>
        public Color RescueTimerColor => rescueTimerColor;

        /// <summary>Seconds the results screen takes to fade in.</summary>
        public float ResultFadeSeconds => resultFadeSeconds;

        /// <summary>Full-screen color behind the results panel.</summary>
        public Color ResultBackdropColor => resultBackdropColor;

        /// <summary>Color of the results panel.</summary>
        public Color ResultPanelColor => resultPanelColor;

        /// <summary>Size of the results panel.</summary>
        public Vector2 ResultPanelSize => resultPanelSize;

        /// <summary>Space between the panel edge and its content.</summary>
        public float ResultPadding => resultPadding;

        /// <summary>Font size of the results title.</summary>
        public float ResultTitleFontSize => resultTitleFontSize;

        /// <summary>Font size of the results rows.</summary>
        public float ResultBodyFontSize => resultBodyFontSize;

        /// <summary>Space between the results rows.</summary>
        public float ResultRowSpacing => resultRowSpacing;

        /// <summary>Size of the restart button.</summary>
        public Vector2 ResultButtonSize => resultButtonSize;

        /// <summary>Color of the restart button.</summary>
        public Color ResultButtonColor => resultButtonColor;

        /// <summary>Color of the restart button under the cursor.</summary>
        public Color ResultButtonHighlightColor => resultButtonHighlightColor;

        /// <summary>Color of the restart button after it was clicked.</summary>
        public Color ResultButtonDisabledColor => resultButtonDisabledColor;

        /// <summary>Color of the restart button text.</summary>
        public Color ResultButtonTextColor => resultButtonTextColor;

        /// <summary>Seconds the start guide stays at full size.</summary>
        public float GuideSeconds => guideSeconds;

        /// <summary>Seconds it takes to shrink into the goal progress row.</summary>
        public float GuideShrinkSeconds => guideShrinkSeconds;

        /// <summary>Gap between the goal progress row and the start guide.</summary>
        public float GuideTopOffset => guideTopOffset;

        /// <summary>Font size of the guide phrase.</summary>
        public float GuideFontSize => guideFontSize;

        /// <summary>Side of an icon in the guide's picture row.</summary>
        public float GuideIconSize => guideIconSize;

        /// <summary>Color of the arrows between the icons.</summary>
        public Color GuideArrowColor => guideArrowColor;

        /// <summary>Panel behind the guide.</summary>
        public Color GuideBackColor => guideBackColor;

        /// <summary>Orange of the edge glow (alpha is set by the max opacity).</summary>
        public Color WarmthVignetteColor => warmthVignetteColor;

        /// <summary>Opacity at the edge while inside a lit fire's warmth radius.</summary>
        public float WarmthVignetteMaxOpacity => warmthVignetteMaxOpacity;

        /// <summary>Seconds to turn on.</summary>
        public float WarmthFadeInSeconds => warmthFadeInSeconds;

        /// <summary>Seconds to turn off.</summary>
        public float WarmthFadeOutSeconds => warmthFadeOutSeconds;

        /// <summary>Distance from the screen centre (0 centre, 1 edge middle) where the vignette starts; inside stays clear.</summary>
        public float VignetteInnerRadius => vignetteInnerRadius;

        /// <summary>Distance where the vignette reaches full strength (corners are 1.41).</summary>
        public float VignetteOuterRadius => vignetteOuterRadius;

        /// <summary>Seconds the sun icon and day text show.</summary>
        public float MorningBannerSeconds => morningBannerSeconds;

        /// <summary>Seconds of fade in and fade out inside that time.</summary>
        public float MorningBannerFadeSeconds => morningBannerFadeSeconds;

        /// <summary>Vertical anchor of the banner (1 = top of the screen).</summary>
        public float MorningBannerTopFraction => morningBannerTopFraction;

        /// <summary>Font size of the day text.</summary>
        public float MorningBannerFontSize => morningBannerFontSize;

        /// <summary>Side of the sun icon.</summary>
        public float MorningBannerIconSize => morningBannerIconSize;

        /// <summary>Color of the day text.</summary>
        public Color MorningBannerColor => morningBannerColor;

        /// <summary>Color of the sun icon.</summary>
        public Color MorningSunColor => morningSunColor;

        /// <summary>Seconds of the golden edge glow (up and down).</summary>
        public float MorningGlowSeconds => morningGlowSeconds;

        /// <summary>Gold of the edge glow.</summary>
        public Color MorningGlowColor => morningGlowColor;

        /// <summary>Peak opacity of the glow (provisional, the plan gives none).</summary>
        public float MorningGlowMaxOpacity => morningGlowMaxOpacity;

        /// <summary>One-shot played when the player enters a lit fire's warmth radius.</summary>
        public AudioClip WarmthEnterClip => warmthEnterClip;

        /// <summary>Looping fire sound, louder when closer.</summary>
        public AudioClip FireLoopClip => fireLoopClip;

        /// <summary>One-shot played at the morning effect.</summary>
        public AudioClip MorningClip => morningClip;

        /// <summary>Volume of the enter sound (0..1).</summary>
        public float WarmthEnterVolume => warmthEnterVolume;

        /// <summary>Volume of the fire loop at the fire (0..1).</summary>
        public float FireLoopMaxVolume => fireLoopMaxVolume;

        /// <summary>Volume of the morning sound (0..1).</summary>
        public float MorningVolume => morningVolume;

        /// <summary>The enter sound of the same fire does not play again within this time.</summary>
        public float WarmthEnterCooldownSeconds => warmthEnterCooldownSeconds;

        /// <summary>Distance from a lit fire where its loop falls to silence (the fire status only reports fires within its nearby range).</summary>
        public float FireLoopHearDistance => fireLoopHearDistance;

        /// <summary>
        /// Top-left corner of the timeline bar, relative to the top center of the canvas. The goal progress row
        /// and the start guide line up under it.
        /// </summary>
        public Vector2 TimelineAnchoredPosition => new Vector2(
            temperaturePanelSize.x * 0.5f + timelineGap,
            -hudTopMargin - (temperaturePanelSize.y - timelineSize.y) * 0.5f);

        private void OnValidate()
        {
            noticeMaxLines = Mathf.Max(noticeMaxLines, 1);
            noticeSeconds = Mathf.Max(noticeSeconds, 0f);
            timelineRangeSeconds = Mathf.Max(timelineRangeSeconds, 1f);
            resultFadeSeconds = Mathf.Max(resultFadeSeconds, 0f);
            guideSeconds = Mathf.Max(guideSeconds, 0f);
            guideShrinkSeconds = Mathf.Max(guideShrinkSeconds, 0.01f);
            warmthFadeInSeconds = Mathf.Max(warmthFadeInSeconds, 0.01f);
            warmthFadeOutSeconds = Mathf.Max(warmthFadeOutSeconds, 0.01f);
            morningGlowSeconds = Mathf.Max(morningGlowSeconds, 0.01f);
            morningBannerSeconds = Mathf.Max(morningBannerSeconds, 0.01f);
            morningBannerFadeSeconds = Mathf.Clamp(morningBannerFadeSeconds, 0f, morningBannerSeconds * 0.5f);
            vignetteOuterRadius = Mathf.Max(vignetteOuterRadius, vignetteInnerRadius + 0.01f);
            warmthEnterCooldownSeconds = Mathf.Max(warmthEnterCooldownSeconds, 0f);
            fireLoopHearDistance = Mathf.Max(fireLoopHearDistance, 0.1f);
        }
    }
}
