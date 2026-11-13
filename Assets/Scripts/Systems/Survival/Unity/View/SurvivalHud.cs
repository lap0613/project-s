using TMPro;
using UnityEngine;
using VContainer;
using Whiteout.Core.Unity;

namespace Whiteout.Survival.Unity
{
    /// <summary>
    /// Three gauge bars (체온 / 허기 / 체력), top-left under the temperature (ui-plan section 7.1, D2).
    /// Each bar is a RectTransform stretched inside its background; its right anchor is set to the gauge ratio,
    /// so no sprite is needed. The labels are TextMeshPro and take their text and font from the
    /// <see cref="UiTextTable"/> (tech-decisions item 24).
    /// </summary>
    /// <example>
    /// <code>
    /// // Put on a Canvas object; assign the three fill RectTransforms (anchors min (0,0), max (1,1), zero offsets)
    /// // and the three TextMeshProUGUI labels.
    /// // GameLifetimeScope registers it with RegisterComponentInHierarchy&lt;SurvivalHud&gt;().
    /// </code>
    /// </example>
    public sealed class SurvivalHud : MonoBehaviour
    {
        private const string TemperatureKey = "hud.gauge.temperature";
        private const string HungerKey = "hud.gauge.hunger";
        private const string HealthKey = "hud.gauge.health";

        [SerializeField] private RectTransform temperatureFill;
        [SerializeField] private RectTransform hungerFill;
        [SerializeField] private RectTransform healthFill;

        [SerializeField] private TMP_Text temperatureLabel;
        [SerializeField] private TMP_Text hungerLabel;
        [SerializeField] private TMP_Text healthLabel;

        private SurvivalSystem _survival;
        private SurvivalSettings _settings;
        private UiTextTable _texts;

        /// <summary>Receives dependencies from the container.</summary>
        /// <param name="survival">The hurdles to show.</param>
        /// <param name="settings">Supplies the gauge maximum.</param>
        /// <param name="texts">Label texts and font.</param>
        [Inject]
        public void Construct(SurvivalSystem survival, SurvivalSettings settings, UiTextTable texts)
        {
            _survival = survival;
            _settings = settings;
            _texts = texts;
        }

        private void Start()
        {
            SetLabel(temperatureLabel, TemperatureKey);
            SetLabel(hungerLabel, HungerKey);
            SetLabel(healthLabel, HealthKey);
        }

        private void Update()
        {
            SetFill(temperatureFill, _survival.Temperature.Value);
            SetFill(hungerFill, _survival.Hunger.Value);
            SetFill(healthFill, _survival.Health.Value);
        }

        private void SetLabel(TMP_Text label, string key)
        {
            _texts.ApplyFont(label);
            label.text = _texts.Get(key);
        }

        private void SetFill(RectTransform fill, float value)
        {
            var ratio = Mathf.Clamp01(value / _settings.MaxValue);
            if (!Mathf.Approximately(fill.anchorMax.x, ratio))
            {
                fill.anchorMax = new Vector2(ratio, 1f);
            }
        }
    }
}
