using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;
using Whiteout.Core.Unity;

namespace Whiteout.UI
{
    /// <summary>
    /// Top-centre HUD (ui-plan section 7.1, T1): the current ambient temperature as the largest number on a panel
    /// (7.3 [A]) and, to its right, the forecast timeline bar (7.3 [B], see <see cref="ForecastTimelineView"/>).
    /// Replaces the old text HUD. The panel and the bar are built at runtime under this object (which should
    /// stretch over the canvas); the existing temperature label is moved onto the panel. Texts and font come from
    /// the <see cref="UiTextTable"/>, sizes and colors from the <see cref="HudConfig"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// // Put on a full-canvas RectTransform; assign Temperature Label (TextMeshProUGUI).
    /// // GameLifetimeScope registers it with RegisterComponentInHierarchy&lt;WeatherHud&gt;().
    /// </code>
    /// </example>
    public sealed class WeatherHud : MonoBehaviour
    {
        private const string TemperatureKey = "hud.temperature";

        [SerializeField] private TMP_Text temperatureLabel;

        private IAmbientTemperatureSource _temperature;
        private UiTextTable _texts;
        private HudConfig _config;
        private IDayTimeSource _time;
        private ForecastTimelineView _timeline;
        private IDisposable _forecastSubscription;
        private IDisposable _rescueSubscription;
        private WeatherForecastUpdated _forecast;
        private bool _hasForecast;
        private float _rescueDelay;
        private bool _hasRescue;
        private float _shownTemperature = float.NaN;

        /// <summary>Receives dependencies from the container.</summary>
        /// <param name="bus">Event channel.</param>
        /// <param name="temperature">Ambient temperature at the player.</param>
        /// <param name="time">Game time of day.</param>
        /// <param name="texts">Texts and font.</param>
        /// <param name="config">Sizes and colors.</param>
        [Inject]
        public void Construct(IEventBus bus, IAmbientTemperatureSource temperature, IDayTimeSource time, UiTextTable texts, HudConfig config)
        {
            _temperature = temperature;
            _time = time;
            _texts = texts;
            _config = config;
            _forecastSubscription = bus.Subscribe<WeatherForecastUpdated>(e =>
            {
                _forecast = e;
                _hasForecast = true;
                _timeline?.SetForecast(e);
            });
            _rescueSubscription = bus.Subscribe<RescueRequested>(e =>
            {
                _rescueDelay = e.ArrivalDelaySeconds;
                _hasRescue = true;
                _timeline?.SetRescue(e.ArrivalDelaySeconds);
            });
        }

        private void Start()
        {
            var panelSize = _config.TemperaturePanelSize;
            var barSize = _config.TimelineSize;
            var left = -panelSize.x * 0.5f;

            var panel = new GameObject("TemperaturePanel", typeof(RectTransform)).GetComponent<RectTransform>();
            panel.SetParent(transform, false);
            panel.anchorMin = new Vector2(0.5f, 1f);
            panel.anchorMax = new Vector2(0.5f, 1f);
            panel.pivot = new Vector2(0f, 1f);
            panel.anchoredPosition = new Vector2(left, -_config.HudTopMargin);
            panel.sizeDelta = panelSize;
            var panelImage = panel.gameObject.AddComponent<Image>();
            panelImage.color = _config.TemperaturePanelColor;
            panelImage.raycastTarget = false;

            var labelRect = temperatureLabel.rectTransform;
            labelRect.SetParent(panel, false);
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            _texts.ApplyFont(temperatureLabel);
            temperatureLabel.fontSize = _config.TemperatureFontSize;
            temperatureLabel.alignment = TextAlignmentOptions.Center;
            temperatureLabel.textWrappingMode = TextWrappingModes.NoWrap;
            temperatureLabel.raycastTarget = false;

            var barTop = -_config.HudTopMargin - (panelSize.y - barSize.y) * 0.5f;
            _timeline = new ForecastTimelineView(
                (RectTransform)transform, new Vector2(left + panelSize.x + _config.TimelineGap, barTop), _config, _texts, _time);
            if (_hasForecast)
            {
                _timeline.SetForecast(_forecast);
            }

            if (_hasRescue)
            {
                _timeline.SetRescue(_rescueDelay);
            }
        }

        private void LateUpdate()
        {
            // Whole degrees; adding 0 turns a rounded -0 into 0 so the label never reads "-0".
            var current = Mathf.Round(_temperature.CurrentTemperature) + 0f;
            if (!Mathf.Approximately(current, _shownTemperature))
            {
                _shownTemperature = current;
                temperatureLabel.text = _texts.Format(TemperatureKey, current);
                temperatureLabel.color = current >= _config.TemperatureWarmThreshold
                    ? _config.TemperatureWarmColor
                    : current <= _config.TemperatureFrozenThreshold ? _config.TemperatureFrozenColor : _config.TemperatureColdColor;
            }

            _timeline.Refresh();
        }

        private void OnDestroy()
        {
            _forecastSubscription?.Dispose();
            _rescueSubscription?.Dispose();
            _timeline?.Dispose();
        }
    }
}
