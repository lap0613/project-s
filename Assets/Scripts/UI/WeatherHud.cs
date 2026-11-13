using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using Whiteout.Core.Contracts;
using Whiteout.Core.Events;

namespace Whiteout.UI
{
    /// <summary>
    /// Minimal HUD text: today / tomorrow / day-after weather (from <see cref="WeatherForecastUpdated"/>)
    /// and the current temperature (read from <see cref="IAmbientTemperatureSource"/>).
    /// Uses one uGUI <see cref="Text"/>. Unsubscribes when destroyed.
    /// </summary>
    /// <example>
    /// <code>
    /// // Put on a Canvas object with a Text child assigned to Label.
    /// // GameLifetimeScope registers it with RegisterComponentInHierarchy&lt;WeatherHud&gt;().
    /// </code>
    /// </example>
    public sealed class WeatherHud : MonoBehaviour
    {
        [SerializeField] private Text label;

        [Tooltip("Text layout. {0} today, {1} tomorrow, {2} day after, {3} temperature in degrees C.")]
        [SerializeField, TextArea(2, 5)] private string format = "오늘: {0}\n내일: {1}\n모레: {2}\n기온: {3:0.#}°C";

        private IAmbientTemperatureSource _temperature;
        private IDisposable _forecastSubscription;
        private WeatherForecastUpdated _forecast;
        private float _shownTemperature = float.NaN;
        private bool _dirty = true;

        /// <summary>Receives dependencies from the container.</summary>
        /// <param name="bus">Event channel.</param>
        /// <param name="temperature">Ambient temperature at the player.</param>
        [Inject]
        public void Construct(IEventBus bus, IAmbientTemperatureSource temperature)
        {
            _temperature = temperature;
            _forecastSubscription = bus.Subscribe<WeatherForecastUpdated>(e =>
            {
                _forecast = e;
                _dirty = true;
            });
        }

        private void Update()
        {
            var current = _temperature.CurrentTemperature;
            if (!_dirty && Mathf.Approximately(current, _shownTemperature))
            {
                return;
            }

            _shownTemperature = current;
            _dirty = false;
            label.text = string.Format(
                format, _forecast.Today.DisplayName, _forecast.Tomorrow.DisplayName, _forecast.DayAfter.DisplayName, current);
        }

        private void OnDestroy()
        {
            _forecastSubscription?.Dispose();
        }
    }
}
