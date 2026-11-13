using System;
using TMPro;
using UnityEngine;
using VContainer;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;
using Whiteout.Core.Unity;

namespace Whiteout.UI
{
    /// <summary>
    /// Two HUD texts (ui-plan section 7.1, D2): the current temperature, top-left above the gauges, and the
    /// today / tomorrow / day-after forecast, top-centre. TextMeshPro labels; texts and font come from the
    /// <see cref="UiTextTable"/> (tech-decisions item 24). Unsubscribes when destroyed.
    /// </summary>
    /// <example>
    /// <code>
    /// // Put on a Canvas object; assign Temperature Label (top-left) and Forecast Label (top-centre).
    /// // GameLifetimeScope registers it with RegisterComponentInHierarchy&lt;WeatherHud&gt;().
    /// </code>
    /// </example>
    public sealed class WeatherHud : MonoBehaviour
    {
        private const string TemperatureKey = "hud.temperature";
        private const string ForecastKey = "hud.forecast";

        [SerializeField] private TMP_Text temperatureLabel;
        [SerializeField] private TMP_Text forecastLabel;

        private IAmbientTemperatureSource _temperature;
        private UiTextTable _texts;
        private IDisposable _forecastSubscription;
        private WeatherForecastUpdated _forecast;
        private float _shownTemperature = float.NaN;
        private bool _forecastDirty = true;

        /// <summary>Receives dependencies from the container.</summary>
        /// <param name="bus">Event channel.</param>
        /// <param name="temperature">Ambient temperature at the player.</param>
        /// <param name="texts">Texts and font.</param>
        [Inject]
        public void Construct(IEventBus bus, IAmbientTemperatureSource temperature, UiTextTable texts)
        {
            _temperature = temperature;
            _texts = texts;
            _forecastSubscription = bus.Subscribe<WeatherForecastUpdated>(e =>
            {
                _forecast = e;
                _forecastDirty = true;
            });
        }

        private void Start()
        {
            _texts.ApplyFont(temperatureLabel);
            _texts.ApplyFont(forecastLabel);
        }

        private void Update()
        {
            // Compare at the shown precision (one decimal) so the label is rebuilt only when its text changes.
            var current = Mathf.Round(_temperature.CurrentTemperature * 10f) / 10f;
            if (!Mathf.Approximately(current, _shownTemperature))
            {
                _shownTemperature = current;
                temperatureLabel.text = _texts.Format(TemperatureKey, current);
            }

            if (_forecastDirty)
            {
                _forecastDirty = false;
                forecastLabel.text = _texts.Format(
                    ForecastKey, _forecast.Today.DisplayName, _forecast.Tomorrow.DisplayName, _forecast.DayAfter.DisplayName);
            }
        }

        private void OnDestroy()
        {
            _forecastSubscription?.Dispose();
        }
    }
}
